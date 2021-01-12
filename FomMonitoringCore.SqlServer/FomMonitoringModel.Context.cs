﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class FomMonitoringEntities : DbContext
    {
        public FomMonitoringEntities()
            : base("name=FomMonitoringEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AuditLogin> AuditLogin { get; set; }
        public virtual DbSet<Bar> Bar { get; set; }
        public virtual DbSet<CurrentState> CurrentState { get; set; }
        public virtual DbSet<HistoryBar> HistoryBar { get; set; }
        public virtual DbSet<HistoryJob> HistoryJob { get; set; }
        public virtual DbSet<HistoryMessage> HistoryMessage { get; set; }
        public virtual DbSet<HistoryPiece> HistoryPiece { get; set; }
        public virtual DbSet<HistoryState> HistoryState { get; set; }
        public virtual DbSet<Languages> Languages { get; set; }
        public virtual DbSet<Machine> Machine { get; set; }
        public virtual DbSet<MachineGroup> MachineGroup { get; set; }
        public virtual DbSet<MachineModel> MachineModel { get; set; }
        public virtual DbSet<MachineType> MachineType { get; set; }
        public virtual DbSet<MessageCategory> MessageCategory { get; set; }
        public virtual DbSet<MessageLanguages> MessageLanguages { get; set; }
        public virtual DbSet<MessageMachine> MessageMachine { get; set; }
        public virtual DbSet<MessageMachineNotification> MessageMachineNotification { get; set; }
        public virtual DbSet<MessagesIndex> MessagesIndex { get; set; }
        public virtual DbSet<MessageTranslation> MessageTranslation { get; set; }
        public virtual DbSet<MessageType> MessageType { get; set; }
        public virtual DbSet<Panel> Panel { get; set; }
        public virtual DbSet<ParameterMachine> ParameterMachine { get; set; }
        public virtual DbSet<ParameterMachineThreshold> ParameterMachineThreshold { get; set; }
        public virtual DbSet<ParameterMachineValue> ParameterMachineValue { get; set; }
        public virtual DbSet<ParameterResetValue> ParameterResetValue { get; set; }
        public virtual DbSet<Piece> Piece { get; set; }
        public virtual DbSet<Plant> Plant { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Roles_Users> Roles_Users { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<StateMachine> StateMachine { get; set; }
        public virtual DbSet<ToolMachine> ToolMachine { get; set; }
        public virtual DbSet<UserCustomerMapping> UserCustomerMapping { get; set; }
        public virtual DbSet<UserMachineMapping> UserMachineMapping { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Faq> Faq { get; set; }
    
        public virtual int usp_CleanMachineData(Nullable<int> machineId)
        {
            var machineIdParameter = machineId.HasValue ?
                new ObjectParameter("machineId", machineId) :
                new ObjectParameter("machineId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_CleanMachineData", machineIdParameter);
        }
    }
}
