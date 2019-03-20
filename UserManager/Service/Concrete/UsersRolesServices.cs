using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.DAL;

namespace UserManager.Service.Concrete
{
    public class UsersRolesServices : IUsersRoles
    {
        public List<DAL.Roles_Users> GetUsersRoles(Guid UserID)
        {
            return DAL.Gateway.Concrete.UsersRoles.GetUsersRoles(UserID);
        }

        public List<DAL.Roles_Users> GetUsersRoles()
        {
            return DAL.Gateway.Concrete.UsersRoles.GetUsersRoles();
        }

        public List<DAL.Roles_Users> GetUsersRoles(UserManagerEntities userManagerEntities, Guid UserID)
        {
            return DAL.Gateway.Concrete.UsersRoles.GetUsersRoles(userManagerEntities, UserID);
        }

        public void DeleteUserRoles(Guid ID)
        {
            DAL.Gateway.Concrete.UsersRoles.DeleteUserRoles(ID);
        }

    }
}
