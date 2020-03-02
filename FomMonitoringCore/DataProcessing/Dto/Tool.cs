using System;

namespace FomMonitoringCore.DataProcessing.Dto
{
    public class ToolMachine
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime? DateLoaded { get; set; }
        public DateTime? DateReplaced { get; set; }
        public long? CurrentLife { get; set; }
        public long? ExpectedLife { get; set; }
    }
}