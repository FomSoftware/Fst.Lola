using System;

namespace FomMonitoringCore.DataProcessing.Dto
{
    public class Spindle : BaseModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public long? ExpectedWorkTime { get; set; }
        public DateTime? InstallDate { get; set; }
        public long? WorkTotalTime { get; set; }
        public long? Work3KTime { get; set; }
        public long? Work6KTime { get; set; }
        public long? Work9KTime { get; set; }
        public long? Work12KTime { get; set; }
        public long? Work15KTime { get; set; }
        public long? Work18KTime { get; set; }
        public int? WorkOverheatingEvents { get; set; }
        public int? WorkOverPowerEvents { get; set; }
        public int? WorkOverVibratingEvents { get; set; }
        public DateTime? Replaced { get; set; }
        public int? ToolChangedCount { get; set; }
        public string Serial { get; set; }
    }
}