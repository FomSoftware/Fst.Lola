using System.Data.Entity;
using System.Data;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringCore.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        public DbContextTransaction Transaction { get; private set; }

        public void StartTransaction(IDbContext dbContext, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            Transaction = dbContext.Database.BeginTransaction(isolationLevel);
        }

        public void CommitTransaction()
        {
            Transaction.Commit();
        }

        public void RollbackTransaction()
        {
            Transaction.Rollback();
        }

        public void Dispose()
        {
            Transaction?.Dispose();
        }

    }
}
