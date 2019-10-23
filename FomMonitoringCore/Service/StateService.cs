using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class StateService : IStateService
    {
        private IHistoryStateRepository _historyStateRepository;
        private IFomMonitoringEntities _context;

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
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    string aggType = period.Aggregation.GetDescription();

                    List<HistoryState> query = _historyStateRepository.Get(hs => hs.MachineId == machine.Id
                                                && hs.Day >= period.StartDate && hs.Day <= period.EndDate
                                                && hs.TypeHistory == aggType).ToList();

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

        #endregion

    }
}
