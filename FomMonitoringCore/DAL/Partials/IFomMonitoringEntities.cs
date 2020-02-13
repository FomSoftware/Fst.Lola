using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCore.DAL.Interceptor;

namespace FomMonitoringCore.DAL
{
    public interface IDbContext : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync();
        int SaveChanges();
        Database Database { get; }
        DbEntityEntry Entry(object entity);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }

    public interface IFomMonitoringEntities : IDbContext
    {
        ObjectResult<Nullable<int>> usp_CleanMachineData(Nullable<int> machineId);
        ObjectResult<usp_AggregationState_Result> usp_AggregationState(Nullable<int> machineId, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> aggregation, Nullable<int> dataType);
        ObjectResult<usp_AggregationPiece_Result> usp_AggregationPiece(Nullable<int> machineId, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> aggregation, Nullable<int> dataType);
        ObjectResult<usp_AggregationBar_Result> usp_AggregationBar(Nullable<int> machineId, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> aggregation);
        ObjectResult<usp_AggregationJob_Result> usp_AggregationJob(Nullable<int> machineId, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> aggregation);
        int usp_HistoricizingAll(Nullable<int> machineId);
        ObjectResult<usp_AggregationMessage_Result> usp_AggregationMessage(Nullable<int> machineId, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> aggregation, Nullable<int> dataType);

    }

    public partial class FST_FomMonitoringEntities : DbContext, IFomMonitoringEntities
    {

    }



    public interface IFomMonitoringEntitiesIsolated : IFomMonitoringEntities
    {


    }

    public partial class FomMonitoringEntitiesIsolated : FST_FomMonitoringEntities, IFomMonitoringEntitiesIsolated
    {
        public FomMonitoringEntitiesIsolated()
        {
            DbInterception.Add(new IsolationLevelInterceptor(IsolationLevel.RepeatableRead));
        }
    }
}
