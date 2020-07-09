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
    
    public partial class MessagesIndex
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MessagesIndex()
        {
            this.HistoryMessage = new HashSet<HistoryMessage>();
            this.MessageMachine = new HashSet<MessageMachine>();
            this.MessageTranslation = new HashSet<MessageTranslation>();
            this.ParameterMachineThreshold = new HashSet<ParameterMachineThreshold>();
        }
    
        public int Id { get; set; }
        public string MessageCode { get; set; }
        public int MessageCategoryId { get; set; }
        public int MessageTypeId { get; set; }
        public bool IsVisibleLOLA { get; set; }
        public int MachineGroupId { get; set; }
        public Nullable<long> PeriodicSpan { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsPeriodicM { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HistoryMessage> HistoryMessage { get; set; }
        public virtual MachineGroup MachineGroup { get; set; }
        public virtual MessageCategory MessageCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MessageMachine> MessageMachine { get; set; }
        public virtual MessageType MessageType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MessageTranslation> MessageTranslation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParameterMachineThreshold> ParameterMachineThreshold { get; set; }
    }
}