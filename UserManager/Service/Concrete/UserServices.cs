using System;
using System.Collections.Generic;
using FomMonitoringCore.SqlServer;
using UserManager.Gateway.Concrete;
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

        public Users GetUser(Guid userId)
        {
            return _usersGateway.GetUser(userId);
        }

        public Users GetUser(string username)
        {
            return _usersGateway.GetUser(username);
        }

        public Users GetUser(string username, string password)
        {
            return _usersGateway.GetUser(username, password);
        }

        public List<Users> GetAllUsers()
        {
            return _usersGateway.GetAllUsers();
        }

        public List<Users> GetListOfNonDeletedUsers(FomMonitoringEntities modelloEntity)
        {
            return _usersGateway.GetListOfNonDeletedUsers(modelloEntity);
        }
       
  
        public List<Users> GetAllUsers(Framework.Common.Enumerators.UserRole userRole)
        {
            return _usersGateway.GetAllUsers(userRole);
        }

        public List<Users> GetAllUsers(string userRoleName)
        {
            return _usersGateway.GetAllUsers(userRoleName);
        }


        public string GetDotNetCultureFromUserID(Guid userId)
        {
            return _usersGateway.GetDotNetCultureFromUserId(userId);
        }

        public void CreateUser(Users user)
        {
            _usersGateway.CreateUser(user);
        }

        public void ModifyUser(Users user)
        {
            _usersGateway.ModifyUser(user);
        }

        public void DeleteUser(Users user)
        {
            _usersGateway.DeleteUser(user);
        }

        public void DeleteUser(Guid userId)
        {
            _usersGateway.DeleteUser(userId);
        }

 

    }
}
