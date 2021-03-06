﻿using System;
using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringResources;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.Extensions;

namespace FomMonitoringBLL.ViewServices
{
    public class EfficiencyViewService : IEfficiencyViewService
    {
        private readonly IStateService _stateService;
        private readonly IContextService _contextService;

        public EfficiencyViewService(IStateService stateService, IContextService contextService)
        {
            _stateService = stateService;
            _contextService = contextService;
        }
        public EfficiencyViewModel GetEfficiency(ContextModel context)
        {
            var result = new EfficiencyViewModel();
            result.vm_machine_info = new MachineInfoViewModel
            {
                model = context.ActualMachine.Model.Name,
                mtype = context.ActualMachine.Type.Name,
                id_mtype = context.ActualMachine.Type.Id,
                machineName = context.ActualMachine.MachineName
                
            };
            result.vm_efficiency = GetVueModel(context.ActualMachine, context.ActualPeriod);
            result.opt_historical = GetHistoricalOptions(context.ActualMachine, context.ActualPeriod);
            result.opt_kpis = GetKpisOptions(context.ActualMachine, context.ActualPeriod);
            result.opt_operators = GetOperatorsOptions(context.ActualMachine, context.ActualPeriod);
            result.opt_states = GetStatesOptions(context.ActualMachine, context.ActualPeriod);

            return result;
        }


        private EfficiencyVueModel GetVueModel(MachineInfoModel machine, PeriodModel period)
        {
            var result = new EfficiencyVueModel();

            var data = _stateService.GetAggregationStates(machine, period, enDataType.Dashboard);

            if (data.Count == 0)
                return result;

            var stateProd = data.FirstOrDefault(w => w.enState == enState.Production);

            var totalProd = stateProd?.ElapsedTime;



            var totalOn = data.Where(w => w.enState != enState.Offline).Select(s => s.ElapsedTime).Sum();
            //long? totalOff = data.Where(w => w.enState == enState.Offline).Select(s => s.ElapsedTime).Sum();

            decimal? percProd = Common.GetPercentage(totalProd, totalOn);


            var total = new TotalTimeModel();
            total.on = CommonViewService.getTimeViewModel(totalOn);
            //total.off = CommonViewService.getTimeViewModel(totalOff);
            result.total = total;

            var states = new List<StateViewModel>();

            // state prod
            var prod = new StateViewModel();
            prod.code = enState.Production.GetDescription();
            prod.text = enState.Production.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            prod.perc = percProd;
            prod.time = CommonViewService.getTimeViewModel(totalProd);
            prod.orderView = _stateService.GetOrderViewState(enState.Production.GetDescription());
            states.Add(prod);

            // state pause
            var pause = new StateViewModel();
            pause.code = enState.Pause.GetDescription();
            pause.text = enState.Pause.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);

            var statePause = data.FirstOrDefault(w => w.enState == enState.Pause);

            if (statePause != null)
            {
                var totalPause = statePause.ElapsedTime;
                pause.perc = Common.GetPercentage(totalPause, totalOn);
                pause.time = CommonViewService.getTimeViewModel(totalPause);
            }
            pause.orderView = _stateService.GetOrderViewState(enState.Pause.GetDescription());
            states.Add(pause);

            // state manual
            var manual = new StateViewModel();
            manual.code = enState.Manual.GetDescription();
            manual.text = enState.Manual.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);

            var stateManual = data.FirstOrDefault(w => w.enState == enState.Manual);

            if (stateManual != null)
            {
                var totalManual = stateManual.ElapsedTime;
                manual.perc = Common.GetPercentage(totalManual, totalOn);
                manual.time = CommonViewService.getTimeViewModel(totalManual);
            }
            manual.orderView = _stateService.GetOrderViewState(enState.Manual.GetDescription());
            states.Add(manual);

