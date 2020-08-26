using FomMonitoringCore.SqlServer;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FomMonitoringCore.SqlServer.Repository;

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

        public void Dispose()
        {
            _context?.Dispose();
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
            var result = new List<HistoryJobModel>();

            try
            {

                    var query = _context.usp_AggregationJob(machine.Id, period.StartDate, period.EndDate, (int)period.Aggregation).ToList();
                    result = query.Adapt<List<HistoryJobModel>>();
                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        #endregion


        public List<HistoryJobModel> GetAllHistoryJobs(MachineInfoModel machine, PeriodModel period)
        {
            var result = new List<HistoryJobModel>();

            try
            {
                    var aggType = period.Aggregation.GetDescription();

                    var query = _historyJobRepository.Get(hs => hs.MachineId == machine.Id
                                              && hs.Day >= period.StartDate && hs.Day <= period.EndDate
                                              && hs.TypeHistory == aggType, tracked: false).ToList();

                    result = query.Adapt<List<HistoryJobModel>>();
                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(CultureInfo.InvariantCulture), " - ", period.EndDate.ToString(CultureInfo.InvariantCulture), " - ", period.Aggregation.ToString()));
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
                    var historyJob = _historyJobRepository.Get(f => f.Code == jobCode && f.MachineId == machineId, o => o.OrderByDescending(i => i.Id), tracked: false).FirstOrDefault();

                    //HistoryJob historyJob = ent.HistoryJob.OrderByDescending(o => o.Id).FirstOrDefault(f => f.Code == jobCode && f.MachineId == machineId);
                    result = historyJob?.Id;
                }     
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), jobCode.ToString(), machineId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }
    }
}
