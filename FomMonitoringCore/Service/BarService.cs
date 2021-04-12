using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringCore.Service
{
    public class BarService : IBarService
    {
        private readonly IFomMonitoringEntities _context;
        public BarService(IFomMonitoringEntities context)
        {
            _context = context;
        }

        #region Entity AGGREGATION

        /// <summary>
        /// Ritorna i dati di barre e spezzoni in base al tipo di aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <returns>Lista dei dettagli di barre e spezzoni</returns>
        public List<HistoryBarModel> GetAggregationBar(MachineInfoModel machine, PeriodModel period)
        {
            var result = new List<HistoryBarModel>();

            try
            {              
                    result = _context.Set<HistoryBar>().Where(hb => hb.MachineId == machine.Id && hb.Day >= period.StartDate && hb.Day <= period.EndDate).GroupBy(g => g.MachineId).Select(n => new HistoryBarModel
                    {
                        Count = n.Count(),
                        Id = n.Max(i => i.Id),
                        Length = n.Sum(i => i.Length),
                        Day = n.Max(i => i.Day),
                        MachineId = n.Key,
                        OffcutCount = n.Sum(i => i.OffcutCount),
                        OffcutLength = n.Sum(i => i.OffcutLength),
                        Period = null,
                        System = null,
                        TypeHistory = "d"
                    }).ToList();                
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

       /* public int? GetBarIdByBarIdOldAndMachineId(int? barIdOld, int machineId, DateTime start, DateTime end, string JCode)
        {
            int? result = null;
            try
            {                                
                    if (barIdOld.HasValue)
                    {
                        var bar = _context.Set<Bar>().FirstOrDefault(f => f.IdOld == barIdOld.Value 
                                                                          && f.MachineId == machineId
                                                                          && f.StartTime <= end 
                                                                          && f.StartTime >= start
                                                                          && f.JobCode == JCode);
                        result = bar?.Id;
                    }                                              
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), Convert.ToString(barIdOld), machineId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }*/

        public int? GetBarId(int? barId, int machineId, List<Bar> listaBarre, string JCode)
        {
            int? result = null;
            try
            {
                Bar barra = listaBarre.FirstOrDefault(b => b.IdOld == barId && b.StartTime > DateTime.MinValue && b.JobCode == JCode);
                if (barra != null)
                {
                    var bar = _context.Set<Bar>().FirstOrDefault(f => f.IdOld == barId
                                                                      && f.MachineId == machineId
                                                                      && f.Index == barra.Index
                                                                      && f.JobCode == JCode);
                    result = bar?.Id;
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), Convert.ToString(barId), machineId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;


        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
