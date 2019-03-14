using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.Service
{
    public class SpindleService
    {
        public static List<SpindleModel> GetSpindles(MachineInfoModel machine)
        {
            List<SpindleModel> result = new List<SpindleModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<Spindle> query = ent.Spindle.Where(w => w.MachineId == machine.Id).ToList();
                    result = query.Adapt<List<SpindleModel>>();
                    foreach (SpindleModel spindle in result)
                    {
                        spindle.ChangeCount = result.Count(c => c.Code == spindle.Code) - 1;
                    }
                    result = result.GroupBy(g => g.Code).Select(s => s.OrderByDescending(o => o.InstallDate).First()).ToList();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machine.Id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }
    }
}
