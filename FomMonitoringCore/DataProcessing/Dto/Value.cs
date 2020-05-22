

// ReSharper disable InconsistentNaming

using System;

namespace FomMonitoringCore.DataProcessing.Dto
{
    public class Value
    {
        public int VariableType { get; set; }
        public string VariableNumber { get; set; }
        public decimal? VariableValue { get; set; }
        public DateTime? VariableResetDate { get; set; }
    }

}
