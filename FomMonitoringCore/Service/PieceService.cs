using CommonCore.Service;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.Service
{
    public class PieceService
    {
        #region AGGREGATION

        /// <summary>
        /// Ritorna gli aggregati pieces in base al tipo di aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public static List<HistoryPieceModel> GetAggregationPieces(MachineInfoModel machine, PeriodModel period, enDataType dataType)
        {
            List<HistoryPieceModel> result = new List<HistoryPieceModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<usp_AggregationPiece_Result> query = ent.usp_AggregationPiece(machine.Id, period.StartDate, period.EndDate, (int)period.Aggregation, (int)dataType).ToList();
                    result = query.Adapt<List<HistoryPieceModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString(),
                    dataType.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        #endregion

        #region HISTORY 

        /// <summary>
        /// Ritorna i dettagli dei pezzi dato l'ID della macchina
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="typePeriod"></param>
        /// <returns>Lista dei dettagli dei pezzi</returns>
        public static List<HistoryPieceModel> GetAllHistoryPiecesByMachineId(int machineId, DateTime dateFrom, DateTime dateTo, enAggregation typePeriod)
        {
            List<HistoryPieceModel> result = new List<HistoryPieceModel>();
            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<HistoryPiece> historyPieceList = ent.HistoryPiece.Where(w => w.MachineId == machineId && w.Period != null &&
                        w.Period.Value.PeriodToDate(typePeriod) >= dateFrom && w.Period.Value.PeriodToDate(typePeriod) <= dateTo).ToList();
                    result = historyPieceList.Adapt<List<HistoryPieceModel>>();
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
        /// Ritorna i dettagli dei pezzi del sistema specificato dato l'ID della macchina
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="system"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="typePeriod"></param>
        /// <returns>Lista dei dettagli dei pezzi</returns>
        public static List<HistoryPieceModel> GetAllHistoryPiecesByMachineIdSystem(int machineId, string system, DateTime dateFrom, DateTime dateTo, enAggregation typePeriod)
        {
            List<HistoryPieceModel> result = new List<HistoryPieceModel>();
            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<HistoryPiece> historyPieceList = ent.HistoryPiece.Where(w => w.MachineId == machineId && w.Period != null &&
                        w.Period.Value.PeriodToDate(typePeriod) >= dateFrom && w.Period.Value.PeriodToDate(typePeriod) <= dateTo && w.System == system).ToList();
                    result = historyPieceList.Adapt<List<HistoryPieceModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), system, dateFrom.ToString(), dateTo.ToString(), typePeriod.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        /// <summary>
        /// Ritorna i dettagli dei pezzi del profilo specificato dato l'ID della macchina
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="profileCode"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="typePeriod"></param>
        /// <returns>Lista dei dettagli dei pezzi</returns>
        public static List<HistoryPieceModel> GetAllHistoryPiecesByMachineIdProfileCode(int machineId, string profileCode, DateTime dateFrom, DateTime dateTo, enAggregation typePeriod)
        {
            List<HistoryPieceModel> result = new List<HistoryPieceModel>();
            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    //List<HistoryPiece> historyPieceList = ent.HistoryPiece.Where(w => w.MachineId == machineId && w.Period != null &&
                    //    w.Period.Value.PeriodToDate(typePeriod) >= dateFrom && w.Period.Value.PeriodToDate(typePeriod) <= dateTo && w.ProfileCode == profileCode).ToList();
                    //result = historyPieceList.Adapt<List<HistoryPieceModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), profileCode, dateFrom.ToString(), dateTo.ToString(), typePeriod.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        #endregion

        #region ACTUAL 

        #endregion
    }
}
