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
        public DateTime? StartTime { get; set; }
        public bool? IsRedone { get; set; }
        public int? JobId { get; set; }
        public double? Length { get; set; }
        public int? MachineId { get; set; }
        public string Operator { get; set; }
        public string RedoneReason { get; set; }
        public int? Shift { get; set; }
        public long? ElapsedTimeCut { get; set; }
        public long? ElapsedTimeWorking { get; set; }
        public long? ElapsedTimeTrim { get; set; }
        public long? ElapsedTimeAnuba { get; set; }
    }
}
