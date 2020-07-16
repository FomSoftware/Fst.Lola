using FomMonitoringCore.Framework.Common;
using System;

namespace FomMonitoringCore.Framework.Model
{
    public class PeriodModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public enAggregation Aggregation { get; set; }

        public DataUpdateModel LastUpdate { get; set; }
    }

    public class DataUpdateModel
    {
        public DateTime DateTime { get; set; }

        public string Date
        {
            get
            {
                return DateTime.ToLocalTime().ToString("d");
            }
        }

        public string TimeZone { get; set; }
       
    }
}
