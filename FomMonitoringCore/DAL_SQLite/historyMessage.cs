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
    
    public partial class historyMessage
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public Nullable<int> Count { get; set; }
        public string Params { get; set; }
        public Nullable<System.DateTime> Day { get; set; }
        public Nullable<long> ElapsedTime { get; set; }
        public Nullable<int> Period { get; set; }
        public Nullable<int> StateId { get; set; }
    }
}
