using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserManager.DAL;

namespace UserManager.Service.Concrete
{
    public class AdministrativeServices : IAdministrativeServices
    {

        public List<DAL.vw_UserInfo> GetListOfUserInfo()
        {
            using (UserManagerEntities userManagerEntities = new UserManagerEntities())
            {
                return (from u in userManagerEntities.vw_UserInfo select u).ToList();
            }
        }

        public List<Languages> GetLanguages()
        {
            return DAL.Gateway.Concrete.Languages.GetLanguages();
        }
    }
}
