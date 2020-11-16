using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FomMonitoringCore.Extensions;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.SqlServer;
using FomMonitoringCore.SqlServer.Repository;

namespace FomMonitoringCore.Service
{
    /// <summary>
    /// </summary>
    public class StateService : IStateService
    {
        private readonly IFomMonitoringEntities _context;
        private readonly IHistoryStateRepository _historyStateRepository;


        public StateService(IHistoryStateRepository historyStateRepository, IFomMonitoringEntities context)
        {
            _historyStateRepository = historyStateRepository;
            _context = context;
        }


        #region HISTORY

        /// <summary>
        ///     Ritorna gli aggregati degli stati in mase al tipo di aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <param name="dataType"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public List<HistoryStateModel> GetAggregationStates(MachineInfoModel machine, PeriodModel period,
            enDataType dataType)
        {
            try
            {
                IEnumerable<HistoryState> queryResult;
                switch (dataType)
                {
                    case enDataType.Dashboard:
                        queryResult = _historyStateRepository.Get(hs => hs.Day <= period.EndDate
                                                                        && hs.Day >= period.StartDate
                                                                        && hs.MachineId == machine.Id
                                                                        && hs.Shift == null
                                                                        && hs.Operator == null);
                        return BuildHistoryStateModelsOperatorDashboard(queryResult);
                    case enDataType.Operators:
                        queryResult = _historyStateRepository.Get(hs => hs.Day <= period.EndDate
                                                                        && hs.Day >= period.StartDate
                                                                        && hs.MachineId == machine.Id
                                                                        && hs.Shift == null
                                                                        && hs.Operator != null);
                        return BuildHistoryStateModelsOperatorDashboard(queryResult);
                    case enDataType.Historical:
                    {
                        queryResult = _historyStateRepository.Get(hs => hs.Day <= period.EndDate
                                                                        && hs.Day >= period.StartDate
                                                                        && hs.MachineId == machine.Id
                                                                        && hs.Shift == null
                                                                        && hs.Operator == null, null, "State");
                        switch (period.Aggregation)
                        {
                            case enAggregation.Day:
                            {
                                int? Func(HistoryState hs)
                                {
                                    return hs.Period;
                                }

                                const string typeHistory = "d";
                                var result = BuildAggregationList(queryResult, typeHistory, Func);
                                return result.ToList();
                            }
                            case enAggregation.Week:
                            {
                                int? Func(HistoryState hs)
                                {
                                    if (hs.Day != null)
                                        return hs.Day.Value.Year * 100 + hs.Day.Value.GetWeekNumber();
                                    return null;
                                }

                                const string typeHistory = "w";
                                var result = BuildAggregationList(queryResult, typeHistory, Func);
                                return result.ToList();
                            }
                            case enAggregation.Month:
                            {
                                int? Func(HistoryState hs)
                                {
                                    if (hs.Day != null)
                                        return hs.Day.Value.Year * 100 + hs.Day.Value.Month;
                                    return null;
                                }

                                const string typeHistory = "m";
                                var result = BuildAggregationList(queryResult, typeHistory, Func);
                                return result.ToList();
                            }
                            case enAggregation.Quarter:
                            {
                                int? Func(HistoryState hs)
                                {
                                    if (hs.Day != null)
                                        return hs.Day.Value.Year * 100 + hs.Day.Value.GetQuarter();
                                    return null;
                                }

                                const string typeHistory = "q";
                                var result = BuildAggregationList(queryResult, typeHistory, Func);
                                return result.ToList();
                            }
                            case enAggregation.Year:
                            {
                                int? Func(HistoryState hs)
                                {
                                    if (hs.Day != null)
                                        return hs.Day.Value.Year * 100 + hs.Day.Value.Year;
                                    return null;
                                }

                                const string typeHistory = "y";
                                var result = BuildAggregationList(queryResult, typeHistory, Func);
                                return result.ToList();
                            }
                        }

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(CultureInfo.InvariantCulture), " - ",
                        period.EndDate.ToString(CultureInfo.InvariantCulture), " - ", period.Aggregation.ToString()),
                    dataType.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return null;
        }

        private IEnumerable<HistoryStateModel> BuildAggregationList(IEnumerable<HistoryState> queryResult,
            string typeHistory, Func<HistoryState, int?> periodFunc)
        {
            var groups = queryResult.GroupBy(hs => new
            {
                hs.MachineId,
                hs.StateId,
                Period = periodFunc.Invoke(hs)
            });

            var result = groups.Select(g => new HistoryStateModel
            {
                Id = g.Max(n => n.Id),
                Day = g.Max(n => n.Day),
                ElapsedTime = g.Sum(n => n.ElapsedTime),
                MachineId = g.Key.MachineId,
                StateId = g.Key.StateId,
                Operator = null,
                Shift = null,
                OverfeedAvg = g.Key.StateId == 1
                    ? g.Sum(i => i.ElapsedTime) > 0
                        ? g.Sum(i => (decimal?) ((decimal) (i.OverfeedAvg ?? 0) * (i.ElapsedTime ?? 0))) /
                          g.Sum(i => i.ElapsedTime)
                        : 0
                    : null,
                Period = g.Key.Period,
                TypeHistory = typeHistory,
                enState = (enState) g.Key.StateId,
                State = new StateModel
                {
                    Id = g.Key.StateId ?? 0,
                    Code = g.FirstOrDefault()?.State.Description,
                    Description = g.FirstOrDefault()?.State.Description
                }
            }).ToList();
            return result;
        }

        private List<HistoryStateModel> BuildHistoryStateModelsOperatorDashboard(
            IEnumerable<HistoryState> queryResult)
        {
            var groups = queryResult.GroupBy(hs => new {hs.MachineId, hs.Operator, hs.Shift, hs.StateId});
            var result = groups.Select(g => new HistoryStateModel
            {
                Id = g.Max(n => n.Id),
                Day = g.Max(n => n.Day),
                ElapsedTime = g.Sum(n => n.ElapsedTime),
                MachineId = g.Key.MachineId,
                StateId = g.Key.StateId,
                Operator = g.Key.Operator,
                Shift = g.Key.Shift,
                OverfeedAvg = g.Key.StateId == 1
                    ? g.Sum(i => i.ElapsedTime) > 0
                        ?
                        g.Sum(i => (decimal?) ((decimal) (i.OverfeedAvg ?? 0) * (i.ElapsedTime ?? 0))) /
                        g.Sum(i => i.ElapsedTime)
                        : 0
                    : null,
                Period = null,
                TypeHistory = "d",
                enState = (enState) g.Key.StateId,
                State = new StateModel
                {
                    Id = g.Key.StateId ?? 0,
                    Code = g.FirstOrDefault()?.State.Description,
                    Description = g.FirstOrDefault()?.State.Description
                }
            }).ToList();
            return result.ToList();
        }


        public List<EfficiencyStateMachineModel> GetOperatorsActivity(MachineInfoModel machine, DateTime dateFrom,
            DateTime dateTo)
        {
            var tmp = _context.Set<HistoryState>()
                .Where(w => w.MachineId == machine.Id && w.Day >= dateFrom && w.Day <= dateTo && w.Operator != null
                            && w.Operator != "Other" && w.StateId != (int?) enState.Offline && w.Shift == null)
                .GroupBy(g => g.Operator).ToList();

            var totTime = tmp.Select(s => new EfficiencyStateMachineModel
            {
                TotalTime = s.Sum(x => x.ElapsedTime),
                Operator = s.Key,
                machineType = machine.MachineTypeId
            }).ToList();

            if (totTime.Count > 0)
                foreach (var item in totTime)
                {
                    //tot. pezzi prodotti
                    var tmp2 = _context.Set<HistoryPiece>()
                        .Where(w => w.MachineId == machine.Id && w.Day >= dateFrom && w.Day <= dateTo &&
                                    w.Operator == item.Operator && w.Shift == null && w.Operator != null).ToList();

                    item.CompletedCount = tmp2.GroupBy(g => g.Operator)
                        .Select(s => s.Sum(x => x.CompletedCount)).FirstOrDefault();

                    //tempo netto
                    var tmp3 = _context.Set<HistoryState>()
                        .Where(w => w.MachineId == machine.Id && w.Day >= dateFrom && w.Day <= dateTo
                                    && w.Operator == item.Operator && w.Shift == null && w.Operator != null &&
                                    (w.StateId == (int) enState.Production || w.StateId == (int) enState.Manual))
                        .ToList();

                    item.StatesTime = tmp3.GroupBy(g => g.StateId)
                        .ToDictionary(s => s.Key, s => s.Sum(x => x.ElapsedTime));
                }

            return totTime;
        }

        public List<EfficiencyStateMachineModel> GetDayActivity(MachineInfoModel machine, List<DateTime> days)
        {
            var res = new List<EfficiencyStateMachineModel>();
            foreach (var day in days)
            {
                var result = new EfficiencyStateMachineModel();
                var dateTo = day.AddDays(1);
                result.StatesTime = _context.Set<HistoryState>()
                    .Where(w => w.MachineId == machine.Id && w.Day >= day && w.Day <= dateTo &&
                                w.StateId != (int?) enState.Offline
                                && w.Operator != null && w.Shift == null)
                    .GroupBy(g => g.StateId).ToDictionary(s => s.Key, s => s.Sum(x => x.ElapsedTime));

                result.TotalTime = 0;
                if (result.StatesTime != null && result.StatesTime.Count > 0)
                    result.TotalTime = result.StatesTime.Values.Sum();

                result.machineType = machine.MachineTypeId;
                result.Day = day;

                //tot. pezzi prodotti
                var tmp2 = _context.Set<HistoryPiece>()
                    .Where(w => w.MachineId == machine.Id && w.Day >= day && w.Day <= dateTo
                                && w.Operator != null && w.Shift == null).ToList();
                result.CompletedCount = tmp2.Sum(x => x.CompletedCount);
                res.Add(result);
            }

            return res;
        }

        public int? GetOrderViewState(string stateCode)
        {
            return _context.Set<State>().FirstOrDefault(s => s.Description == stateCode)?.OrderView;
        }

        #endregion
    }
}