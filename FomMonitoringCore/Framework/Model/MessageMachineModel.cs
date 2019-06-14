using System;

namespace FomMonitoringCore.Framework.Model
{
    public class MessageMachineModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime? Day { get; set; }
        public string Params { get; set; }
        public long? ElapsedTime { get; set; }
        public int? MachineId { get; set; }
        public string Operator { get; set; }
        public string Type { get; set; }
        public int? Group { get; set; }
        public DateTime? StartTime { get; set; }
        public int? StateId { get; set; }       
        public DateTime? IgnoreDate { get; set; }
        public bool? IsPeriodicMsg { get; set; }
    }
}
