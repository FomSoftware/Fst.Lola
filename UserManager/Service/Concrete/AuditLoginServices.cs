using System;
using System.Collections.Generic;
using FomMonitoringCore.SqlServer;
using AuditLogin = UserManager.Gateway.Concrete.AuditLogin;

namespace UserManager.Service.Concrete
{
  public  class AuditLoginServices : IAuditLoginServices
    {
      public List<FomMonitoringCore.SqlServer.AuditLogin> GetAuditLogin()
      {
          return AuditLogin.GetAuditLogin();
      }

      public List<FomMonitoringCore.SqlServer.AuditLogin> GetListOfAuditLogin(FomMonitoringEntities ModelloEntity)
      {
          return AuditLogin.GetListOfAuditLogin(ModelloEntity);

      }
      public DateTime GetUserLastAccessedDateAndTime(Guid UserID)
      {
          return AuditLogin.GetUserLastAccessedDateAndTime(UserID);
      }
    }
}
