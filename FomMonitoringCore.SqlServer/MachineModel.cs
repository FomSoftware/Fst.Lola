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
    
    public partial class MachineModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MachineModel()
        {
            this.Machine = new HashSet<Machine>();
            this.ParameterMachine = new HashSet<ParameterMachine>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int ModelCodev997 { get; set; }
        public int MessageCategoryId { get; set; }
        public string Parameter { get; set; }
        public Nullable<int> MachineTypeId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Machine> Machine { get; set; }
        public virtual MessageCategory MessageCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParameterMachine> ParameterMachine { get; set; }
    }
}
