using System;
using System.Collections.Generic;

namespace UserManager.Gateway
{
    public interface IRoles
    {
        List<FomMonitoringCore.SqlServer.Roles> GetListOfNonDeletedRoles();
        void DeleteRole(Guid roleId);
        bool CheckIfRoleAlreadyExist(string name);

        /// <summary>
        ///     Modifica alcune proprietà dell'utente
        /// </summary>
        /// <param name="role"></param>
        void ModifyRole(FomMonitoringCore.SqlServer.Roles role);

        FomMonitoringCore.SqlServer.Roles GetRoles(Guid rolesId);
        List<FomMonitoringCore.SqlServer.Roles> GetRoles();
        List<FomMonitoringCore.SqlServer.Roles> GetRolesFromUserId(Guid userId);
        void DeleteRoles(Guid userId);
        void AddRolesToUser(Guid userId, List<Guid> rolesId);
        void DeleteRolesFromUser(Guid userId);
    }
}