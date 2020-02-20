using System;
using System.Collections.Generic;
using System.Linq;
using UserManager.DAL;

namespace UserManager.Service.Concrete
{
    public class RoleService : IRolesService
    {
        public List<UserManager.DAL.Roles> GetListOfNonDeletedRoles(DAL.UserManagerEntities ModelloEntity)
        {
            return DAL.Gateway.Concrete.Roles.GetListOfNonDeletedRoles(ModelloEntity);

        }

        public void ModifyRole(DAL.Roles Role)
        {
            DAL.Gateway.Concrete.Roles.ModifyRole(Role);
        }

        public void DeleteRole(Guid RoleID)
        {
            DAL.Gateway.Concrete.Roles.DeleteRole(RoleID);
        }

        public List<DAL.Roles> GetRoles()
        {
            return DAL.Gateway.Concrete.Roles.GetRoles();
        }

        public DAL.Roles GetRole(Guid RolesID)
        {
            return DAL.Gateway.Concrete.Roles.GetRoles(RolesID);
        }

        public bool CheckUserRoles(Guid UserID, int IdRole)
        {
            List<Roles> userRoles = GetUserRoles(UserID);
            return userRoles.Where(w => w.IdRole == IdRole).Any();
        }

        public bool CheckUserRoles(Guid UserID, Roles role)
        {
            List<Roles> userRoles = GetUserRoles(UserID);
            return (userRoles.Contains(role)) ? true : false;
        }

        public List<Roles> GetUserRoles(Guid UserID)
        {
            return DAL.Gateway.Concrete.Roles.GetRolesFromUserID(UserID);
        }

        public List<Roles> GetUserRoles(UserManagerEntities userManagerEntities, Guid UserID)
        {
            return DAL.Gateway.Concrete.Roles.GetRolesFromUserID(userManagerEntities, UserID);
        }

        public void AddGroupToUser(Guid UserID, Guid RoleID)
        {
            DAL.Gateway.Concrete.Roles.AddRolesToUser(UserID, new List<Guid>() { RoleID });
        }
        public void DeleteRoles(Guid UserID)
        {
            DAL.Gateway.Concrete.Roles.DeleteRoles(UserID);
        }

        public void AddGroupsToUser(Guid UserID, List<Guid> RolesID)
        {
            DAL.Gateway.Concrete.Roles.AddRolesToUser(UserID, RolesID);
        }
    }
}
