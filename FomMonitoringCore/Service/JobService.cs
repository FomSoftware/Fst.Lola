using CommonCore.Service;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace FomMonitoringCore.Service
{
    public class JobService
    {
        #region SP AGGREGATION

        /// <summary>
        /// Ritorna l'elenco dei job in base all'aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public static List<HistoryJobModel> GetAggregationJobs(MachineInfoModel machine, PeriodModel period)
        {
            List<HistoryJobModel> result = new List<HistoryJobModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<usp_AggregationJob_Result> query = ent.usp_AggregationJob(machine.Id, period.StartDate, period.EndDate, (int)period.Aggregation).ToList();
                    result = query.Adapt<List<HistoryJobModel>>();
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


        public static List<HistoryJobModel> GetAllHistoryJobs(MachineInfoModel machine, PeriodModel period)
        {
            List<HistoryJobModel> result = new List<HistoryJobModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    string aggType = period.Aggregation.GetDescription();

                    List<HistoryJob> query = (from hs in ent.HistoryJob
                                              where hs.MachineId == machine.Id
                                              && hs.Day >= period.StartDate && hs.Day <= period.EndDate
                                              && hs.TypeHistory == aggType
                                              select hs).ToList();

                    result = query.Adapt<List<HistoryJobModel>>();
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


        public static int? GetJobIdByJobCode(string jobCode, int machineId)
        {
            int? result = null;
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                        if (!string.IsNullOrEmpty(jobCode))
                        {
                            HistoryJob historyJob = ent.HistoryJob.OrderByDescending(o => o.Id).FirstOrDefault(f => f.Code == jobCode && f.MachineId == machineId);
                            result = historyJob != null ? historyJob.Id : (int?)null;
                        }
                        transaction.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format("{0}(jobCode = '{1}')", Common.GetStringLog(), jobCode.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }
    }
}
