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
    
    public partial class HistoryBar
    {
        public int Id { get; set; }
        public Nullable<int> Count { get; set; }
        public Nullable<System.DateTime> Day { get; set; }
        public Nullable<double> Length { get; set; }
        public Nullable<int> MachineId { get; set; }
        public Nullable<int> OffcutCount { get; set; }
        public Nullable<int> OffcutLength { get; set; }
        public Nullable<int> Period { get; set; }
        public string System { get; set; }
        public string TypeHistory { get; set; }
    
        public virtual Machine Machine { get; set; }
    }
}
