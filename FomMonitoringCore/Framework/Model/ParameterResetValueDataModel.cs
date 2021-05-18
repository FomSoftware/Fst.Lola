using System;

namespace FomMonitoringCore.Framework.Model
{
    public class ParameterResetValueDataModel
    {
        public int Id { get; set; }
        public int ParameterMachineId { get; set; }
        public DateTime? ResetDate { get; set; }
        public decimal? ResetValue { get; set; }
        public decimal? ValueBeforeReset { get; set; }
        public int? MachineId { get; set; }
        public string MachineGroupName { get; set; }
        public string VariableName { get; set; }
    }
}
