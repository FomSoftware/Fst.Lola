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
    
    public partial class usp_AggregationJob_Result
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public Nullable<System.DateTime> Day { get; set; }
        public Nullable<long> ElapsedTime { get; set; }
        public Nullable<int> MachineId { get; set; }
        public Nullable<int> Period { get; set; }
        public Nullable<int> PiecesProduced { get; set; }
        public Nullable<int> TotalPieces { get; set; }
        public string TypeHistory { get; set; }
    }
}
