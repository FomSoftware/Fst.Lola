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
    
    public partial class piece
    {
        public int Id { get; set; }
        public Nullable<int> BarId { get; set; }
        public Nullable<double> Length { get; set; }
        public string JobCode { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<long> TimeSpan { get; set; }
        public Nullable<long> TimeSpanProducing { get; set; }
        public string Operator { get; set; }
        public Nullable<bool> Redone { get; set; }
        public string RedoneReason { get; set; }
        public Nullable<long> TimeSpanCutting { get; set; }
        public Nullable<long> TimeSpanWorking { get; set; }
        public Nullable<long> TimeSpanTrim { get; set; }
        public Nullable<long> TimeSpanAnuba { get; set; }
    }
}
