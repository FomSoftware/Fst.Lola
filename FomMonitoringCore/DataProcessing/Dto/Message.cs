using System;

namespace FomMonitoringCore.DataProcessing.Dto
{
    public class MessageMachine
    {
        public int Id { get; set; }
        public DateTime? Time { get; set; }
        public long? TimeSpanDuration { get; set; }
        public string Code { get; set; }
        public string Params { get; set; }
        public string Operator { get; set; }
        public int? State { get; set; }
        public string Type { get; set; }
        public int? Group { get; set; }
    }
}