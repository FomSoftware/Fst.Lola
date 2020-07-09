using System;
using System.Collections.Generic;
using FomMonitoringCore.SqlServer;

namespace UserManager.Service.Concrete
{
    public class UserServices : IUserServices
    {

        public bool ChangePassword(Guid userId, string newPassword, string oldPassword, out string message)
        {
            if (userId == Guid.Empty)
            {
                message = "Utente non loggato correttamente! Provare ad eseguire il logout e rientrare nell'applicazione!";
                return false;
            }

            return Gateway.Concrete.Users.ChangePassword(userId, newPassword, oldPassword, out message);
        }

        public Users GetUser(Guid userId)
        {
            return Gateway.Concrete.Users.GetUser(userId);
        }

        public Users GetUser(string username)
        {
            return Gateway.Concrete.Users.GetUser(username);
        }

        public Users GetUser(string username, string password)
        {
            return Gateway.Concrete.Users.GetUser(username, password);
        }

        public List<Users> GetAllUsers()
        {
            return Gateway.Concrete.Users.GetAllUsers();
        }

        public List<Users> GetListOfNonDeletedUsers(FomMonitoringEntities modelloEntity)
        {
            return Gateway.Concrete.Users.GetListOfNonDeletedUsers(modelloEntity);
        }
       
  
        public List<Users> GetAllUsers(Framework.Common.Enumerators.UserRole userRole)
        {
            return Gateway.Concrete.Users.GetAllUsers(userRole);
        }

        public List<Users> GetAllUsers(string userRoleName)
        {
            return Gateway.Concrete.Users.GetAllUsers(userRoleName);
        }


        public string GetDotNetCultureFromUserID(Guid userId)
        {
            return Gateway.Concrete.Users.GetDotNetCultureFromUserId(userId);
        }

        public void CreateUser(Users user)
        {
            Gateway.Concrete.Users.CreateUser(user);
        }

        public void ModifyUser(Users user)
        {
            Gateway.Concrete.Users.ModifyUser(user);
        }

        public void DeleteUser(Users user)
        {
            Gateway.Concrete.Users.DeleteUser(user);
        }

        public void DeleteUser(Guid userId)
        {
            Gateway.Concrete.Users.DeleteUser(userId);
        }

 

    }
}
