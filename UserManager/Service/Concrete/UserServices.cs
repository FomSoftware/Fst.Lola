using System;
using System.Collections.Generic;
using FomMonitoringCore.SqlServer;
using UserManager.Gateway;
using Users = FomMonitoringCore.SqlServer.Users;

namespace UserManager.Service.Concrete
{
    public class UserServices : IUserServices
    {
        private readonly IUsers _usersGateway;

        public UserServices(IUsers usersGateway)
        {
            _usersGateway = usersGateway;
        }
        public bool ChangePassword(Guid userId, string newPassword, string oldPassword, out string message)
        {
            if (userId == Guid.Empty)
            {
                message = "Utente non loggato correttamente! Provare ad eseguire il logout e rientrare nell'applicazione!";
                return false;
            }

            return _usersGateway.ChangePassword(userId, newPassword, oldPassword, out message);
        }

        public Users GetUser(string username)
        {
            return _usersGateway.GetUser(username);
        }
        

    }
}
