using System;
using System.Collections.Generic;

namespace UserManager.Gateway
{
    public interface IAuditLogin
    {
        List<FomMonitoringCore.SqlServer.AuditLogin> GetListOfAuditLogin();
        List<FomMonitoringCore.SqlServer.AuditLogin> GetAuditLogin();
        void InsertAuditLogin(bool accessed, string username, Guid? userId, string message);
        DateTime? GetUserLastAccessedDateAndTime(Guid userId);
    }
}