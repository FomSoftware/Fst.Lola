using System;
using System.Collections.Generic;
using UserManager.Gateway.Concrete;

namespace UserManager.Service.Concrete
{
    public class UsersRolesServices : IUsersRoles
    {
        public List<FomMonitoringCore.SqlServer.Roles_Users> GetUsersRoles(Guid userId)
        {
            return UsersRoles.GetUsersRoles(userId);
        }

        public List<FomMonitoringCore.SqlServer.Roles_Users> GetUsersRoles()
        {
            return UsersRoles.GetUsersRoles();
        }

        public List<FomMonitoringCore.SqlServer.Roles_Users> GetUsersRoles(FomMonitoringCore.SqlServer.FomMonitoringEntities userManagerEntities, Guid userId)
        {
            return UsersRoles.GetUsersRoles(userManagerEntities, userId);
        }

        public void DeleteUserRoles(Guid ID)
        {
            UsersRoles.DeleteUserRoles(ID);
        }

    }
}
