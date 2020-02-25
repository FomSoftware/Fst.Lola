using System;

namespace FomMonitoringCore.DataProcessing.Dto
{
    public class HistoryJobMachine : BaseModel
    {
        public int Id { get; set; }
        public string JobCode { get; set; }
        public int? TotalPiecesInJob { get; set; }
        public int? CurrentlyProducedPieces { get; set; }
        public DateTime? Day { get; set; }
        public long? ElapsedTime { get; set; }
    }

    public class BarMachine : BaseModel
    {
        public int Id { get; set; }
        public int? Index { get; set; }
        public string System { get; set; }
        public string ProfileCode { get; set; }
        public string Color { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double? Length { get; set; }
        public bool? IsOffcut { get; set; }
        public double? BadAreas { get; set; }
        public string JobCode { get; set; }
    }

    public class PieceMachine : BaseModel
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