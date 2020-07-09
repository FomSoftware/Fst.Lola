using System;
using System.Collections.Generic;
using FomMonitoringCore.SqlServer;
using UserManager.Framework.Common;

namespace UserManager.Gateway.Concrete
{
    public interface IUsers
    {
        bool LoginUser(string username, string password, string domain, out string message, out FomMonitoringCore.SqlServer.Users user);
        bool LoginUserWithoutPassword(string username, out string message, out FomMonitoringCore.SqlServer.Users user);
        bool LoginUserWithoutPassword(string username, string domain, out string message, out FomMonitoringCore.SqlServer.Users user);
        bool LoginUserWithoutPassword(Guid userId, out string message, out FomMonitoringCore.SqlServer.Users user);
        bool ChangePassword(Guid userId, string newPassword, string oldPassword, out string message);
        FomMonitoringCore.SqlServer.Users GetUser(Guid userId);
        FomMonitoringCore.SqlServer.Users GetUser(string username);
        FomMonitoringCore.SqlServer.Users GetUser(string username, string password);
        List<FomMonitoringCore.SqlServer.Users> GetAllUsers();
        List<FomMonitoringCore.SqlServer.Users> GetListOfNonDeletedUsers(FomMonitoringEntities fomMonitoringEntities);
        List<FomMonitoringCore.SqlServer.Users> GetAllUsers(Enumerators.UserRole userRole);
        List<FomMonitoringCore.SqlServer.Users> GetAllUsers(string userRoleName);
        string GetDotNetCultureFromUserId(Guid userId);
        bool CheckIfUsernameAlreadyExist(string username);

        /// <summary>
        /// Crea un nuovo utente nel database
        /// </summary>
        /// <param name="user"></param>
        void CreateUser(FomMonitoringCore.SqlServer.Users user);

        /// <summary>
        /// Modifica alcune proprietà dell'utente
        /// </summary>
        /// <param name="user"></param>
        void ModifyUser(FomMonitoringCore.SqlServer.Users user);

        /// <summary>
        /// Disabilita un utente
        /// </summary>
        /// <param name="userId"></param>
        void DeleteUser(Guid userId);

        /// <summary>
        /// Disabilita un utente
        /// </summary>
        /// <param name="user"></param>
        void DeleteUser(FomMonitoringCore.SqlServer.Users user);
    }
}