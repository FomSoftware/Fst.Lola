using System;

namespace FomMonitoringCore.Framework.Model
{
    public class JsonVariableValueModel
    {
        public int VariableType { get; set; }
        public string VariableNumber { get; set; }
        public decimal VariableValue { get; set; }
        public DateTime? VariableResetDate { get; set; }
    }
}
