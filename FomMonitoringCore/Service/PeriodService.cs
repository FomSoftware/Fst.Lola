using CommonCore.Service;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using System;

namespace FomMonitoringCore.Service
{
    public class PeriodService
    {
        public static PeriodModel GetPeriodModel(DateTime start, DateTime end, enAggregation type)
        {
            PeriodModel result = new PeriodModel();

            try
            {
                result.StartDate = start;
                result.EndDate = end;
                result.Aggregation = type;

                DataUpdateModel dataUpdate = new DataUpdateModel();
                dataUpdate.DateTime = end;
                result.LastUpdate = dataUpdate;
            }
            catch (Exception ex)
            {
                LogService.WriteLog(ex.GetStringLog(), LogService.TypeLevel.Error, ex);
            }

            return result;
        }
    }
}
