using System;

namespace FomMonitoringCore.Framework.Model
{
    public class SpindleModel
    {
        public int Id { get; set; }
        //public int ChangeCount { get; set; }
        public string Code { get; set; }
        public long? ElapsedTimeWorkTotal { get; set; }
        public long? ElapsedTimeWork3K { get; set; }
        public long? ElapsedTimeWork6K { get; set; }
        public long? ElapsedTimeWork9K { get; set; }
        public long? ElapsedTimeWork12K { get; set; }
        public long? ElapsedTimeWork15K { get; set; }
        public long? ElapsedTimeWork18K { get; set; }
        public decimal AverageElapsedTimeWork { get; set; }
        public long? ExpectedWorkTime { get; set; }
        public int? WorkOverheatingCount { get; set; }
        public int? WorkOverPowerCount { get; set; }
        public int? WorkOverVibratingCount { get; set; }
        public DateTime? InstallDate { get; set; }
        public int? MachineId { get; set; }
        public DateTime? ReplacedDate { get; set; }
        public string Serial { get; set; }
        public int? ToolChangedCount { get; set; }
    }
}
