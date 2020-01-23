//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FomMonitoringCore.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class CurrentState
    {
        public int Id { get; set; }
        public string JobCode { get; set; }
        public Nullable<int> JobProducedPieces { get; set; }
        public Nullable<int> JobTotalPieces { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<int> MachineId { get; set; }
        public string Operator { get; set; }
        public Nullable<int> StateId { get; set; }
        public string StateTransitionCode { get; set; }
    
        public virtual State State { get; set; }
        public virtual Machine Machine { get; set; }
    }
}
