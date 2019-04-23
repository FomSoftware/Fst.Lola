using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.Service
{
    public class MessageService
    {
        #region SP AGGREGATION

        /// <summary>
        /// Ritorna gli allarmi n base a tipo di dati da visualizzare in base al tipo di aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <param name="dataType"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public static List<HistoryMessageModel> GetAggregationMessages(MachineInfoModel machine, PeriodModel period, enDataType dataType)
        {
            List<HistoryMessageModel> result = new List<HistoryMessageModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<usp_AggregationMessage_Result> query = ent.usp_AggregationMessage(machine.Id, period.StartDate, period.EndDate, (int)period.Aggregation, (int)dataType).ToList();
                    result = query.Adapt<List<HistoryMessageModel>>();
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

        #endregion


        /// <summary>
        /// Ritorna i dettagli degli allarmi in base a macchina e periodo
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public static List<HistoryMessageModel> GetAllHistoryMessages(MachineInfoModel machine, PeriodModel period)
        {
            List<HistoryMessageModel> result = new List<HistoryMessageModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    string aggType = period.Aggregation.GetDescription();

                    List<HistoryMessage> query = (from hs in ent.HistoryMessage
                                                where hs.MachineId == machine.Id
                                                && hs.Day >= period.StartDate && hs.Day <= period.EndDate
                                                && hs.TypeHistory == aggType
                                                select hs).ToList();

                    result = query.Adapt<List<HistoryMessageModel>>();
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

        public static List<MessageMachineModel> GetMessageDetails(MachineInfoModel machine, PeriodModel period)
        {
            List<MessageMachineModel> result = new List<MessageMachineModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<MessageMachine> query = ent.MessageMachine.Where(m => m.MachineId == machine.Id &&
                                        m.Day >= period.StartDate && m.Day <= period.EndDate).ToList();

                    result = query.Adapt<List<MessageMachine>, List<MessageMachineModel>>();
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

        public static List<MessageMachineModel> GetAllCurrentMessages(MachineInfoModel machine, PeriodModel period)
        {
            List<MessageMachineModel> result = new List<MessageMachineModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {

                    List<MessageMachine> query = (from hs in ent.MessageMachine
                                                where hs.MachineId == machine.Id
                                                && hs.Day >= period.StartDate && hs.Day <= period.EndDate
                                                select hs).ToList();

                    result = query.Adapt<List<MessageMachineModel>>();
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

    }
}
