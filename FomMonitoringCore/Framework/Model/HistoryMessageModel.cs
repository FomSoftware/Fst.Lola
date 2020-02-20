using System;

namespace FomMonitoringCore.Framework.Model
{
    public class HistoryMessageModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int? Count { get; set; }
        public string Description { get; set; }
        public DateTime? Day { get; set; }
        public string Params { get; set; }
        public long? ElapsedTime { get; set; }
        public int? MachineId { get; set; }
        public int? Period { get; set; }
        public int? StateId { get; set; }
        public int? Type { get; set; }
        public string TypeHistory { get; set; }
    }
}
