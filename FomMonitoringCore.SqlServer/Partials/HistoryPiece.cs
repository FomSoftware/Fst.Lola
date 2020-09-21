using System;
using FomMonitoringCore.Extensions;

namespace FomMonitoringCore.SqlServer
{
    public partial class HistoryPiece
    {
        public int? WeekOfYearDay {
            get {
            if(this.Day != null && this.Day.HasValue)
                return this.Day.Value.GetWeekNumber();
            return null;
            }
        }
        public int? QuarteOfYearDay
        {
            get
            {
                if (this.Day != null && this.Day.HasValue)
                    return this.Day.Value.GetQuarter();
                return null;
            }
        }
    }
}
