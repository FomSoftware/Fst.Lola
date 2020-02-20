using System;
using System.Collections.Generic;
using UserManager.DAL;

namespace UserManager.Service
{
    public interface IUsersRoles
    {
        List<DAL.Roles_Users> GetUsersRoles(Guid UserID);

        List<DAL.Roles_Users> GetUsersRoles(UserManagerEntities userManagerEntities, Guid UserID);

        void DeleteUserRoles(Guid ID);

    }
}
