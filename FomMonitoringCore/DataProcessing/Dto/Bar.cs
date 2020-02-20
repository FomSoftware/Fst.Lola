using System;

namespace FomMonitoringCore.DataProcessing.Dto
{
    public class Bar:BaseModel
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
        public string BadAreas { get; set; }
        public string JobCode { get; set; }
    }
}