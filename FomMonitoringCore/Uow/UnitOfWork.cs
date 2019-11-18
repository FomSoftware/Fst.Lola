using FomMonitoringCore.DAL;
using System.Data.Entity;

namespace FomMonitoringCore.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        public DbContextTransaction Transaction { get; private set; }

        public void StartTransaction(IDbContext dbContext)
        {
            Transaction = dbContext.Database.BeginTransaction();
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
