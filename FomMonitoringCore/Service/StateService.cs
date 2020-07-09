using FomMonitoringCore.SqlServer;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
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
            List<HistoryStateModel> result = new List<HistoryStateModel>();

            try
            {
                List<usp_AggregationState_Result> query = _context.usp_AggregationState(machine.Id, period.StartDate, period.EndDate, (int)period.Aggregation, (int)dataType).ToList();
                result = query.Adapt<List<HistoryStateModel>>();
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString()),
                    dataType.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
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
                             && w.StateId != (int?)enState.Offline && w.Shift == null)
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
                                    (w.StateId == (int)enState.Automatico || w.StateId == (int) enState.Manual)).ToList();

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

        #endregion

    }
}
