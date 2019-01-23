﻿using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringResources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class ProductivityViewService
    {
        public static ProductivityViewModel GetProductivity(ContextModel context)
        {
            ProductivityViewModel result = new ProductivityViewModel();

            result.vm_productivity = GetVueModel(context.ActualMachine, context.ActualPeriod);
            result.opt_historical = GetHistoricalOptions(context.ActualMachine, context.ActualPeriod);
            result.opt_operators = GetOperatorsOptions(context.ActualMachine, context.ActualPeriod);
            result.opt_shifts = GetShiftsOptions(context.ActualMachine, context.ActualPeriod);

            return result;
        }


        private static ProductivityVueModel GetVueModel(MachineInfoModel machine, PeriodModel period)
        {
            ProductivityVueModel result = new ProductivityVueModel();

            List<HistoryPieceModel> data = PieceService.GetAggregationPieces(machine, period, enDataType.Dashboard);

            if (data.Count == 0)
                return result;

            // tempo lordo
            long? grossTime = data.Select(s => s.ElapsedTime).Sum();
            int doneCount = data.Select(s => s.CompletedCount).Sum() ?? 0;
            int redoneCount = data.Select(s => s.RedoneCount).Sum() ?? 0;

            // kpi pezzi all'ora
            int? ratio = Common.GetRatioProductivity(doneCount, grossTime);
            result.kpi = CommonViewService.getKpiViewModel(ratio, machine.PiecesProductivityGreenThreshold, machine.PiecesProductivityYellowThreshold);

            // pieces
            PieceViewModel piece = new PieceViewModel();
            piece.total = doneCount + redoneCount;

            ProdDataModel done = new ProdDataModel();
            done.perc = Common.GetPercentage(doneCount, piece.total);
            done.number = doneCount;
            piece.done = done;

            ProdDataModel redone = new ProdDataModel();
            redone.perc = Common.GetPercentage(redoneCount, piece.total);
            redone.number = redoneCount;
            piece.redone = redone;

            result.piece = piece;

            // materials
            List<HistoryBarModel> dataBars = BarService.GetAggregationBar(machine, period);

            if (dataBars.Count > 0)
            {
                int barLength = dataBars.Select(s => s.Length ?? 0).Sum().RoundToInt();
                int cutoffLength = dataBars.Select(s => s.OffcutCount ?? 0).Sum();
                int totalLength = barLength + cutoffLength;

                MaterialViewModel material = new MaterialViewModel();
                material.total = ((double)totalLength / 1000).RoundToInt();

                ProdDataModel bar = new ProdDataModel();
                bar.perc = Common.GetPercentage(barLength, totalLength);
                bar.number = ((double)barLength / 1000).RoundToInt();
                material.bar = bar;

                ProdDataModel cutoff = new ProdDataModel();
                cutoff.perc = Common.GetPercentage(cutoffLength, totalLength);
                cutoff.number = ((double)cutoffLength / 1000).RoundToInt();
                material.cutoff = cutoff;

                result.material = material;
            }


            // phases
            List<ProdDataModel> phases = new List<ProdDataModel>();

            ProdDataModel working = new ProdDataModel();
            working.text = Resource.Working;
            working.perc = Common.GetPercentage(data.Select(s => s.ElapsedTimeWorking ?? 0).Sum(), grossTime);
            phases.Add(working);

            ProdDataModel trim = new ProdDataModel();
            trim.text = Resource.Trim;
            trim.perc = Common.GetPercentage(data.Select(s => s.ElapsedTimeTrim ?? 0).Sum(), grossTime);
            phases.Add(trim);

            ProdDataModel cut = new ProdDataModel();
            cut.text = Resource.Cut;
            cut.perc = Common.GetPercentage(data.Select(s => s.ElapsedTimeCut ?? 0).Sum(), grossTime);
            phases.Add(cut);

            long timeAnuba = data.Select(s => s.ElapsedTimeAnuba ?? 0).Sum();

            if (timeAnuba > 0)
            {
                ProdDataModel anuba = new ProdDataModel();
                anuba.text = "Anuba";
                anuba.perc = Common.GetPercentage(timeAnuba, grossTime);
                phases.Add(anuba);
            }

            result.phases = phases.OrderByDescending(o => o.perc).ToList();

            // operators
            List<HistoryPieceModel> dataOperators = PieceService.GetAggregationPieces(machine, period, enDataType.Operators);

            if (dataOperators.Count > 0)
            {
                var groupOperator = dataOperators.GroupBy(g => new { g.Operator, g.ElapsedTime }).ToList();

                if (groupOperator.Count <= 3)
                {
                    result.operators = groupOperator.Select(o => new ProdDataModel()
                    {
                        text = o.Key.Operator,
                        perc = Common.GetPercentage(o.Key.ElapsedTime ?? 0, grossTime)
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
                            perc = Common.GetPercentage(o.Key.ElapsedTime ?? 0, grossTime)
                        }).ToList();
                    }

                    if (secondGroup.Count > 0)
                    {
                        var elpasedTimeTotal = secondGroup.Select(s => s.Key.ElapsedTime ?? 0).Sum();

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


        private static ChartViewModel GetHistoricalOptions(MachineInfoModel machine, PeriodModel period)
        {
            ChartViewModel options = new ChartViewModel();

            enAggregation granularity = Common.GetAggregationType(period.StartDate, period.EndDate);
            DateTime startDateTrend = Common.GetStartDateTrend(period.EndDate, granularity);

            PeriodModel periodTrend = new PeriodModel();
            periodTrend.StartDate = startDateTrend;
            periodTrend.EndDate = period.EndDate;
            periodTrend.Aggregation = granularity;

            List<HistoryPieceModel> data = PieceService.GetAggregationPieces(machine, periodTrend, enDataType.Historical).OrderBy(o => o.Day).ToList();

            if (data.Count == 0)
                return null;

            List<DateTime> days = data.Select(s => (DateTime)s.Day).Distinct().ToList();
            options.categories = CommonViewService.GetTimeCategories(days, granularity);
            options.yTitle = string.Format("{0} (%)", Resource.Efficiency);
            options.yTitle2 = string.Format("{0} (pz/h)", Resource.Productivity);
            options.series = GetSeriesChart(data);

            return options;
        }


        private static ChartViewModel GetOperatorsOptions(MachineInfoModel machine, PeriodModel period)
        {
            ChartViewModel options = new ChartViewModel();

            List<HistoryPieceModel> data = PieceService.GetAggregationPieces(machine, period, enDataType.Operators);

            if (data.Count == 0)
                return null;

            List<string> operators = data.Select(s => s.Operator).Distinct().ToList();
            options.categories = operators;
            options.yTitle = string.Format("{0} (%)", Resource.Efficiency);
            options.yTitle2 = string.Format("{0} (pz/h)", Resource.Productivity);
            options.series = GetSeriesChart(data);

            return options;
        }


        private static ChartViewModel GetShiftsOptions(MachineInfoModel machine, PeriodModel period)
        {
            ChartViewModel options = new ChartViewModel();

            List<HistoryPieceModel> data = PieceService.GetAggregationPieces(machine, period, enDataType.Shifts);

            if (data.Count == 0)
                return null;

            List<int> shifts = data.Select(s => (int)s.Shift).Distinct().ToList();
            options.categories = shifts.Select(s => string.Format("{0}° {1}", s.ToString(), Resource.Shift)).ToList();
            options.yTitle = string.Format("{0} (%)", Resource.Efficiency);
            options.yTitle2 = string.Format("{0} (pz/h)", Resource.Productivity);
            options.series = GetSeriesChart(data);

            return options;
        }


        private static List<SerieViewModel> GetSeriesChart(List<HistoryPieceModel> data)
        {
            List<SerieViewModel> series = new List<SerieViewModel>();

            SerieViewModel serieEfficiency = new SerieViewModel();
            serieEfficiency.type = (int)enSerieProd.Efficiency;
            serieEfficiency.name = enSerieProd.Efficiency.ToLocalizedString();
            serieEfficiency.color = CommonViewService.GetColorChart(enSerieProd.Efficiency);
            serieEfficiency.data = data.Select(s => Common.GetPercentage(s.ElapsedTimeProducing, s.ElapsedTime).RoundToInt()).ToList();
            series.Add(serieEfficiency);

            SerieViewModel serieGrossTime = new SerieViewModel();
            serieGrossTime.type = (int)enSerieProd.GrossTime;
            serieGrossTime.name = enSerieProd.GrossTime.ToLocalizedString();
            serieGrossTime.color = CommonViewService.GetColorChart(enSerieProd.GrossTime);
            serieGrossTime.data = data.Select(s => Common.GetRatioProductivity(s.CompletedCount, s.ElapsedTime) ?? 0).ToList();
            series.Add(serieGrossTime);

            SerieViewModel serieNetTime = new SerieViewModel();
            serieNetTime.type = (int)enSerieProd.NetTime;
            serieNetTime.name = enSerieProd.NetTime.ToLocalizedString();
            serieNetTime.color = CommonViewService.GetColorChart(enSerieProd.NetTime);
            serieNetTime.data = data.Select(s => Common.GetRatioProductivity(s.CompletedCount, s.ElapsedTimeProducing) ?? 0).ToList();
            series.Add(serieNetTime);

            return series;
        }

    }
}
