﻿using System;
using System.Data;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringCore.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        void CommitTransaction();
        void RollbackTransaction();
        void StartTransaction(IDbContext dbContext, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}