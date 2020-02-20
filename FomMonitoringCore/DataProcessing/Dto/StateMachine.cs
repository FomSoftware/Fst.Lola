using System;

namespace FomMonitoringCore.DataProcessing.Dto
{
    public class StateMachine : BaseModel
    {
        public int Id { get; set; }
        public DateTime? StartTime { get; set; }
        public long? TimeSpanDuration { get; set; }
        public DateTime? EndTime { get; set; }
        public double? Overfeed { get; set; }
        public int? State { get; set; }
        public string Reason { get; set; }
        public string Operator { get; set; }
    }
}