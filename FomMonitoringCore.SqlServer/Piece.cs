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
    
    public partial class Piece
    {
        public int Id { get; set; }
        public Nullable<int> BarId { get; set; }
        public Nullable<System.DateTime> Day { get; set; }
        public Nullable<long> ElapsedTime { get; set; }
        public Nullable<long> ElapsedTimeProducing { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<bool> IsRedone { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<double> Length { get; set; }
        public Nullable<int> MachineId { get; set; }
        public string Operator { get; set; }
        public string RedoneReason { get; set; }
        public Nullable<int> Shift { get; set; }
        public Nullable<long> ElapsedTimeCut { get; set; }
        public Nullable<long> ElapsedTimeWorking { get; set; }
        public Nullable<long> ElapsedTimeTrim { get; set; }
    
        public virtual Bar Bar { get; set; }
        public virtual Machine Machine { get; set; }
        public virtual HistoryJob HistoryJob { get; set; }
    }
}
