using System;
using System.Collections.Generic;

namespace UserManager.Service
{
   public interface IAuditLoginServices
    {
        List<FomMonitoringCore.SqlServer.AuditLogin> GetAuditLogin();
        List<FomMonitoringCore.SqlServer.AuditLogin> GetListOfAuditLogin(FomMonitoringCore.SqlServer.FomMonitoringEntities modelloEntity);
        DateTime GetUserLastAccessedDateAndTime(Guid userId);
    }
}
