using System;
using FomMonitoringCore.Extensions;

namespace FomMonitoringCore.SqlServer
{
    public partial class HistoryPiece
    {
        public int? WeekOfYearDay => Day.GetWeekNumber();

        public int? QuarteOfYearDay => Day.GetQuarter();
    }
}
