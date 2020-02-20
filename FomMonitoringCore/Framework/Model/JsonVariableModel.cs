using System;
using System.Collections.Generic;

namespace FomMonitoringCore.Framework.Model
{
    public class JsonVariablesModel
    {
        public int Id { get; set; }
        public DateTime UtcDateTime { get; set; }
        public List<JsonVariableValueModel> Values { get; set; }
    }
}
