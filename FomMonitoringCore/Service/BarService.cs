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
    public class BarService
    {
        #region SP AGGREGATION

        /// <summary>
        /// Ritorna i dati di barre e spezzoni in base al tipo di aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <returns>Lista dei dettagli di barre e spezzoni</returns>
        public static List<HistoryBarModel> GetAggregationBar(MachineInfoModel machine, PeriodModel period)
        {
            List<HistoryBarModel> result = new List<HistoryBarModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<usp_AggregationBar_Result> query = ent.usp_AggregationBar(machine.Id, period.StartDate, period.EndDate, (int)period.Aggregation).ToList();
                    result = query.Adapt<List<HistoryBarModel>>();
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

        #endregion

        public static int? GetBarIdByBarIdOldAndMachineId(int? barIdOld, int machineId)
        {
            int? result = null;
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                        if (barIdOld.HasValue)
                        {
                            Bar bar = ent.Bar.FirstOrDefault(f => f.IdOld == barIdOld.Value && f.MachineId == machineId);
                            result = bar == null ? (int?)null : bar.Id;
                        }
                        transaction.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), Convert.ToString(barIdOld), machineId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }
    }
}
