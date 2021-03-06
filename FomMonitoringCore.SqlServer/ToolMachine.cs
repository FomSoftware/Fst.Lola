//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FomMonitoringCore.SqlServer
{
    using System;
    using System.Collections.Generic;
    
    public partial class ToolMachine
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public Nullable<long> CurrentLife { get; set; }
        public Nullable<System.DateTime> DateLoaded { get; set; }
        public Nullable<System.DateTime> DateReplaced { get; set; }
        public string Description { get; set; }
        public Nullable<long> ExpectedLife { get; set; }
        public bool IsActive { get; set; }
        public bool IsBroken { get; set; }
        public bool IsRevised { get; set; }
        public Nullable<int> MachineId { get; set; }
    
        public virtual Machine Machine { get; set; }
    }
}
