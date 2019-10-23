using FomMonitoringCore.DAL;
using System;

namespace FomMonitoringCore.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        void CommitTransaction();
        void RollbackTransaction();
        void StartTransaction(IDbContext dbContext);
    }
}