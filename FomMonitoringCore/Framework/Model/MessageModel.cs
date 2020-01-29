using System;
using System.Collections.Generic;

namespace FomMonitoringCore.Framework.Model
{
    public class MessageModel
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> Time { get; set; }
        public string Code { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string Operator { get; set; }
        public Nullable<int> State { get; set; }
    }
}
