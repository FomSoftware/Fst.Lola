using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.SqlServer;

namespace UserManager.Service.Concrete
{
    public class RoleService : IRolesService
    {
        public List<Roles> GetListOfNonDeletedRoles(FomMonitoringEntities modelloEntity)
        {
            return Gateway.Concrete.Roles.GetListOfNonDeletedRoles(modelloEntity);

        }

        public void ModifyRole(Roles role)
        {
            Gateway.Concrete.Roles.ModifyRole(role);
        }

        public void DeleteRole(Guid roleId)
        {
            Gateway.Concrete.Roles.DeleteRole(roleId);
        }

        public List<Roles> GetRoles()
        {
            return Gateway.Concrete.Roles.GetRoles();
        }

        public Roles GetRole(Guid rolesId)
        {
            return Gateway.Concrete.Roles.GetRoles(rolesId);
        }

        public bool CheckUserRoles(Guid userId, int idRole)
        {
            var userRoles = GetUserRoles(userId);
            return userRoles.Any(w => w.IdRole == idRole);
        }

        public bool CheckUserRoles(Guid userId, Roles role)
        {
            var userRoles = GetUserRoles(userId);
            return (userRoles.Contains(role)) ? true : false;
        }

        public List<Roles> GetUserRoles(Guid userId)
        {
            return Gateway.Concrete.Roles.GetRolesFromUserId(userId);
        }

        public List<Roles> GetUserRoles(FomMonitoringEntities userManagerEntities, Guid userId)
        {
            return Gateway.Concrete.Roles.GetRolesFromUserId(userManagerEntities, userId);
        }

        public void AddGroupToUser(Guid userId, Guid roleId)
        {
            Gateway.Concrete.Roles.AddRolesToUser(userId, new List<Guid>() { roleId });
        }
        public void DeleteRoles(Guid userId)
        {
            Gateway.Concrete.Roles.DeleteRoles(userId);
        }

        public void AddGroupsToUser(Guid userId, List<Guid> rolesId)
        {
            Gateway.Concrete.Roles.AddRolesToUser(userId, rolesId);
        }
    }
}
