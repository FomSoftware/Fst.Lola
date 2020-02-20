using System;

namespace FomMonitoringCore.Framework.Model
{
    public class CurrentStateModel
    {
        public int Id { get; set; }
        public string JobCode { get; set; }
        public int? JobProducedPieces { get; set; }
        public int? JobTotalPieces { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int? MachineId { get; set; }
        public string Operator { get; set; }
        public int? StateId { get; set; }
        public string StateTransitionCode { get; set; }
    }
}