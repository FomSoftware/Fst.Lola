using System;

namespace FomMonitoringCore.DataProcessing.Dto
{
    public class Piece : BaseModel
    {
        public int Id { get; set; }
        public int? BarId { get; set; }
        public double? Length { get; set; }
        public string JobCode { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? StartTime { get; set; }
        public long? TimeSpan { get; set; }
        public long? TimeSpanProducing { get; set; }
        public string Operator { get; set; }
        public bool? Redone { get; set; }
        public string RedoneReason { get; set; }
        public long? TimeSpanCutting { get; set; }
        public long? TimeSpanWorking { get; set; }
        public long? TimeSpanTrim { get; set; }
        public long? TimeSpanAnuba { get; set; }
    }
}