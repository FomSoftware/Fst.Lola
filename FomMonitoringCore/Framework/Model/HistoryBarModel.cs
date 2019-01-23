using System;

namespace FomMonitoringCore.Framework.Model
{
    public class HistoryBarModel
    {
        public int Id { get; set; }
        public int? Count { get; set; }
        public DateTime? Day { get; set; }
        public double? Length { get; set; }
        public int? MachineId { get; set; }
        public int? OffcutCount { get; set; }
        public int? OffcutLength { get; set; }
        public int? Period { get; set; }
        public string System { get; set; }
        public string TypeHistory { get; set; }
    }
}