            // state error
            var error = new StateViewModel();
            error.code = enState.Error.GetDescription();
            error.text = enState.Error.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);

            var stateError = data.FirstOrDefault(w => w.enState == enState.Error);

            if (stateError != null)
            {
                var totalError = stateError.ElapsedTime;
                error.perc = Common.GetPercentage(totalError, totalOn);
                error.time = CommonViewService.getTimeViewModel(totalError);
            }
            error.orderView = _stateService.GetOrderViewState(enState.Error.GetDescription());
            states.Add(error);

            result.states = states.OrderBy(o => o.orderView).ToList();


            var overfeed = stateProd?.OverfeedAvg;

            decimal percKpi;
            if (machine.MachineTypeId == (int)enMachineType.Troncatrice || machine.MachineTypeId == (int)enMachineType.CentroLavoro)
            {
                percKpi = (decimal) percProd + (manual.perc ?? 0);
            }
            else
            {
                percKpi = (decimal) percProd;
            }

            result.kpi = CommonViewService.getKpiViewModel(percKpi, machine.StateProductivityGreenThreshold, machine.StateProductivityYellowThreshold);
            result.overfeed = CommonViewService.getKpiViewModel(overfeed, machine.OverfeedGreenThreshold, machine.OverfeedYellowThreshold);


            return result;
        }


        private ChartViewModel GetHistoricalOptions(MachineInfoModel machine, PeriodModel period)
        {
            var options = new ChartViewModel();

            var granularity = Common.GetAggregationType(period.StartDate, period.EndDate);
            var startDateTrend = Common.GetStartDateTrend(period.EndDate, period.StartDate, granularity);

            var periodTrend = new PeriodModel
            {
                StartDate = startDateTrend.ToUniversalTime().Date,
                EndDate = period.EndDate.ToUniversalTime().Date.AddDays(1).AddMinutes(-1),
                Aggregation = granularity
            };

            var data = _stateService.GetAggregationStates(machine, periodTrend, enDataType.Historical).Where(w => w.enState != enState.Offline).OrderBy(o => o.Day).ToList();

            if (data.Count == 0)
                return null;

            // calcolo dell'unità di misura delle serie del grafico sul valore medio 
            var avgData = (long)data.Average(a => a.ElapsedTime ?? 0);
            var measurement = Common.GetTimeMeasurement(avgData);
            options.yTitle = $"{Resource.Duration} ({measurement.GetDescription()})";
            options.valueSuffix = $" {measurement.GetDescription()}";

            var days = data.Where(w => w.Day != null).Select(s => s.Day.Value).Distinct().ToList();
            options.categories = CommonViewService.GetTimeCategories(days, granularity);

            var series = new List<SerieViewModel>();

            var serieProd = new SerieViewModel();
            serieProd.name = enState.Production.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieProd.color = CommonViewService.GetColorState(enState.Production);
            List<long> lista = GetDataGroup(data, days, granularity, enState.Production);
            serieProd.data = Common.ConvertElapsedByMeasurement(lista, measurement);
            series.Add(serieProd);


            var serieManual = new SerieViewModel();
            serieManual.name = enState.Manual.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieManual.color = CommonViewService.GetColorState(enState.Manual);
            serieManual.data = Common.ConvertElapsedByMeasurement(GetDataGroup(data, days, granularity, enState.Manual), measurement);
            series.Add(serieManual);

            var seriePause = new SerieViewModel();
            seriePause.name = enState.Pause.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            seriePause.color = CommonViewService.GetColorState(enState.Pause);
            seriePause.data = Common.ConvertElapsedByMeasurement(GetDataGroup(data, days, granularity, enState.Pause), measurement);
            series.Add(seriePause);

            var serieError = new SerieViewModel();
            serieError.name = enState.Error.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieError.color = CommonViewService.GetColorState(enState.Error);
            serieError.data = Common.ConvertElapsedByMeasurement(GetDataGroup(data, days, granularity, enState.Error), measurement);
            series.Add(serieError);

            options.series = series;

            return options;
        }

        private List<long> GetDataGroup(List<HistoryStateModel> data, List<DateTime> days, enAggregation granularity, enState state)
        {
            List<long> lista = new List<long>();

            switch (granularity)
            {
                case enAggregation.Day:
                    lista = days.Select(d =>
                        data.Where(w => w.enState == state && w.Day.Value == d)
                            .Select(s => s.ElapsedTime ?? 0).DefaultIfEmpty(0).First()).ToList();
                    break;
                case enAggregation.Week:
                    var weeks = data.Where(w => w.enState == state).GroupBy(a => a.Day.Value.GetWeekNumber())
                        .Select(n => new { week = n.Key, somma = n.Sum(o => o.ElapsedTime ?? 0) }).ToList();
                    lista = days.Select(s => s.GetWeekNumber()).Distinct().ToList()
                        .Select(d => weeks.Where(l => l.week == d).FirstOrDefault()?.somma ?? 0).ToList();
                    break;
                case enAggregation.Month:
                    var months = data.Where(w => w.enState == state).GroupBy(a => a.Day.Value.Month)
                        .Select(n => new { month = n.Key, somma = n.Sum(o => o.ElapsedTime ?? 0) }).ToList();
                    lista = days.Select(s => s.Month).Distinct().ToList()
                        .Select(d => months.Where(l => l.month == d).FirstOrDefault()?.somma ?? 0).ToList();
                    break;
                case enAggregation.Quarter:
                    var quarters = data.Where(w => w.enState == state).GroupBy(a => a.Day.Value.GetQuarter() + (a.Day.Value.Year * 10))
                        .Select(n => new { quarter = n.Key, somma = n.Sum(o => o.ElapsedTime ?? 0) }).ToList();
                    lista = days.Select(s => s.GetQuarter() + (s.Year * 10)).Distinct().ToList()
                        .Select(d => quarters.Where(l => l.quarter == d).FirstOrDefault()?.somma ?? 0).ToList();
                    break;
                case enAggregation.Year:
                    var years = data.Where(w => w.enState == state).GroupBy(a => a.Day.Value.Year)
                        .Select(n => new { year = n.Key, somma = n.Sum(o => o.ElapsedTime ?? 0) }).ToList();
                    lista = days.Select(s => s.Year).Distinct().ToList()
                        .Select(d => years.Where(l => l.year == d).FirstOrDefault()?.somma ?? 0).ToList();
                    break;
                default:
                    break;
            }

            return lista;
        }

        private long CalculateEfficiencyOperator(string operatorName, IList<HistoryStateModel> states, MachineInfoModel machine)
        {
            long efficiency;
            var totalTime = states.Where(w => w.Operator == operatorName).Sum(w => w.ElapsedTime ?? 0);

            if (totalTime == 0)
            {
                return 0;
            }


            if (machine.MachineTypeId == (int)enMachineType.Troncatrice || machine.MachineTypeId == (int)enMachineType.CentroLavoro)
            {
                efficiency = states
                    .Where(w => (w.enState == enState.Manual || w.enState == enState.Production) &&
                                w.Operator == operatorName).Sum(w => w.ElapsedTime ?? 0);
                return efficiency * 100 / totalTime;
            }
            efficiency = states
                .Where(w => w.enState == enState.Production &&
                            w.Operator == operatorName).Sum(w => w.ElapsedTime ?? 0);
            return efficiency * 100 / totalTime;
        }

        private ChartViewModel GetOperatorsOptions(MachineInfoModel machine, PeriodModel period)
        {
            var options = new ChartViewModel();

            var data = _stateService.GetAggregationStates(machine, period, enDataType.Operators).Where(w => w.enState != enState.Offline && w.Operator != "Other").OrderBy(o => o.Day).ToList();

            if (data.Count == 0)
                return null;

            var operators = data
                .Select(s => new {s.Operator, Efficiency = CalculateEfficiencyOperator(s.Operator, data, machine)})
                .OrderByDescending(p => p.Efficiency)
                .Select(x => x.Operator)
                .Distinct()
                .ToList();

            options.categories = operators;
            options.yTitle = $"{Resource.TimeOn} (%)";
            options.series = GetSeriesStackedBarChart(data, operators, enDataType.Operators);

            return options;
        }

        private ChartViewModel GetKpisOptions(MachineInfoModel machine, PeriodModel period)
        {
            var options = new ChartViewModel();
            var data = _stateService.GetAggregationStates(machine, period, enDataType.Dashboard);


            var stateProd = data.FirstOrDefault(w => w.enState == enState.Production);

            var stateManual = data.FirstOrDefault(w => w.enState == enState.Manual);
            

            var totalValue = machine.MachineTypeId == (int)enMachineType.Troncatrice || machine.MachineTypeId == (int)enMachineType.CentroLavoro ?
                stateProd?.ElapsedTime + stateManual?.ElapsedTime : stateProd?.ElapsedTime;
            var totalOn = data.Where(w => w.enState != enState.Offline).Select(s => s.ElapsedTime).Sum();

            decimal? percProd = Common.GetPercentage(totalValue, totalOn);

            options.series = new List<SerieViewModel>(){
                new SerieViewModel {
                    stateProductivityGreenThreshold = machine.StateProductivityGreenThreshold ?? 0,
                    stateProductivityYellowThreshold = machine.StateProductivityYellowThreshold ?? 0,
                    y = CommonViewService.getKpiViewModel(percProd, machine.StateProductivityGreenThreshold, machine.StateProductivityYellowThreshold).value ?? (decimal)0
                }
            };
            return options;
        }
        

        private ChartViewModel GetStatesOptions(MachineInfoModel machine, PeriodModel period)
        {
            var options = new ChartViewModel();
            options.series = new List<SerieViewModel>();


            var data = _stateService.GetAggregationStates(machine, period, enDataType.Dashboard);

            if (!data.Any())
                return options;

            var stateProd = data.FirstOrDefault(w => w.enState == enState.Production);
            var statePause = data.FirstOrDefault(w => w.enState == enState.Pause);

            var stateError = data.FirstOrDefault(w => w.enState == enState.Error);
            var stateManual = data.FirstOrDefault(w => w.enState == enState.Manual);

            long? totalProd = stateProd?.ElapsedTime ?? 0;
            long? totalPause = statePause?.ElapsedTime ?? 0;
            long? totalError = stateError?.ElapsedTime ?? 0;
            long? totalManual = stateManual?.ElapsedTime ?? 0;

            var totalOn = data.Where(w => w.enState != enState.Offline).Select(s => s.ElapsedTime).Sum();
            var totalOff = data.Where(w => w.enState == enState.Offline).Select(s => s.ElapsedTime).Sum();

            decimal? percProd = Common.GetPercentage(totalProd, totalOn);
            decimal? percPause = Common.GetPercentage(totalPause, totalOn);
            decimal? percError = Common.GetPercentage(totalError, totalOn);
            decimal? percManual = Common.GetPercentage(totalManual, totalOn);


            // state prod
            var prod = new SerieViewModel();
            prod.name = enState.Production.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            prod.y = percProd ?? 0;
            prod.color = CommonViewService.GetColorState(enState.Production);
            options.series.Add(prod);
            
            // state pause
            var pause = new SerieViewModel();
            pause.name = enState.Pause.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            pause.y = percPause ?? 0;
            pause.color = CommonViewService.GetColorState(enState.Pause);
            options.series.Add(pause);


            var manual = new SerieViewModel();
            manual.name = enState.Manual.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            manual.y = percManual ?? 0;
            manual.color = CommonViewService.GetColorState(enState.Manual);
            options.series.Add(manual);


            var error = new SerieViewModel();
            error.name = enState.Error.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            error.y = percError ?? 0;
            error.color = CommonViewService.GetColorState(enState.Error);
            options.series.Add(error);

            
            return options;
        }

        private List<SerieViewModel> GetSeriesStackedBarChart(List<HistoryStateModel> data, List<string> categories, enDataType chart)
        {
            var series = new List<SerieViewModel>();

            var serieProd = new SerieViewModel();
            serieProd.name = enState.Production.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieProd.color = CommonViewService.GetColorState(enState.Production);
            serieProd.data = new List<int>();

            var seriePause = new SerieViewModel();
            seriePause.name = enState.Pause.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            seriePause.color = CommonViewService.GetColorState(enState.Pause);
            seriePause.data = new List<int>();

            var serieManual = new SerieViewModel();
            serieManual.name = enState.Manual.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieManual.color = CommonViewService.GetColorState(enState.Manual);
            serieManual.data = new List<int>();

            var serieError = new SerieViewModel();
            serieError.name = enState.Error.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieError.color = CommonViewService.GetColorState(enState.Error);
            serieError.data = new List<int>();

            // create data foreach categories (operator or shift)
            foreach (var categorie in categories)
            {
                var dataCategorie = new List<HistoryStateModel>();

                switch (chart)
                {
                    case enDataType.Operators:
                        dataCategorie = data.Where(w => w.Operator == categorie).ToList();
                        break;
                }

                if (dataCategorie.Count == 0)
                    continue;

                var totalOn = dataCategorie.Where(w => w.enState != enState.Offline).Select(s => s.ElapsedTime).Sum();

                var stateProd = dataCategorie.FirstOrDefault(w => w.enState == enState.Production);

                if (stateProd != null)
                {
                    var totalProd = stateProd.ElapsedTime;
                    decimal? percProd = Common.GetPercentage(totalProd, totalOn);
                    serieProd.data.Add(((decimal) percProd).RoundToInt());
                }
                else
                    serieProd.data.Add(0);

                var stateManual = dataCategorie.FirstOrDefault(w => w.enState == enState.Manual);

                if (stateManual != null)
                {
                    var totalManual = stateManual.ElapsedTime;
                    decimal? percManual = Common.GetPercentage(totalManual, totalOn);
                    serieManual.data.Add(((decimal) percManual).RoundToInt());
                }
                else
                    serieManual.data.Add(0);

                var statePause = dataCategorie.FirstOrDefault(w => w.enState == enState.Pause);

                if (statePause != null)
                {
                    var totalPause = statePause.ElapsedTime;
                    decimal? percPause = Common.GetPercentage(totalPause, totalOn);
                    seriePause.data.Add(((decimal) percPause).RoundToInt());
                }
                else
                    seriePause.data.Add(0);

                var stateError = dataCategorie.FirstOrDefault(w => w.enState == enState.Error);

                if (stateError != null)
                {
                    var totalError = stateError.ElapsedTime;
                    decimal? percError = Common.GetPercentage(totalError, totalOn);
                    serieError.data.Add(((decimal) percError).RoundToInt());
                }
                else
                    serieError.data.Add(0);
            }

            series.Add(serieError);
            series.Add(seriePause);
            series.Add(serieManual);
            series.Add(serieProd);

            return series;
        }

    }
}
