using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserManager.DAL;

namespace UserManager.Service
{
  public interface IRolesService
    {
            List<UserManager.DAL.Roles> GetListOfNonDeletedRoles(DAL.UserManagerEntities ModelloEntity);

            List<DAL.Roles> GetRoles();

            void ModifyRole(UserManager.DAL.Roles role);

            void DeleteRole(Guid RoleID);

            DAL.Roles GetRole(Guid RoleID);

            bool CheckUserRoles(Guid UserID, int IdRole);

            bool CheckUserRoles(Guid UserID, Roles role);
        
            List<DAL.Roles> GetUserRoles(Guid UserID);

            List<DAL.Roles> GetUserRoles(UserManagerEntities userManagerEntities, Guid UserID);

            void AddGroupToUser(Guid UserID, Guid RoleID);

            void AddGroupsToUser(Guid UserID, List<Guid> RolesID);

            void DeleteRoles(Guid UserID);
     }
}

