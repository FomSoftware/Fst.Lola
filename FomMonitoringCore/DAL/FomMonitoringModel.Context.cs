﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class FST_FomMonitoringEntities : DbContext
    {
        public FST_FomMonitoringEntities()
            : base("name=FST_FomMonitoringEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AlarmMachine> AlarmMachine { get; set; }
        public virtual DbSet<HistoryAlarm> HistoryAlarm { get; set; }
        public virtual DbSet<MachineModel> MachineModel { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<HistoryBar> HistoryBar { get; set; }
        public virtual DbSet<HistoryState> HistoryState { get; set; }
        public virtual DbSet<StateMachine> StateMachine { get; set; }
        public virtual DbSet<Piece> Piece { get; set; }
        public virtual DbSet<Bar> Bar { get; set; }
        public virtual DbSet<HistoryPiece> HistoryPiece { get; set; }
        public virtual DbSet<CurrentState> CurrentState { get; set; }
        public virtual DbSet<ToolMachine> ToolMachine { get; set; }
        public virtual DbSet<MachineType> MachineType { get; set; }
        public virtual DbSet<Machine> Machine { get; set; }
        public virtual DbSet<Plant> Plant { get; set; }
        public virtual DbSet<HistoryJob> HistoryJob { get; set; }
        public virtual DbSet<UserCustomerMapping> UserCustomerMapping { get; set; }
        public virtual DbSet<Spindle> Spindle { get; set; }
        public virtual DbSet<JsonData> JsonData { get; set; }
        public virtual DbSet<UserMachineMapping> UserMachineMapping { get; set; }
        public virtual DbSet<HistoryMessage> HistoryMessage { get; set; }
        public virtual DbSet<MessageMachine> MessageMachine { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<FomMonitoring> FomMonitoring { get; set; }
        public virtual DbSet<FomMonitoringApi> FomMonitoringApi { get; set; }
        public virtual DbSet<FomMonitoringDatabaseMapping> FomMonitoringDatabaseMapping { get; set; }
        public virtual DbSet<FomMonitoringUpdateUsers> FomMonitoringUpdateUsers { get; set; }
    
        public virtual ObjectResult<usp_AggregationState_Result> usp_AggregationState(Nullable<int> machineId, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> aggregation, Nullable<int> dataType)
        {
            var machineIdParameter = machineId.HasValue ?
                new ObjectParameter("machineId", machineId) :
                new ObjectParameter("machineId", typeof(int));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(System.DateTime));
    
            var aggregationParameter = aggregation.HasValue ?
                new ObjectParameter("aggregation", aggregation) :
                new ObjectParameter("aggregation", typeof(int));
    
            var dataTypeParameter = dataType.HasValue ?
                new ObjectParameter("dataType", dataType) :
                new ObjectParameter("dataType", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_AggregationState_Result>("usp_AggregationState", machineIdParameter, startDateParameter, endDateParameter, aggregationParameter, dataTypeParameter);
        }
    
        public virtual ObjectResult<usp_AggregationPiece_Result> usp_AggregationPiece(Nullable<int> machineId, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> aggregation, Nullable<int> dataType)
        {
            var machineIdParameter = machineId.HasValue ?
                new ObjectParameter("machineId", machineId) :
                new ObjectParameter("machineId", typeof(int));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(System.DateTime));
    
            var aggregationParameter = aggregation.HasValue ?
                new ObjectParameter("aggregation", aggregation) :
                new ObjectParameter("aggregation", typeof(int));
    
            var dataTypeParameter = dataType.HasValue ?
                new ObjectParameter("dataType", dataType) :
                new ObjectParameter("dataType", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_AggregationPiece_Result>("usp_AggregationPiece", machineIdParameter, startDateParameter, endDateParameter, aggregationParameter, dataTypeParameter);
        }
    
        public virtual ObjectResult<usp_AggregationBar_Result> usp_AggregationBar(Nullable<int> machineId, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> aggregation)
        {
            var machineIdParameter = machineId.HasValue ?
                new ObjectParameter("machineId", machineId) :
                new ObjectParameter("machineId", typeof(int));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(System.DateTime));
    
            var aggregationParameter = aggregation.HasValue ?
                new ObjectParameter("aggregation", aggregation) :
                new ObjectParameter("aggregation", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_AggregationBar_Result>("usp_AggregationBar", machineIdParameter, startDateParameter, endDateParameter, aggregationParameter);
        }
    
        public virtual ObjectResult<usp_AggregationAlarm_Result> usp_AggregationAlarm(Nullable<int> machineId, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> aggregation, Nullable<int> dataType)
        {
            var machineIdParameter = machineId.HasValue ?
                new ObjectParameter("machineId", machineId) :
                new ObjectParameter("machineId", typeof(int));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(System.DateTime));
    
            var aggregationParameter = aggregation.HasValue ?
                new ObjectParameter("aggregation", aggregation) :
                new ObjectParameter("aggregation", typeof(int));
    
            var dataTypeParameter = dataType.HasValue ?
                new ObjectParameter("dataType", dataType) :
                new ObjectParameter("dataType", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_AggregationAlarm_Result>("usp_AggregationAlarm", machineIdParameter, startDateParameter, endDateParameter, aggregationParameter, dataTypeParameter);
        }
    
        public virtual ObjectResult<usp_AggregationJob_Result> usp_AggregationJob(Nullable<int> machineId, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> aggregation)
        {
            var machineIdParameter = machineId.HasValue ?
                new ObjectParameter("machineId", machineId) :
                new ObjectParameter("machineId", typeof(int));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(System.DateTime));
    
            var aggregationParameter = aggregation.HasValue ?
                new ObjectParameter("aggregation", aggregation) :
                new ObjectParameter("aggregation", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_AggregationJob_Result>("usp_AggregationJob", machineIdParameter, startDateParameter, endDateParameter, aggregationParameter);
        }
    
        public virtual ObjectResult<usp_MesUserMachines_Result> usp_MesUserMachines(Nullable<int> plantId, Nullable<System.DateTime> date)
        {
            var plantIdParameter = plantId.HasValue ?
                new ObjectParameter("plantId", plantId) :
                new ObjectParameter("plantId", typeof(int));
    
            var dateParameter = date.HasValue ?
                new ObjectParameter("date", date) :
                new ObjectParameter("date", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_MesUserMachines_Result>("usp_MesUserMachines", plantIdParameter, dateParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> usp_CleanMachineData(Nullable<int> machineId)
        {
            var machineIdParameter = machineId.HasValue ?
                new ObjectParameter("machineId", machineId) :
                new ObjectParameter("machineId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("usp_CleanMachineData", machineIdParameter);
        }
    
        public virtual int usp_HistoricizingAll(Nullable<int> machineId)
        {
            var machineIdParameter = machineId.HasValue ?
                new ObjectParameter("machineId", machineId) :
                new ObjectParameter("machineId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_HistoricizingAll", machineIdParameter);
        }
    }
}
