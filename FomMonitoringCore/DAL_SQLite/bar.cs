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
    
    public partial class bar
    {
        public int Id { get; set; }
        public Nullable<int> Index { get; set; }
        public string System { get; set; }
        public string ProfileCode { get; set; }
        public string Color { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<long> TimeSpanTotal { get; set; }
        public Nullable<long> TimeSpanProducing { get; set; }
        public Nullable<double> Length { get; set; }
        public Nullable<bool> IsOffcut { get; set; }
        public string BadAreas { get; set; }
        public string JobCode { get; set; }
    }
}
