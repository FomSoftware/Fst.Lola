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
    
    public partial class usp_MesUserMachines_Result
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public Nullable<double> StateEfficiency { get; set; }
        public Nullable<double> StateOverfeedAvg { get; set; }
        public Nullable<int> PieceCompletedCount { get; set; }
        public Nullable<long> PieceElapsedTime { get; set; }
        public Nullable<long> PieceElapsedTimeProducing { get; set; }
        public Nullable<int> PieceRedoneCount { get; set; }
        public Nullable<int> AlarmCount { get; set; }
        public Nullable<long> AlarmElapsedTime { get; set; }
        public Nullable<int> ActualStateId { get; set; }
        public string ActualStateCode { get; set; }
        public string ActualJobCode { get; set; }
        public Nullable<double> ActualJobPerc { get; set; }
        public string ActualOperator { get; set; }
    }
}
