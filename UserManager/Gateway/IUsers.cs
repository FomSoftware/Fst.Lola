using System;
using System.Collections.Generic;
using FomMonitoringCore.SqlServer;
using UserManager.Framework.Common;

namespace UserManager.Gateway
{
    public interface IUsers
    {
        bool LoginUser(string username, string password, string domain, out string message, out FomMonitoringCore.SqlServer.Users user);
        bool LoginUserWithoutPassword(string username, out string message, out FomMonitoringCore.SqlServer.Users user);
        bool LoginUserWithoutPassword(Guid userId, out string message, out FomMonitoringCore.SqlServer.Users user);
        bool ChangePassword(Guid userId, string newPassword, string oldPassword, out string message);
        FomMonitoringCore.SqlServer.Users GetUser(string username);
        FomMonitoringCore.SqlServer.Users GetUserById(Guid Id);




    }
}