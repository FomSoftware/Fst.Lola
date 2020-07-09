using System;
using System.Collections.Generic;

namespace UserManager.Service
{
    public interface IRolesService
    {
            List<FomMonitoringCore.SqlServer.Roles> GetListOfNonDeletedRoles();

            List<FomMonitoringCore.SqlServer.Roles> GetRoles();

            void ModifyRole(FomMonitoringCore.SqlServer.Roles role);

            void DeleteRole(Guid roleId);

            FomMonitoringCore.SqlServer.Roles GetRole(Guid roleId);

            bool CheckUserRoles(Guid userId, int idRole);

            bool CheckUserRoles(Guid userId, FomMonitoringCore.SqlServer.Roles role);
        
            List<FomMonitoringCore.SqlServer.Roles> GetUserRoles(Guid userId);

            void AddGroupToUser(Guid userId, Guid roleId);

            void AddGroupsToUser(Guid userId, List<Guid> rolesId);

            void DeleteRoles(Guid userId);
     }
}

