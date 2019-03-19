using System;
using System.Collections.Generic;
using System.Linq;
using UserManager.Framework.Util;

namespace UserManager.DAL.Gateway.Concrete
{
    public static class AuditLogin
    {
        public static List<DAL.AuditLogin> GetListOfAuditLogin(UserManagerEntities userManagerEntities)
        {
            try
            {
                return (from al in userManagerEntities.AuditLogin
                        orderby al.DateAndTime descending
                        select al).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error while getting audit login: {0}", ex.Message));
            }
        }

        public static List<DAL.AuditLogin> GetAuditLogin()
        {
            try
            {
                using (var userManagerEntities = new UserManagerEntities())
                {
                    return (from al in userManagerEntities.AuditLogin
                            select al).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error while getting audit login: {0}", ex.Message));
            }

        }


        public const string EntitySetName = "AuditLogin";

        public static void InsertAuditLogin(bool Accessed, string Username, Nullable<Guid> UserID, string Message)
        {
            try
            {
                ExtensionMethods.CheckGuidIsValidAndNotEmpty(UserID.ToString(), out UserID);

                using (var userManagerEntities = new UserManagerEntities())
                {
                    var al = new DAL.AuditLogin
                    {
                        ID = Guid.NewGuid(),
                        Accessed = Accessed,
                        Username = Username,
                        UserID = UserID,
                        MessageInfo = Message,
                        DateAndTime = DateTime.Now,
                        IP = ExtensionMethods.GetUserConnectionIP()
                    };

                    userManagerEntities.AuditLogin.Add(al);
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DateTime GetUserLastAccessedDateAndTime(Guid UserID)
        {
            try
            {
                using (var userManagerEntities = new UserManagerEntities())
                {
                    DateTime LastAccessDateAndTime = (from al in userManagerEntities.AuditLogin
                                                       where al.UserID == UserID
                                                       orderby al.DateAndTime descending
                                                       select al.DateAndTime).FirstOrDefault();

                    return LastAccessDateAndTime;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
