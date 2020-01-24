namespace FomMonitoringCore.Framework.Model
{
    using System;

    public class ToolMachineModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public long? CurrentLife { get; set; }
        public DateTime? DateLoaded { get; set; }
        public DateTime? DateReplaced { get; set; }
        public string Description { get; set; }
        public long? ExpectedLife { get; set; }
        public bool IsActive { get; set; }
        public bool IsBroken { get; set; }
        public bool IsRevised { get; set; }
        public int? MachineId { get; set; }
    }
}
