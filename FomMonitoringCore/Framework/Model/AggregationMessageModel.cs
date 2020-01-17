using FomMonitoringCore.Framework.Common;
using System;

namespace FomMonitoringCore.Framework.Model
{
    public partial class AggregationMessageModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? Count { get; set; }
        public DateTime? Day { get; set; }
        public string Params { get; set; }
        public long? ElapsedTime { get; set; }
        public int? MachineId { get; set; }
        public int? Period { get; set; }
        public int? StateId { get; set; }
        public enState enState { get; set; }
        public string TypeHistory { get; set; }
        public int? Group { get; set; }

        public int? Type { get; set; }
    }
}
