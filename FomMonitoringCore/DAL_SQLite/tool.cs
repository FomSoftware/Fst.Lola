//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FomMonitoringCore.DAL_SQLite
{
    using System;
    using System.Collections.Generic;
    
    public partial class tool
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> DateLoaded { get; set; }
        public Nullable<System.DateTime> DateReplaced { get; set; }
        public Nullable<long> CurrentLife { get; set; }
        public Nullable<long> ExpectedLife { get; set; }
    }
}
