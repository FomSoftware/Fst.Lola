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
    
    public partial class historyPiece
    {
        public int Id { get; set; }
        public Nullable<int> CompletedCount { get; set; }
        public Nullable<System.DateTime> Day { get; set; }
        public Nullable<long> ElapsedTime { get; set; }
        public Nullable<long> ElapsedTimeProducing { get; set; }
        public string Operator { get; set; }
        public Nullable<int> Period { get; set; }
        public Nullable<int> PieceLengthSum { get; set; }
        public Nullable<int> RedoneCount { get; set; }
        public string System { get; set; }
    }
}
