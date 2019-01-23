using System;

namespace FomMonitoringCore.Framework.Model
{
    public class AlarmMachineModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime? Day { get; set; }
        public string Description { get; set; }
        public long? ElapsedTime { get; set; }
        public int? MachineId { get; set; }
        public string Operator { get; set; }
        public DateTime? StartTime { get; set; }
        public int? StateId { get; set; }
    }
}
