using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.SqlServer;
using UserManager.Framework.Util;

namespace UserManager.Gateway.Concrete
{
    public static class AuditLogin
    {
        public static List<FomMonitoringCore.SqlServer.AuditLogin> GetListOfAuditLogin(FomMonitoringEntities userManagerEntities)
        {
            try
            {
                return (from al in userManagerEntities.AuditLogin
                        orderby al.DateAndTime descending
                        select al).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting audit login: {ex.Message}");
            }
        }

        public static List<FomMonitoringCore.SqlServer.AuditLogin> GetAuditLogin()
        {
            try
            {
                using (var userManagerEntities = new FomMonitoringEntities())
                {
                    return (from al in userManagerEntities.AuditLogin
                            select al).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting audit login: {ex.Message}");
            }

        }


        public const string EntitySetName = "AuditLogin";

        public static void InsertAuditLogin(bool accessed, string username, Guid? userId, string message)
        {
                ExtensionMethods.CheckGuidIsValidAndNotEmpty(userId.ToString(), out userId);

                using (var userManagerEntities = new FomMonitoringEntities())
                {
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

                    userManagerEntities.AuditLogin.Add(al);
                    userManagerEntities.SaveChanges();
                }
        }

        public static DateTime GetUserLastAccessedDateAndTime(Guid userId)
        {
                using (var userManagerEntities = new FomMonitoringEntities())
                {
                    var lastAccessDateAndTime = (from al in userManagerEntities.AuditLogin
                                                       where al.UserID == userId
                                                       orderby al.DateAndTime descending
                                                       select al.DateAndTime).FirstOrDefault();

                    return lastAccessDateAndTime;
                }
        }

    }
}
