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
        public List<FomMonitoringCore.SqlServer.AuditLogin> GetListOfAuditLogin()
        {
            try
            {
                return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.AuditLogin>()
                    .OrderByDescending(n => n.DateAndTime).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting audit login: {ex.Message}");
            }
        }

        public List<FomMonitoringCore.SqlServer.AuditLogin> GetAuditLogin()
        {
            try
            {
                return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.AuditLogin>().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting audit login: {ex.Message}");
            }

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

        public DateTime? GetUserLastAccessedDateAndTime(Guid userId)
        {
            return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.AuditLogin>().OrderByDescending(al => al.DateAndTime)
                .FirstOrDefault(al => al.UserID == userId)?.DateAndTime;
                
        }

    }
}
