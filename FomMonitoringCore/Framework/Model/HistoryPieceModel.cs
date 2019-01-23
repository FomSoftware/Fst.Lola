using System;

namespace FomMonitoringCore.Framework.Model
{
    public class HistoryPieceModel
    {
        public int Id { get; set; }
        public int? CompletedCount { get; set; }
        public DateTime Day { get; set; }
        public long? ElapsedTime { get; set; }
        public long? ElapsedTimeProducing { get; set; }
        public long? ElapsedTimeCut { get; set; }
        public long? ElapsedTimeWorking { get; set; }
        public long? ElapsedTimeTrim { get; set; }
        public long? ElapsedTimeAnuba { get; set; }
        public int? MachineId { get; set; }
        public string Operator { get; set; }
        public int? Period { get; set; }
        public int? PieceLengthSum { get; set; }
        public int? RedoneCount { get; set; }
        public int? Shift { get; set; }
        public string System { get; set; }
        public string TypeHistory { get; set; }
    }
}
