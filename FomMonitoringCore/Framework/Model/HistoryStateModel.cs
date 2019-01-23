using FomMonitoringCore.Framework.Common;
using System;

namespace FomMonitoringCore.Framework.Model
{
    public class HistoryStateModel
    {
        public int Id { get; set; }
        public DateTime? Day { get; set; }
        public long? ElapsedTime { get; set; }
        public int? MachineId { get; set; }
        public string Operator { get; set; }
        public decimal? OverfeedAvg { get; set; }
        public int? Period { get; set; }
        public int? Shift { get; set; }
        public int? StateId { get; set; }
        public enState enState { get; set; }
        public StateModel State { get; set; }
        public string TypeHistory { get; set; }
    }
}
