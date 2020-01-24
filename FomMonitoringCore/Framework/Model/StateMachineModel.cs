using System;

namespace FomMonitoringCore.Framework.Model
{
    public class StateMachineModel
    {
        public int Id { get; set; }
        public DateTime? Day { get; set; }
        public long? ElapsedTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? MachineId { get; set; }
        public string Operator { get; set; }
        public double? Overfeed { get; set; }
        public string Reason { get; set; }
        public string Shift { get; set; }
        public DateTime? StartTime { get; set; }
        public int? StateId { get; set; }
    }
}
