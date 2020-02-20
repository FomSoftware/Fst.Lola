using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.Repository.SQL;

namespace FomMonitoringCore.Service
{
    public class JobService : IJobService
    {
        private readonly IFomMonitoringEntities _context;
        private readonly IHistoryJobRepository _historyJobRepository;

        public JobService(IFomMonitoringEntities context, IHistoryJobRepository historyJobRepository)
        {
            _context = context;
            _historyJobRepository = historyJobRepository;
        }

        #region SP AGGREGATION

        /// <summary>
        /// Ritorna l'elenco dei job in base all'aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public List<HistoryJobModel> GetAggregationJobs(MachineInfoModel machine, PeriodModel period)
        {
            List<HistoryJobModel> result = new List<HistoryJobModel>();

            try
            {

                    List<usp_AggregationJob_Result> query = _context.usp_AggregationJob(machine.Id, period.StartDate, period.EndDate, (int)period.Aggregation).ToList();
                    result = query.Adapt<List<HistoryJobModel>>();
                
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

        #endregion


        public List<HistoryJobModel> GetAllHistoryJobs(MachineInfoModel machine, PeriodModel period)
        {
            List<HistoryJobModel> result = new List<HistoryJobModel>();

            try
            {
                    string aggType = period.Aggregation.GetDescription();

                    List<HistoryJob> query = _historyJobRepository.Get(hs => hs.MachineId == machine.Id
                                              && hs.Day >= period.StartDate && hs.Day <= period.EndDate
                                              && hs.TypeHistory == aggType, tracked: false).ToList();

                    result = query.Adapt<List<HistoryJobModel>>();
                
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


        public int? GetJobIdByJobCode(string jobCode, int machineId)
        {
            int? result = null;
            try
            {                
                if (!string.IsNullOrEmpty(jobCode))
                {
                    HistoryJob historyJob = _historyJobRepository.Get(f => f.Code == jobCode && f.MachineId == machineId, o => o.OrderByDescending(i => i.Id), tracked: false).FirstOrDefault();

                    //HistoryJob historyJob = ent.HistoryJob.OrderByDescending(o => o.Id).FirstOrDefault(f => f.Code == jobCode && f.MachineId == machineId);
                    result = historyJob != null ? historyJob.Id : (int?)null;
                }     
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), jobCode.ToString(), machineId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }
    }
}
