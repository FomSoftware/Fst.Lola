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
    using System.Collections.Generic;
    
    public partial class ParameterMachine
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ParameterMachine()
        {
            this.ParameterMachineValue = new HashSet<ParameterMachineValue>();
        }
    
        public int Id { get; set; }
        public string ModelCode { get; set; }
        public string VarNumber { get; set; }
        public string HmiLabel { get; set; }
        public Nullable<int> MachineGroup { get; set; }
        public string CnType { get; set; }
        public string CnUm { get; set; }
        public string Keyword { get; set; }
        public string HmiUm { get; set; }
        public string DefaultValue { get; set; }
        public string WLevel { get; set; }
        public string RLevel { get; set; }
        public string HmiSection { get; set; }
        public string LolaLabel { get; set; }
        public string ThresholdMin { get; set; }
        public string ThresholdMax { get; set; }
        public string ThresholdLabel { get; set; }
        public string Cluster { get; set; }
        public int MachineModelId { get; set; }
        public string Historicized { get; set; }
        public Nullable<int> PanelId { get; set; }
        public string Panel { get; set; }
    
        public virtual MachineModel MachineModel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParameterMachineValue> ParameterMachineValue { get; set; }
        public virtual Panel Panel1 { get; set; }
    }
}
