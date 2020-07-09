using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.SqlServer;
using UserManager.Gateway;
using Roles = FomMonitoringCore.SqlServer.Roles;

namespace UserManager.Service.Concrete
{
    public class RoleService : IRolesService
    {
        private IRoles _rolesGateway;

        public RoleService(IRoles rolesGateway)
        {
            _rolesGateway = rolesGateway;
        }
        public List<Roles> GetListOfNonDeletedRoles()
        {
            return _rolesGateway.GetListOfNonDeletedRoles();

        }

        public void ModifyRole(Roles role)
        {
            _rolesGateway.ModifyRole(role);
        }

        public void DeleteRole(Guid roleId)
        {
            _rolesGateway.DeleteRole(roleId);
        }

        public List<Roles> GetRoles()
        {
            return _rolesGateway.GetRoles();
        }

        public Roles GetRole(Guid rolesId)
        {
            return _rolesGateway.GetRoles(rolesId);
        }

        public bool CheckUserRoles(Guid userId, int idRole)
        {
            var userRoles = GetUserRoles(userId);
            return userRoles.Any(w => w.IdRole == idRole);
        }

        public bool CheckUserRoles(Guid userId, Roles role)
        {
            var userRoles = GetUserRoles(userId);
            return userRoles.Contains(role);
        }

        public List<Roles> GetUserRoles(Guid userId)
        {
            return _rolesGateway.GetRolesFromUserId(userId);
        }


        public void AddGroupToUser(Guid userId, Guid roleId)
        {
            _rolesGateway.AddRolesToUser(userId, new List<Guid>() { roleId });
        }
        public void DeleteRoles(Guid userId)
        {
            _rolesGateway.DeleteRoles(userId);
        }

        public void AddGroupsToUser(Guid userId, List<Guid> rolesId)
        {
            _rolesGateway.AddRolesToUser(userId, rolesId);
        }
    }
}
