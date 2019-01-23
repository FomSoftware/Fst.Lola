using System;

namespace FomMonitoringCore.Framework.Models
{
    public class CurrentStateModel
    {
        public int Id { get; set; }
        public string JobCode { get; set; }
        public int? JobProducedPieces { get; set; }
        public int? JobTotalPieces { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int? MachineId { get; set; }
        public DateTime? NextMaintenanceService { get; set; }
        public string Operator { get; set; }
        public string Plant { get; set; }
        public int? StateId { get; set; }
        public string StateTransitionCode { get; set; }
        public string StateTransitionDescription { get; set; }
    }
}