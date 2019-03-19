using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserManager.Service.Concrete
{
    public class UserServices : IUserServices
    {

        public bool ChangePassword(Guid UserID, string NewPassword, string OldPassword, out string Message)
        {
            if (UserID == Guid.Empty)
            {
                Message = "Utente non loggato correttamente! Provare ad eseguire il logout e rientrare nell'applicazione!";
                return false;
            }

            return DAL.Gateway.Concrete.Users.ChangePassword(UserID, NewPassword, OldPassword, out Message);
        }

        public DAL.Users GetUser(Guid UserID)
        {
            return DAL.Gateway.Concrete.Users.GetUser(UserID);
        }

        public DAL.Users GetUser(string Username)
        {
            return DAL.Gateway.Concrete.Users.GetUser(Username);
        }

        public DAL.Users GetUser(string Username, string Password)
        {
            return DAL.Gateway.Concrete.Users.GetUser(Username, Password);
        }

        public List<DAL.Users> GetAllUsers()
        {
            return DAL.Gateway.Concrete.Users.GetAllUsers();
        }

        public List<DAL.Users> GetListOfNonDeletedUsers(DAL.UserManagerEntities ModelloEntity)
        {
            return DAL.Gateway.Concrete.Users.GetListOfNonDeletedUsers(ModelloEntity);
        }
       
  
        public List<DAL.Users> GetAllUsers(UserManager.Framework.Common.Enumerators.UserRole UserRole)
        {
            return DAL.Gateway.Concrete.Users.GetAllUsers(UserRole);
        }

        public List<DAL.Users> GetAllUsers(string UserRoleName)
        {
            return DAL.Gateway.Concrete.Users.GetAllUsers(UserRoleName);
        }


        public string GetDotNetCultureFromUserID(Guid UserID)
        {
            return DAL.Gateway.Concrete.Users.GetDotNetCultureFromUserID(UserID);
        }

        public void CreateUser(DAL.Users User)
        {
            DAL.Gateway.Concrete.Users.CreateUser(User);
        }

        public void ModifyUser(DAL.Users User)
        {
            DAL.Gateway.Concrete.Users.ModifyUser(User);
        }

        public void DeleteUser(DAL.Users User)
        {
            DAL.Gateway.Concrete.Users.DeleteUser(User);
        }

        public void DeleteUser(Guid UserID)
        {
            DAL.Gateway.Concrete.Users.DeleteUser(UserID);
        }

 

    }
}
