using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager.Service
{
   public interface IAuditLoginServices
    {
        List<DAL.AuditLogin> GetAuditLogin();
        List<UserManager.DAL.AuditLogin> GetListOfAuditLogin(DAL.UserManagerEntities ModelloEntity);
        DateTime GetUserLastAccessedDateAndTime(Guid UserID);
    }
}
