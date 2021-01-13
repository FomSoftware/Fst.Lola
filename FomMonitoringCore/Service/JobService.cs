using FomMonitoringCore.SqlServer;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using Autofac.Features.ResolveAnything;
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
                result = _context.Set<HistoryJob>().Where(hb =>
                        hb.MachineId == machine.Id && hb.Day >= period.StartDate && hb.Day <= period.EndDate).ToList()
                    .GroupBy(g => new {g.MachineId, g.Code, Day = g.Day.Value.Date})
                    .Select(n => new HistoryJobModel
                    {
                        Id = n.Max(i => i.Id),
                        Code = n.Key.Code,
                        Day = n.Key.Day,
                        MachineId = n.Key.MachineId,
                        TotalPieces = n.OrderByDescending(i => i.Id).FirstOrDefault()?.TotalPieces,
                        ElapsedTime = n.Max(i => i.ElapsedTime),
                        PiecesProduced = GetProdPieces(n.Max(i => i.Id), n.Key.Day),
                        Period = null
                    }).ToList();
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

        private int? GetProdPieces(int? jobId, DateTime day)
        {
            return _context.Set<Piece>().Count(p => DbFunctions.TruncateTime(p.Day.Value) == day.Date
                                                    && p.JobId == jobId);
        }

        #endregion





        public int? GetJobIdByJobCode(string jobCode, int machineId, DateTime? endDateTime)
        {
            int? result = null;
            try
            {                
                if (!string.IsNullOrEmpty(jobCode))
                {
                    var date = endDateTime?.Date;
                    var historyJob = _historyJobRepository.Get(f => f.Code == jobCode && f.MachineId == machineId && DbFunctions.TruncateTime(f.Day) == date, o => o.OrderByDescending(i => i.Id), tracked: false).FirstOrDefault();

                    result = historyJob?.Id;
                }     
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), jobCode, machineId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public void CleanHistoryJobs()
        {
            //elimino solo quelli degli ultimi 10 giorni che non hanno piece 
            //perchè altrimenti andrebbe sempre su tutta la tabella e sarebbe sempre più lenta
            //quando si avvia il task si fa una prima pulizia generale.
            var start = DateTime.UtcNow.AddDays(int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("CleanInterval")));
            var toDelete = _context.Set<HistoryJob>()
                .Where(j => DbFunctions.TruncateTime(j.Day) >= start.Date && !j.Piece.Any()).ToList();

            if (!toDelete.Any()) 
                return;

            _context.Set<HistoryJob>().RemoveRange(toDelete);
            _context.SaveChanges();

        }
    }
}
