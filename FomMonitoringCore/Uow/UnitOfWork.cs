using FomMonitoringCore.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FomMonitoringCore.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        public DbContextTransaction transaction { get; private set; }

        public void StartTransaction(IDbContext dbContext)
        {
            this.transaction = dbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            this.transaction.Commit();
        }

        public void RollbackTransaction()
        {
            this.transaction.Rollback();
        }

        public void Dispose()
        {
            this.transaction?.Dispose();
        }

    }
}
