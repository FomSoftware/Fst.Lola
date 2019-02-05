using CommonCore.Service;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.Service
{
    public class AlarmService
    {
        #region SP AGGREGATION

        /// <summary>
        /// Ritorna gli allarmi n base a tipo di dati da visualizzare in base al tipo di aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <param name="dataType"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public static List<HistoryAlarmModel> GetAggregationAlarms(MachineInfoModel machine, PeriodModel period, enDataType dataType)
        {
            List<HistoryAlarmModel> result = new List<HistoryAlarmModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<usp_AggregationAlarm_Result> query = ent.usp_AggregationAlarm(machine.Id, period.StartDate, period.EndDate, (int)period.Aggregation, (int)dataType).ToList();
                    result = query.Adapt<List<HistoryAlarmModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format("{0}(machineId = '{1}', startDate = '{2}', endDate = '{3}', typePeriod = '{4}')",
                    Common.GetStringLog(), machine.Id.ToString(),
                    period.StartDate.ToString(),
                    period.EndDate.ToString(), period.Aggregation.ToString());
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
        public static List<HistoryAlarmModel> GetAllHistoryAlarms(MachineInfoModel machine, PeriodModel period)
        {
            List<HistoryAlarmModel> result = new List<HistoryAlarmModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    string aggType = period.Aggregation.GetDescription();

                    List<HistoryAlarm> query = (from hs in ent.HistoryAlarm
                                                where hs.MachineId == machine.Id
                                                && hs.Day >= period.StartDate && hs.Day <= period.EndDate
                                                && hs.TypeHistory == aggType
                                                select hs).ToList();

                    result = query.Adapt<List<HistoryAlarmModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format("{0}(machineId = '{1}', startDate = '{2}', endDate = '{3}', typePeriod = '{4}')",
                    Common.GetStringLog(), machine.Id.ToString(),
                    period.StartDate.ToString(),
                    period.EndDate.ToString(), period.Aggregation.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }
    }
}
