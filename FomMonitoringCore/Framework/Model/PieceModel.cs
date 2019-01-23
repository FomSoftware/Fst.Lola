using System;

namespace FomMonitoringCore.Framework.Model
{
    public class PieceModel
    {
        public int Id { get; set; }
        public int? BarId { get; set; }
        public DateTime? Day { get; set; }
        public long? ElapsedTime { get; set; }
        public long? ElapsedTimeProducing { get; set; }
        public DateTime? EndTime { get; set; }
        public string FrameId { get; set; }
        public int? Index { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? IsRedone { get; set; }
        public int? JobId { get; set; }
        public double? Length { get; set; }
        public int? MachineId { get; set; }
        public string Operator { get; set; }
        public string RedoneReason { get; set; }
        public int? Shift { get; set; }
        public string System { get; set; }
        public string TipologyCode { get; set; }
        public DateTime? StartTimeCut { get; set; }
        public DateTime? EndTimeCut { get; set; }
        public long? ElapsedTimeCut { get; set; }
        public DateTime? StartTimeWorking { get; set; }
        public DateTime? EndTimeWorking { get; set; }
        public long? ElapsedTimeWorking { get; set; }
        public DateTime? StartTimeTrim { get; set; }
        public DateTime? EndTimeTrim { get; set; }
        public long? ElapsedTimeTrim { get; set; }
        public DateTime? StartTimeAnuba { get; set; }
        public DateTime? EndTimeAnuba { get; set; }
        public long? ElapsedTimeAnuba { get; set; }
    }
}
