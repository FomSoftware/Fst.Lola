using System;
using System.Collections.Generic;

namespace UserManager.Service
{
    public interface IUsersRoles
    {
        List<FomMonitoringCore.SqlServer.Roles_Users> GetUsersRoles(Guid UserID);

        List<FomMonitoringCore.SqlServer.Roles_Users> GetUsersRoles(FomMonitoringCore.SqlServer.FomMonitoringEntities userManagerEntities, Guid UserID);

        void DeleteUserRoles(Guid ID);

    }
}
