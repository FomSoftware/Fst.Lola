using FomMonitoringCore.SqlServer;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FomMonitoringCore.Extensions;
using FomMonitoringCore.SqlServer.Repository;

namespace FomMonitoringCore.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class StateService : IStateService
    {
        private readonly IHistoryStateRepository _historyStateRepository;
        private readonly IFomMonitoringEntities _context;



        public StateService(IHistoryStateRepository historyStateRepository, IFomMonitoringEntities context)
        {
            _historyStateRepository = historyStateRepository;
            _context = context;
        }


        #region HISTORY 

        /// <summary>
        /// Ritorna i dettagli degli stati in base a macchina e periodo
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public List<HistoryStateModel> GetAllHistoryStates(MachineInfoModel machine, PeriodModel period)
        {
            List<HistoryStateModel> result = new List<HistoryStateModel>();

            try
            {                
                string aggType = period.Aggregation.GetDescription();

                List<HistoryState> query = _historyStateRepository.Get(hs => hs.MachineId == machine.Id
                                            && hs.Day >= period.StartDate && hs.Day <= period.EndDate
                                            && hs.TypeHistory == aggType).ToList();

                result = query.Adapt<List<HistoryStateModel>>();                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }


        /// <summary>
        /// Ritorna gli aggregati degli stati in mase al tipo di aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public List<HistoryStateModel> GetAggregationStates(MachineInfoModel machine, PeriodModel period, enDataType dataType)
        {

            try
            {
                IEnumerable<HistoryState> queryResult;
                if (dataType == enDataType.Dashboard)
                {
                    queryResult = _historyStateRepository.Get(hs => hs.Day <= period.EndDate
                                                                        && hs.Day >= period.StartDate
                                                                        && hs.MachineId == machine.Id
                                                                        && hs.Shift == null
                                                                        && hs.Operator == null);
                    return BuildHistoryStateModelsOperatorDashboard(queryResult);
                }

                if (dataType == enDataType.Operators)
                {
                    queryResult = _historyStateRepository.Get(hs => hs.Day <= period.EndDate
                                                                    && hs.Day >= period.StartDate
                                                                    && hs.MachineId == machine.Id
                                                                    && hs.Shift == null
                                                                    && hs.Operator != null);
                    return BuildHistoryStateModelsOperatorDashboard(queryResult);
                }

                if (dataType == enDataType.Historical)
                {
                    queryResult = _historyStateRepository.Get(hs => hs.Day <= period.EndDate
                                                                    && hs.Day >= period.StartDate
                                                                    && hs.MachineId == machine.Id
                                                                    && hs.Shift == null
                                                                    && hs.Operator == null, null, "State");
                    if (period.Aggregation == enAggregation.Day)
                    {
                        var result = queryResult.Select(g => new HistoryStateModel
                        {
                            Id = g.Id,
                            Day = g.Day,
                            ElapsedTime = g.ElapsedTime,
                            MachineId = g.MachineId,
                            StateId = g.StateId,
                            Operator = g.Operator,
                            Shift = g.Shift,
                            OverfeedAvg = (decimal?)g.OverfeedAvg,
                            Period = g.Period,
                            TypeHistory = g.TypeHistory,
                            enState = (enState)g.StateId,
                            State = new StateModel()
                            {
                                Id = g.StateId ?? 0,
                                Code = g.State.Description,
                                Description = g.State.Description
                            }
                        }).ToList();

                        return result;
                    }

                    if (period.Aggregation == enAggregation.Week)
                    {

                        int Func(HistoryState hs) => hs.Day.Value.Year * 100 + hs.Day.Value.GetWeekNumber();

                        const string typeHistory = "w";
                        var result = BuildAggregationList(queryResult, typeHistory, Func);
                        return result.ToList();
                    }

                    if (period.Aggregation == enAggregation.Month)
                    {

                        int Func(HistoryState hs) => hs.Day.Value.Year * 100 + hs.Day.Value.Month;
                        const string typeHistory = "m";
                        var result = BuildAggregationList(queryResult, typeHistory, Func);
                        return result.ToList();
                    }

                    if (period.Aggregation == enAggregation.Quarter)
                    {
                        int Func(HistoryState hs) => hs.Day.Value.Year * 100 + hs.Day.Value.GetQuarter();

                        const string typeHistory = "q";
                        var result = BuildAggregationList(queryResult, typeHistory, Func);
                        return result.ToList();
                    }

                    if (period.Aggregation == enAggregation.Year)
                    {
                        int Func(HistoryState hs) => hs.Day.Value.Year * 100 + hs.Day.Value.Year;
                        const string typeHistory = "y";
                        var result = BuildAggregationList(queryResult, typeHistory, Func);
                        return result.ToList();
                    }
                }


            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(CultureInfo.InvariantCulture), " - ", period.EndDate.ToString(CultureInfo.InvariantCulture), " - ", period.Aggregation.ToString()),
                    dataType.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return null;

        }

        private List<HistoryStateModel> BuildAggregationList(IEnumerable<HistoryState> queryResult, string typeHistory, Func<HistoryState, int> periodFunc)
        {
            
            var groups =  queryResult.GroupBy(hs => new
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
                        ?
                        g.Sum(i => (decimal?) ((decimal) (i.OverfeedAvg ?? 0) * (i.ElapsedTime ?? 0))) /
                        g.Sum(i => i.ElapsedTime)
                        : 0
                    : null,
                Period = g.Key.Period,
                TypeHistory = typeHistory,
                enState = (enState) g.Key.StateId,
                State = new StateModel()
                {
                    Id = g.Key.StateId ?? 0,
                    Code = g.FirstOrDefault()?.State.Description,
                    Description = g.FirstOrDefault()?.State.Description
                }
            }).ToList();
            return result;
        }

        private List<HistoryStateModel> BuildHistoryStateModelsOperatorDashboard(IEnumerable<HistoryState> queryResult)
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
                    ? g.Sum(i => i.ElapsedTime) > 0 ? g.Sum(i => (decimal?)((decimal)(i.OverfeedAvg ?? 0) * (i.ElapsedTime ?? 0))) / g.Sum(i => i.ElapsedTime) : 0
                    : null,
                Period = null,
                TypeHistory = "d",
                enState = (enState) g.Key.StateId,
                State = new StateModel()
                {
                    Id = g.Key.StateId ?? 0,
                    Code = g.FirstOrDefault()?.State.Description,
                    Description = g.FirstOrDefault()?.State.Description
                }
            }).ToList();
            return result.ToList();
        }


        /// <summary>
        /// Ritorna i dettagli degli stati dato l'ID della macchina
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="typePeriod"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public List<HistoryStateModel> GetAllHistoryStatesByMachineId(int machineId, DateTime dateFrom, DateTime dateTo, enAggregation typePeriod)
        {
            List<HistoryStateModel> result = new List<HistoryStateModel>();
            try
            {

                    List<HistoryState> historyStateList = _historyStateRepository.Get(w => w.MachineId == machineId &&
                        w.Period.Value.PeriodToDate(typePeriod) >= dateFrom &&
                        w.Period.Value.PeriodToDate(typePeriod) <= dateTo).ToList();
                    result = historyStateList.Adapt<List<HistoryStateModel>>();
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), dateFrom.ToString(), dateTo.ToString(), typePeriod.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        /// <summary>
        /// Ritorna i dettagli dello stato specificato dato l'ID della macchina
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="stateId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="typePeriod"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public List<HistoryStateModel> GetHistoryStateByMachineIdStateId(int machineId, int stateId, DateTime dateFrom, DateTime dateTo, enAggregation typePeriod)
        {
            List<HistoryStateModel> result = new List<HistoryStateModel>();

            try
            {
                List<HistoryState> historyStateList = _historyStateRepository.Get(w => w.MachineId == machineId && w.StateId == stateId && w.Period != null &&
                    w.Period.Value.PeriodToDate(typePeriod) >= dateFrom && w.Period.Value.PeriodToDate(typePeriod) <= dateTo).ToList();
                result = historyStateList.Adapt<List<HistoryStateModel>>();
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), stateId.ToString(), dateFrom.ToString(), dateTo.ToString(), typePeriod.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }


        /// <summary>
        /// Ritorna la data più vecchia dei dati di una certa macchina
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="typePeriod"></param>
        /// <returns>Data</returns>
        public DateTime? GetOldestDateByMachineIdTypePeriod(int machineId, enAggregation typePeriod)
        {
            DateTime? result = null;
            try
            {
                int? period = _historyStateRepository.Get(w => w.MachineId == machineId && w.TypeHistory == typePeriod.GetDescription()).Min(m => (int?)m.Period);
                result = period == null ? null : ((int)period).PeriodToDate(typePeriod);
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), typePeriod.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public List<EfficiencyStateMachineModel> GetOperatorsActivity(MachineInfoModel machine, DateTime dateFrom, DateTime dateTo)
        {
            var tmp =  _context.Set<HistoryState>()
                 .Where(w => w.MachineId == machine.Id && w.Day >= dateFrom && w.Day <= dateTo && w.Operator != null 
                             && w.Operator != "Other" && w.StateId != (int?)enState.Offline && w.Shift == null)
                 .GroupBy(g => g.Operator).ToList();

            List<EfficiencyStateMachineModel> totTime = tmp.Select(s => new EfficiencyStateMachineModel
            {
                     TotalTime = s.Sum(x => x.ElapsedTime),
                     Operator = s.Key,
                     machineType = machine.MachineTypeId
                 }).ToList();

            if (totTime.Count > 0)
            {
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
                                    && w.Operator == item.Operator &&  w.Shift == null && w.Operator != null &&
                                    (w.StateId == (int)enState.Production || w.StateId == (int) enState.Manual)).ToList();

                    item.StatesTime = tmp3.GroupBy(g => g.StateId )
                        .ToDictionary(s => s.Key, s => s.Sum(x => x.ElapsedTime));
                }
            }

            return totTime;

        }

        public List<EfficiencyStateMachineModel> GetDayActivity(MachineInfoModel machine, List<DateTime> days)
        {
            List<EfficiencyStateMachineModel> res = new List<EfficiencyStateMachineModel>();
            foreach (var day in days)
            {
                EfficiencyStateMachineModel result = new EfficiencyStateMachineModel();
                DateTime dateTo = day.AddDays(1);
                result.StatesTime = _context.Set<HistoryState>()
                    .Where(w => w.MachineId == machine.Id && w.Day >= day && w.Day <= dateTo && w.StateId != (int?)enState.Offline
                                && w.Operator != null && w.Shift == null)
                    .GroupBy(g => g.StateId).ToDictionary(s => s.Key, s => s.Sum(x => x.ElapsedTime));

                result.TotalTime = 0;
                if (result.StatesTime != null && result.StatesTime.Count > 0)
                {
                    result.TotalTime = result.StatesTime.Values.Sum();
                }

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
