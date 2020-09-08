using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.SqlServer;
using UserManager.Framework.Util;

namespace UserManager.Gateway
{
    public class AuditLogin : IAuditLogin
    {
        private readonly IFomMonitoringEntities _fomMonitoringEntities;

        public AuditLogin(IFomMonitoringEntities fomMonitoringEntities)
        {
            _fomMonitoringEntities = fomMonitoringEntities;
        }
       

        public void InsertAuditLogin(bool accessed, string username, Guid? userId, string message)
        {
                ExtensionMethods.CheckGuidIsValidAndNotEmpty(userId.ToString(), out userId);
            
                    var al = new FomMonitoringCore.SqlServer.AuditLogin
                    {
                        ID = Guid.NewGuid(),
                        Accessed = accessed,
                        Username = username,
                        UserID = userId,
                        MessageInfo = message,
                        DateAndTime = DateTime.Now,
                        IP = ExtensionMethods.GetUserConnectionIP()
                    };

                    _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.AuditLogin>().Add(al);
                    _fomMonitoringEntities.SaveChanges();
                
        }

    }
}
