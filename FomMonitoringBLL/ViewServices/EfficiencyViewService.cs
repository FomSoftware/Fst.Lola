using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringResources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class EfficiencyViewService : IEfficiencyViewService
    {
        private IStateService _stateService;
        private IContextService _contextService;

        public EfficiencyViewService(IStateService stateService, IContextService contextService)
        {
            _stateService = stateService;
            _contextService = contextService;
        }
        public EfficiencyViewModel GetEfficiency(ContextModel context)
        {
            EfficiencyViewModel result = new EfficiencyViewModel();
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
            result.opt_shifts = GetShiftsOptions(context.ActualMachine, context.ActualPeriod);
            result.opt_states = GetStatesOptions(context.ActualMachine, context.ActualPeriod);

            return result;
        }


        public EfficiencyVueModel GetVueModel(MachineInfoModel machine, PeriodModel period)
        {
            EfficiencyVueModel result = new EfficiencyVueModel();

            List<HistoryStateModel> data = _stateService.GetAggregationStates(machine, period, enDataType.Dashboard);

            if (data.Count == 0)
                return result;

            HistoryStateModel stateProd = data.Where(w => w.enState == enState.Production).FirstOrDefault();

            long? totalProd = stateProd?.ElapsedTime;
            long? totalOn = data.Where(w => w.enState != enState.Off).Select(s => s.ElapsedTime).Sum();
            //long? totalOff = data.Where(w => w.enState == enState.Off).Select(s => s.ElapsedTime).Sum();

            decimal? percProd = Common.GetPercentage(totalProd, totalOn);
            decimal? overfeed = stateProd?.OverfeedAvg;

            result.kpi = CommonViewService.getKpiViewModel(percProd, machine.StateProductivityGreenThreshold, machine.StateProductivityYellowThreshold);
            result.overfeed = CommonViewService.getKpiViewModel(overfeed, machine.OverfeedGreenThreshold, machine.OverfeedYellowThreshold);

            TotalTimeModel total = new TotalTimeModel();
            total.on = CommonViewService.getTimeViewModel(totalOn);
            //total.off = CommonViewService.getTimeViewModel(totalOff);
            result.total = total;

            List<StateViewModel> states = new List<StateViewModel>();

            // state prod
            StateViewModel prod = new StateViewModel();
            prod.code = enState.Production.GetDescription();
            prod.text = enState.Production.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            prod.perc = percProd;
            prod.time = CommonViewService.getTimeViewModel(totalProd);
            states.Add(prod);

            // state pause
            StateViewModel pause = new StateViewModel();
            pause.code = enState.Pause.GetDescription();
            pause.text = enState.Pause.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);

            HistoryStateModel statePause = data.Where(w => w.enState == enState.Pause).FirstOrDefault();

            if (statePause != null)
            {
                long? totalPause = statePause.ElapsedTime;
                pause.perc = Common.GetPercentage(totalPause, totalOn);
                pause.time = CommonViewService.getTimeViewModel(totalPause);
            }
            states.Add(pause);

            // state manual
            StateViewModel manual = new StateViewModel();
            manual.code = enState.Manual.GetDescription();
            manual.text = enState.Manual.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);

            HistoryStateModel stateManual = data.Where(w => w.enState == enState.Manual).FirstOrDefault();

            if (stateManual != null)
            {
                long? totalManual = stateManual.ElapsedTime;
                manual.perc = Common.GetPercentage(totalManual, totalOn);
                manual.time = CommonViewService.getTimeViewModel(totalManual);
            }
            states.Add(manual);

            // state error
            StateViewModel error = new StateViewModel();
            error.code = enState.Error.GetDescription();
            error.text = enState.Error.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);

            HistoryStateModel stateError = data.Where(w => w.enState == enState.Error).FirstOrDefault();

            if (stateError != null)
            {
                long? totalError = stateError.ElapsedTime;
                error.perc = Common.GetPercentage(totalError, totalOn);
                error.time = CommonViewService.getTimeViewModel(totalError);
            }
            states.Add(error);

            result.states = states.OrderByDescending(o => o.perc).ToList();

            return result;
        }


        private ChartViewModel GetHistoricalOptions(MachineInfoModel machine, PeriodModel period)
        {
            ChartViewModel options = new ChartViewModel();

            enAggregation granularity = Common.GetAggregationType(period.StartDate, period.EndDate);
            //DateTime startDateTrend = Common.GetStartDateTrend(period.EndDate, granularity);

            PeriodModel periodTrend = new PeriodModel();
            periodTrend.StartDate = period.StartDate.ToUniversalTime();
            periodTrend.EndDate = period.EndDate.ToUniversalTime();
            periodTrend.Aggregation = granularity;

            List<HistoryStateModel> data = _stateService.GetAggregationStates(machine, periodTrend, enDataType.Historical).Where(w => w.enState != enState.Off).OrderBy(o => o.Day).ToList();

            if (data.Count == 0)
                return null;

            // calcolo dell'unità di misura delle serie del grafico sul valore medio 
            long avgData = (long)data.Average(a => a.ElapsedTime ?? 0);
            enMeasurement measurement = Common.GetTimeMeasurement(avgData);
            options.yTitle = string.Format("{0} ({1})", Resource.Duration, measurement.GetDescription());
            options.valueSuffix = string.Format(" {0}", measurement.GetDescription());

            List<DateTime> days = data.Where(w => w.Day != null).Select(s => s.Day.Value).Distinct().ToList();
            options.categories = CommonViewService.GetTimeCategories(days, granularity);

            List<SerieViewModel> series = new List<SerieViewModel>();

            SerieViewModel serieProd = new SerieViewModel();
            serieProd.name = enState.Production.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieProd.color = CommonViewService.GetColorState(enState.Production);
            serieProd.data = Common.ConvertElapsedByMeasurement(data.Where(w => w.enState == enState.Production).Select(s => s.ElapsedTime ?? 0).ToList(), measurement);
            series.Add(serieProd);

            SerieViewModel seriePause = new SerieViewModel();
            seriePause.name = enState.Pause.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            seriePause.color = CommonViewService.GetColorState(enState.Pause);
            seriePause.data = Common.ConvertElapsedByMeasurement(data.Where(w => w.enState == enState.Pause).Select(s => s.ElapsedTime ?? 0).ToList(), measurement);
            series.Add(seriePause);

            SerieViewModel serieManual = new SerieViewModel();
            serieManual.name = enState.Manual.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieManual.color = CommonViewService.GetColorState(enState.Manual);
            serieManual.data = Common.ConvertElapsedByMeasurement(data.Where(w => w.enState == enState.Manual).Select(s => s.ElapsedTime ?? 0).ToList(), measurement);
            series.Add(serieManual);

            SerieViewModel serieError = new SerieViewModel();
            serieError.name = enState.Error.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieError.color = CommonViewService.GetColorState(enState.Error);
            serieError.data = Common.ConvertElapsedByMeasurement(data.Where(w => w.enState == enState.Error).Select(s => s.ElapsedTime ?? 0).ToList(), measurement);
            series.Add(serieError);

            options.series = series;

            return options;
        }


        private ChartViewModel GetOperatorsOptions(MachineInfoModel machine, PeriodModel period)
        {
            ChartViewModel options = new ChartViewModel();

            List<HistoryStateModel> data = _stateService.GetAggregationStates(machine, period, enDataType.Operators).Where(w => w.enState != enState.Off).OrderBy(o => o.Day).ToList();

            if (data.Count == 0)
                return null;

            List<string> operators = data.Select(s => s.Operator).Distinct().ToList();
            options.categories = operators;
            options.yTitle = string.Format("{0} (%)", Resource.TimeOn);
            options.series = GetSeriesStackedBarChart(data, operators, enDataType.Operators);

            return options;
        }

        private ChartViewModel GetKpisOptions(MachineInfoModel machine, PeriodModel period)
        {
            ChartViewModel options = new ChartViewModel();
            List<HistoryStateModel> data = _stateService.GetAggregationStates(machine, period, enDataType.Dashboard);


            HistoryStateModel stateProd = data.Where(w => w.enState == enState.Production).FirstOrDefault();

            long? totalProd = stateProd?.ElapsedTime;
            long? totalOn = data.Where(w => w.enState != enState.Off).Select(s => s.ElapsedTime).Sum();
            long? totalOff = data.Where(w => w.enState == enState.Off).Select(s => s.ElapsedTime).Sum();

            decimal? percProd = Common.GetPercentage(totalProd, totalOn);
            decimal? overfeed = stateProd?.OverfeedAvg;

            options.series = new List<SerieViewModel>(){
                new SerieViewModel {
                    stateProductivityGreenThreshold = machine.StateProductivityGreenThreshold ?? 0,
                    stateProductivityYellowThreshold = machine.StateProductivityYellowThreshold ?? 0,
                    y = CommonViewService.getKpiViewModel(percProd, machine.StateProductivityGreenThreshold, machine.StateProductivityYellowThreshold).value ?? (decimal)0
                }
            };
            return options;
        }

        private ChartViewModel GetShiftsOptions(MachineInfoModel machine, PeriodModel period)
        {
            ChartViewModel options = new ChartViewModel();

            List<HistoryStateModel> data = _stateService.GetAggregationStates(machine, period, enDataType.Shifts).Where(w => w.enState != enState.Off).OrderBy(o => o.Day).ToList();

            if (data.Count == 0)
                return null;

            List<string> shifts = data.Select(s => s.Shift.ToString()).Distinct().ToList();
            options.categories = shifts.Select(s => string.Format("{0}° {1}", s, Resource.Shift)).ToList();
            options.yTitle = string.Format("{0} (%)", Resource.TimeOn);
            options.series = GetSeriesStackedBarChart(data, shifts, enDataType.Shifts);

            return options;
        }

        private ChartViewModel GetStatesOptions(MachineInfoModel machine, PeriodModel period)
        {
            ChartViewModel options = new ChartViewModel();
            options.series = new List<SerieViewModel>();


            List<HistoryStateModel> data = _stateService.GetAggregationStates(machine, period, enDataType.Dashboard);

            if (!data.Any())
                return options;

            HistoryStateModel stateProd = data.FirstOrDefault(w => w.enState == enState.Production);
            HistoryStateModel statePause = data.FirstOrDefault(w => w.enState == enState.Pause);

            HistoryStateModel stateError = data.FirstOrDefault(w => w.enState == enState.Error);
            HistoryStateModel stateManual = data.FirstOrDefault(w => w.enState == enState.Manual);

            long? totalProd = stateProd?.ElapsedTime ?? 0;
            long? totalPause = statePause?.ElapsedTime ?? 0;
            long? totalError = stateError?.ElapsedTime ?? 0;
            long? totalManual = stateManual?.ElapsedTime ?? 0;

            long? totalOn = data.Where(w => w.enState != enState.Off).Select(s => s.ElapsedTime).Sum();
            long? totalOff = data.Where(w => w.enState == enState.Off).Select(s => s.ElapsedTime).Sum();

            decimal? percProd = Common.GetPercentage(totalProd, totalOn);
            decimal? percPause = Common.GetPercentage(totalPause, totalOn);
            decimal? percError = Common.GetPercentage(totalError, totalOn);
            decimal? percManual = Common.GetPercentage(totalManual, totalOn);

            decimal? overfeed = stateProd.OverfeedAvg;

            List<StateViewModel> states = new List<StateViewModel>();

            // state prod
            SerieViewModel prod = new SerieViewModel();
            prod.name = enState.Production.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            prod.y = percProd ?? 0;
            prod.color = CommonViewService.GetColorState(enState.Production);
            options.series.Add(prod);
            
            // state pause
            SerieViewModel pause = new SerieViewModel();
            pause.name = enState.Pause.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            pause.y = percPause ?? 0;
            pause.color = CommonViewService.GetColorState(enState.Pause);
            options.series.Add(pause);


            SerieViewModel manual = new SerieViewModel();
            manual.name = enState.Manual.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            manual.y = percManual ?? 0;
            manual.color = CommonViewService.GetColorState(enState.Manual);
            options.series.Add(manual);


            SerieViewModel error = new SerieViewModel();
            error.name = enState.Error.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            error.y = percError ?? 0;
            error.color = CommonViewService.GetColorState(enState.Error);
            options.series.Add(error);

            
            return options;
        }

        private List<SerieViewModel> GetSeriesStackedBarChart(List<HistoryStateModel> data, List<string> categories, enDataType chart)
        {
            List<SerieViewModel> series = new List<SerieViewModel>();

            SerieViewModel serieProd = new SerieViewModel();
            serieProd.name = enState.Production.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieProd.color = CommonViewService.GetColorState(enState.Production);
            serieProd.data = new List<int>();

            SerieViewModel seriePause = new SerieViewModel();
            seriePause.name = enState.Pause.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            seriePause.color = CommonViewService.GetColorState(enState.Pause);
            seriePause.data = new List<int>();

            SerieViewModel serieManual = new SerieViewModel();
            serieManual.name = enState.Manual.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieManual.color = CommonViewService.GetColorState(enState.Manual);
            serieManual.data = new List<int>();

            SerieViewModel serieError = new SerieViewModel();
            serieError.name = enState.Error.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieError.color = CommonViewService.GetColorState(enState.Error);
            serieError.data = new List<int>();

            // create data foreach categories (operator or shift)
            foreach (string categorie in categories)
            {
                List<HistoryStateModel> dataCategorie = new List<HistoryStateModel>();

                switch (chart)
                {
                    case enDataType.Operators:
                        dataCategorie = data.Where(w => w.Operator == categorie).ToList();
                        break;
                    case enDataType.Shifts:
                        dataCategorie = data.Where(w => w.Shift.ToString() == categorie).ToList();
                        break;
                }

                if (dataCategorie.Count == 0)
                    continue;

                long? totalOn = dataCategorie.Where(w => w.enState != enState.Off).Select(s => s.ElapsedTime).Sum();

                HistoryStateModel stateProd = dataCategorie.Where(w => w.enState == enState.Production).FirstOrDefault();

                if (stateProd != null)
                {
                    long? totalProd = stateProd.ElapsedTime;
                    decimal? percProd = Common.GetPercentage(totalProd, totalOn);
                    serieProd.data.Add((percProd ?? 0).RoundToInt());
                }
                else
                    serieProd.data.Add(0);

                HistoryStateModel statePause = dataCategorie.Where(w => w.enState == enState.Pause).FirstOrDefault();

                if (statePause != null)
                {
                    long? totalPause = statePause.ElapsedTime;
                    decimal? percPause = Common.GetPercentage(totalPause, totalOn);
                    seriePause.data.Add((percPause ?? 0).RoundToInt());
                }
                else
                    seriePause.data.Add(0);

                HistoryStateModel stateManual = dataCategorie.Where(w => w.enState == enState.Manual).FirstOrDefault();

                if (stateManual != null)
                {
                    long? totalManual = stateManual.ElapsedTime;
                    decimal? percManual = Common.GetPercentage(totalManual, totalOn);
                    serieManual.data.Add((percManual ?? 0).RoundToInt());
                }
                else
                    serieManual.data.Add(0);

                HistoryStateModel stateError = dataCategorie.Where(w => w.enState == enState.Error).FirstOrDefault();

                if (stateError != null)
                {
                    long? totalError = stateError.ElapsedTime;
                    decimal? percError = Common.GetPercentage(totalError, totalOn);
                    serieError.data.Add((percError ?? 0).RoundToInt());
                }
                else
                    serieError.data.Add(0);
            }

            series.Add(serieError);
            series.Add(serieManual);
            series.Add(seriePause);
            series.Add(serieProd);

            return series;
        }

    }
}
