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
        public int? MachineId { get; set; }
        public int? Period { get; set; }
        public string TypeHistory { get; set; }
        public int? MessageIndexId { get; set; }
        public int? Type { get; set; }
    }
}
