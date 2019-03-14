using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class StateService
    {
        #region HISTORY 

        /// <summary>
        /// Ritorna i dettagli degli stati in base a macchina e periodo
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public static List<HistoryStateModel> GetAllHistoryStates(MachineInfoModel machine, PeriodModel period)
        {
            List<HistoryStateModel> result = new List<HistoryStateModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    string aggType = period.Aggregation.GetDescription();

                    List<HistoryState> query = (from hs in ent.HistoryState
                                                where hs.MachineId == machine.Id
                                                && hs.Day >= period.StartDate && hs.Day <= period.EndDate
                                                && hs.TypeHistory == aggType
                                                select hs).ToList();

                    result = query.Adapt<List<HistoryStateModel>>();
                }
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
        public static List<HistoryStateModel> GetAggregationStates(MachineInfoModel machine, PeriodModel period, enDataType dataType)
        {
            List<HistoryStateModel> result = new List<HistoryStateModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<usp_AggregationState_Result> query = ent.usp_AggregationState(machine.Id, period.StartDate, period.EndDate, (int)period.Aggregation, (int)dataType).ToList();
                    result = query.Adapt<List<HistoryStateModel>>();
                }
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
        public static List<HistoryStateModel> GetAllHistoryStatesByMachineId(int machineId, DateTime dateFrom, DateTime dateTo, enAggregation typePeriod)
        {
            List<HistoryStateModel> result = new List<HistoryStateModel>();
            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<HistoryState> historyStateList = ent.HistoryState.Where(w => w.MachineId == machineId &&
                        w.Period.Value.PeriodToDate(typePeriod) >= dateFrom &&
                        w.Period.Value.PeriodToDate(typePeriod) <= dateTo).ToList();
                    result = historyStateList.Adapt<List<HistoryStateModel>>();
                }
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
        public static List<HistoryStateModel> GetHistoryStateByMachineIdStateId(int machineId, int stateId, DateTime dateFrom, DateTime dateTo, enAggregation typePeriod)
        {
            List<HistoryStateModel> result = new List<HistoryStateModel>();
            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<HistoryState> historyStateList = ent.HistoryState.Where(w => w.MachineId == machineId && w.StateId == stateId && w.Period != null &&
                        w.Period.Value.PeriodToDate(typePeriod) >= dateFrom && w.Period.Value.PeriodToDate(typePeriod) <= dateTo).ToList();
                    result = historyStateList.Adapt<List<HistoryStateModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), stateId.ToString(), dateFrom.ToString(), dateTo.ToString(), typePeriod.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        /// <summary>
        /// Ritorna i dettagli degli stati di un operatore specificato dato l'ID della macchina
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="operatorName"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="typePeriod"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public static List<HistoryStateModel> GetHistoryStateByMachineIdOperator(int machineId, string operatorName, DateTime dateFrom, DateTime dateTo, enAggregation typePeriod)
        {
            List<HistoryStateModel> result = new List<HistoryStateModel>();
            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    //List<HistoryState> historyStateList = ent.HistoryState.Where(w => w.MachineId == machineId && w.Operator == operatorName && w.Period != null &&
                    //    w.Period.Value.PeriodToDate(typePeriod) >= dateFrom && w.Period.Value.PeriodToDate(typePeriod) <= dateTo).ToList();
                    //result = historyStateList.Adapt<List<HistoryStateModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), operatorName, dateFrom.ToString(), dateTo.ToString(), typePeriod.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        /// <summary>
        /// Ritorna i dettagli dello stato specificato di un operatore specificato dato l'ID della macchina
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="stateId"></param>
        /// <param name="operatorName"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="typePeriod"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public static List<HistoryStateModel> GetHistoryStateByMachineIdStateIdOperator(int machineId, int stateId, string operatorName, DateTime dateFrom, DateTime dateTo, enAggregation typePeriod)
        {
            List<HistoryStateModel> result = new List<HistoryStateModel>();
            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    //List<HistoryState> historyStateList = ent.HistoryState.Where(w => w.MachineId == machineId && w.Operator == operatorName && w.Period != null &&
                    //    w.Period.Value.PeriodToDate(typePeriod) >= dateFrom && w.Period.Value.PeriodToDate(typePeriod) <= dateTo).ToList();
                    //result = historyStateList.Adapt<List<HistoryStateModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), stateId.ToString(), operatorName, dateFrom.ToString(), dateTo.ToString(), typePeriod.ToString());
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
        public static DateTime? GetOldestDateByMachineIdTypePeriod(int machineId, enAggregation typePeriod)
        {
            DateTime? result = null;
            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    int? period = ent.HistoryState.Where(w => w.MachineId == machineId && w.TypeHistory == typePeriod.GetDescription()).Min(m => (int?)m.Period);
                    result = period == null ? null : ((int)period).PeriodToDate(typePeriod);
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), typePeriod.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        internal static object GetShiftByStartTime(object v, DateTime? startTime)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ACTUAL

        #endregion
    }
}
