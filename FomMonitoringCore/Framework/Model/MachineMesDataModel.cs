using System;

namespace FomMonitoringCore.Framework.Model
{
    public class MachineMesDataModel
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public double? StateEfficiency { get; set; }
        public double? StateOverfeedAvg { get; set; }
        public int? PieceCompletedCount { get; set; }
        public long? PieceElapsedTime { get; set; }
        public long? PieceElapsedTimeProducing { get; set; }
        public int? PieceRedoneCount { get; set; }
        public int? AlarmCount { get; set; }
        public int? ActualStateId { get; set; }
        public string ActualStateCode { get; set; }
        public string ActualJobCode { get; set; }
        public double? ActualJobPerc { get; set; }
        public string ActualOperator { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}