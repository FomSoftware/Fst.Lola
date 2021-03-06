﻿using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringResources;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class ProductivityViewService : IProductivityViewService
    {
        private readonly IPieceService _pieceService;
        private readonly IContextService _contextService;
        private readonly IBarService _barService;
        private readonly IStateService _stateService;
        private readonly IMachineService _machineService;

        public ProductivityViewService(IPieceService pieceService, IContextService contextService, 
            IBarService barService, IStateService stateService, IMachineService machineService)
        {
            _pieceService = pieceService;
            _contextService = contextService;
            _barService = barService;
            _stateService = stateService;
            _machineService = machineService;
        }

        public ProductivityViewModel GetProductivity(ContextModel context)
        {
            var result = new ProductivityViewModel();

            result.vm_machine_info = new MachineInfoViewModel
            {
                model = context.ActualMachine.Model.Name,
                mtype = context.ActualMachine.Type.Name,
                id_mtype = context.ActualMachine.Type.Id,
                machineName = context.ActualMachine.MachineName

            };
            var operatorActivities = _stateService.GetOperatorsActivity(context.ActualMachine, context.ActualPeriod.StartDate, context.ActualPeriod.EndDate);

            result.vm_productivity = GetVueModel( operatorActivities, context.ActualMachine, context.ActualPeriod);
            result.opt_historical = GetHistoricalOptions( context.ActualMachine, context.ActualPeriod);
            result.opt_operators = GetOperatorsOptions(operatorActivities);
           

            return result;
        }

        /*private ParameterMachineValueModel GetVariables(MachineInfoModel machine)
        {
            ParameterMachineValueModel result = null;
            List<int> panels = _machineService.GetMachinePanels(machine.MachineModelId);
            if (panels != null)
            {
                if (panels.Contains((int) enPanel.ProductionLMX))
                {
                    result = _machineService.GetProductionValueModel(machine, enPanel.ProductionLMX);
                }
        }
        return result;
        }*/

        private CurrentStateModel GetCurrentState(int machineId)
        {
            //solo in questo caso (FMC) il dato lo devo leggere dal currentState
            var result = _machineService.GetCurrentStateModel(machineId);
            
            return result;
        }

        private ProductivityVueModel GetVueModel(List<EfficiencyStateMachineModel> operatorActivities, MachineInfoModel machine, PeriodModel period)
        {
             var result = new ProductivityVueModel();

             if (machine.MachineTypeId == (int)enMachineType.CentroLavoro || machine.MachineTypeId == (int)enMachineType.LineaTaglio ||
                 (machine.MachineTypeId == (int)enMachineType.LineaTaglioLavoro))
             {
                 result.currentState = GetCurrentState(machine.Id);
             }

             var data = _pieceService.GetAggregationPieces(machine, period, enDataType.Dashboard);
            //var operatorActivities =_stateService.GetOperatorsActivity(machine, period.StartDate, period.EndDate);
            if (data.Count == 0)
                return result;

            // tempo lordo: devo andare sulla tabella degli stati perchè non ce l'ho sui pezzi
            var grossTime = operatorActivities.Sum(m => m.TotalTime);
            var netTime = operatorActivities.Sum(m => m.ProducingTime);
            var productionTime = operatorActivities.Sum(m => m.ProducionModeTime ?? 0);
            //var grossTime = data.Select(s => s.ElapsedTime).Sum();
            var doneCount = data.Select(s => s.CompletedCount).Sum() ?? 0;
            var redoneCount = data.Select(s => s.RedoneCount).Sum() ?? 0;

            // kpi pezzi all'ora
            var ratio = Common.GetRatioProductivity(doneCount, grossTime);
            result.kpi = CommonViewService.getKpiViewModel(ratio, machine.PiecesProductivityGreenThreshold, machine.PiecesProductivityYellowThreshold);

            // pieces
            var piece = new PieceViewModel();
            piece.total = doneCount + redoneCount;

            var done = new ProdDataModel();
            done.perc = Common.GetPercentage(doneCount, piece.total);
            done.number = doneCount;
            piece.done = done;

            var redone = new ProdDataModel();
            redone.perc = Common.GetPercentage(redoneCount, piece.total);
            redone.number = redoneCount;
            piece.redone = redone;

            result.piece = piece;

            // materials
            var dataBars = _barService.GetAggregationBar(machine, period);

            if (dataBars.Count > 0)
            {
                var barLength = dataBars.Select(s => s.Length ?? 0).Sum().RoundToInt();
                double cutoffLength = dataBars.Select(s => s.OffcutLength ?? 0).Sum();
                var totalLength = barLength + cutoffLength;

                var material = new MaterialViewModel();
                material.total = (totalLength / 1000);

                var bar = new ProdDataModel();
                bar.perc = Common.GetPercentage(barLength, totalLength);
                bar.number = ((double) barLength / 1000);
                material.bar = bar;

                //Nelle troncatrici questi dati sono nascosti, non ci sono, 
                //per le altre è da verificare cosa prendono
                var cutoff = new ProdDataModel();
                cutoff.perc = Common.GetPercentage(cutoffLength, totalLength);
                cutoff.number = ((double)cutoffLength / 1000);
                material.cutoff = cutoff;

                result.material = material;
            }


            // phases
            var phases = new List<ProdDataModel>();
            switch (machine.MachineTypeId)
            {
                case (int)enMachineType.Troncatrice:
                case (int)enMachineType.LineaTaglio:
                    var cut = new ProdDataModel();
                    cut.text = Resource.Cut;
                    cut.perc = Common.GetPercentage(netTime, grossTime);
                    phases.Add(cut);
                    break;

                case (int)enMachineType.CentroLavoro:
                    var working = new ProdDataModel();
                    working.text = Resource.Working;
                    working.perc = Common.GetPercentage(productionTime, grossTime);
                    phases.Add(working);
                    break;

                default:
                    working = new ProdDataModel();
                    working.text = Resource.Working;
                    working.perc = Common.GetPercentage(data.Select(s => s.ElapsedTimeWorking ?? 0).Sum(), grossTime);
                    phases.Add(working);

                    var trim = new ProdDataModel();
                    trim.text = Resource.Trim;
                    trim.perc = Common.GetPercentage(data.Select(s => s.ElapsedTimeTrim ?? 0).Sum(), grossTime);
                    phases.Add(trim);

                    cut = new ProdDataModel();
                    cut.text = Resource.Cut;
                    cut.perc = Common.GetPercentage(data.Select(s => s.ElapsedTimeCut ?? 0).Sum(), grossTime);
                    phases.Add(cut);

                    break;
            }

            result.phases = phases.OrderByDescending(o => o.perc).ToList();

            // operators
            //var dataOperators = _pieceService.GetAggregationPieces(machine, period, enDataType.Operators);

            var dataOperators = operatorActivities;

            if (dataOperators.Count > 0)
            {
                var groupOperator = dataOperators.GroupBy(g => new { g.Operator, g.TotalTime }).OrderByDescending(x => x.Key.TotalTime).ToList();

                if (groupOperator.Count <= 3)
                {
                    result.operators = groupOperator.Select(o => new ProdDataModel()
                    {
                        text = o.Key.Operator,
                        perc = Common.GetPercentage(o.Key.TotalTime ?? 0, grossTime)
                    }).OrderByDescending(p => p.perc).ToList();
                }
                else
                {
                    var firstGroup = groupOperator.GetRange(0, 2);
                    var secondGroup = groupOperator.GetRange(2, groupOperator.Count - 2);

                    if (firstGroup.Count > 0)
                    {
                        result.operators = firstGroup.Select(o => new ProdDataModel()
                        {
                            text = o.Key.Operator,
                            perc = Common.GetPercentage(o.Key.TotalTime ?? 0, grossTime)
                        }).OrderByDescending(p => p.perc).ToList();
                    }

                    if (secondGroup.Count > 0)
                    {
                        var elpasedTimeTotal = secondGroup.Select(s => s.Key.TotalTime ?? 0).Sum();

                        result.operators.Add(new ProdDataModel()
                        {
                            text = Resource.Others,
                            perc = Common.GetPercentage(elpasedTimeTotal, grossTime)
                        });
                    }
                }
            }

            // gross time
            result.time = CommonViewService.getTimeViewModel(grossTime);

            return result;
        }



        private ChartViewModel GetHistoricalOptions(MachineInfoModel machine, PeriodModel period)
        {
            var options = new ChartViewModel();

            var granularity = Common.GetAggregationType(period.StartDate, period.EndDate);
            var startDateTrend = Common.GetStartDateTrend(period.EndDate, period.StartDate, granularity);

            var periodTrend = new PeriodModel();

            periodTrend.StartDate = startDateTrend.ToUniversalTime().Date;
            periodTrend.EndDate = period.EndDate.ToUniversalTime().Date.AddDays(1).AddTicks(-1);
            periodTrend.Aggregation = granularity;

            var data = _pieceService.GetAggregationPieces(machine, periodTrend, enDataType.Historical).OrderBy(o => o.Day).ToList();

            if (data.Count == 0)
                return null;
            var days = data.Select(s => s.Day).Distinct().ToList();

            var valori = _stateService.GetDayActivity(machine, days);
            
            options.categories = CommonViewService.GetTimeCategories(days, granularity);
            options.yTitle = $"{Resource.Efficiency} (%)";
            options.yTitle2 = $"{Resource.Productivity} (pz/h)";
            options.series = GetSeriesChartProd(valori, false);

            return options;
        }


        private ChartViewModel GetOperatorsOptions(List<EfficiencyStateMachineModel> states)
        {
            var options = new ChartViewModel();

            if (states.Count == 0)
                return null;

            //ordinamento in base all'efficienza
            states = states.OrderByDescending(s => Common.GetRatioProductivity(s.CompletedCount, s.TotalTime)).ToList();
            
            var operators = states.Select(s => s.Operator).Take(4).ToList();
            operators.Add(Resource.Others);
            options.categories = operators;
            options.yTitle = $"{Resource.Efficiency} (%)";
            options.yTitle2 = $"{Resource.Productivity} (pz/h)";
            options.series = GetSeriesChartProd(states, true);

            return options;
        }


        private List<SerieViewModel> GetSeriesChartProd(IList<EfficiencyStateMachineModel> data, bool aggregateOthers)
        {
            var series = new List<SerieViewModel>();
            var dataOther = new List<EfficiencyStateMachineModel>();
            if (aggregateOthers)
            {
                dataOther = data.Skip(4).ToList();
                data = data.Take(4).ToList();
            }

            var serieEfficiency = new SerieViewModel();
            serieEfficiency.type = (int)enSerieProd.Efficiency;
            serieEfficiency.name = enSerieProd.Efficiency.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieEfficiency.color = CommonViewService.GetColorChart(enSerieProd.Efficiency);
            serieEfficiency.data = data.Select(s => Common.GetPercentage(s.ProducingTime, s.TotalTime).RoundToInt()).ToList();
            if (dataOther.Any())
            {
                serieEfficiency.data.Add(Common.GetPercentage(dataOther.Sum(s => s.ProducingTime), dataOther.Sum(s => s.TotalTime)).RoundToInt());
            }
            series.Add(serieEfficiency);

            var serieGrossTime = new SerieViewModel();
            serieGrossTime.type = (int)enSerieProd.GrossTime;
            serieGrossTime.name = new System.Resources.ResourceManager(typeof(Resource)).GetString("NPiecesGrossTime");// enSerieProd.GrossTime.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage) + " pz/h";
            serieGrossTime.color = CommonViewService.GetColorChart(enSerieProd.GrossTime);
            serieGrossTime.data = data.Select(s => Common.GetRatioProductivity(s.CompletedCount, s.TotalTime) ?? 0).ToList();
            if (dataOther.Any())
                serieGrossTime.data.Add(Common.GetRatioProductivity(dataOther.Sum(s => s.CompletedCount), dataOther.Sum(s => s.TotalTime)) ?? 0);
            series.Add(serieGrossTime);

            var serieNetTime = new SerieViewModel();
            serieNetTime.type = (int)enSerieProd.NetTime;
            serieNetTime.name = new System.Resources.ResourceManager(typeof(Resource)).GetString("NPiecesNetTime");// enSerieProd.NetTime.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage) + " pz/h";
            serieNetTime.color = CommonViewService.GetColorChart(enSerieProd.NetTime);
            serieNetTime.data = data.Select(s => Common.GetRatioProductivity(s.CompletedCount, s.ProducingTime) ?? 0).ToList();
            if (dataOther.Any())
                serieNetTime.data.Add(Common.GetRatioProductivity(dataOther.Sum(s => s.CompletedCount), dataOther.Sum(s => s.ProducingTime)) ?? 0);
            series.Add(serieNetTime);

            return series;
        }

    }
}
