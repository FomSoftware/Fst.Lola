using System;
using System.Collections.Generic;

namespace UserManager.Service.Concrete
{
  public  class AuditLoginServices : IAuditLoginServices
    {
      public List<DAL.AuditLogin> GetAuditLogin()
      {
          return DAL.Gateway.Concrete.AuditLogin.GetAuditLogin();
      }

      public List<UserManager.DAL.AuditLogin> GetListOfAuditLogin(DAL.UserManagerEntities ModelloEntity)
      {
          return DAL.Gateway.Concrete.AuditLogin.GetListOfAuditLogin(ModelloEntity);

      }
      public DateTime GetUserLastAccessedDateAndTime(Guid UserID)
      {
          return DAL.Gateway.Concrete.AuditLogin.GetUserLastAccessedDateAndTime(UserID);
      }
    }
}
