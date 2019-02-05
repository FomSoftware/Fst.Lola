using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using UserManager.DAL;
using UserManager.Service.Concrete;

namespace FomMonitoringCore.Service
{
    public class AccountService
    {
        public static AccountService Init()
        {
            return new AccountService();
        }

        /// <summary>
        /// Get logged user.
        /// </summary>
        /// <returns></returns>
        public UserModel GetLoggedUser()
        {
            UserModel LoggedUser = new UserModel();

            var loggedUserServices = new LoggedUserServices();
            Users User = loggedUserServices.GetLoggedUser();

            if (!User.Enabled)
                return null;

            LoggedUser.ID = User.ID;
            LoggedUser.Username = User.Username;
            LoggedUser.FirstName = User.FirstName;
            LoggedUser.LastName = User.LastName;
            LoggedUser.Roles = User.Roles_Users.Select(s => (enRole)s.Roles.IdRole).ToList();
            LoggedUser.Language = User.Languages;

            return LoggedUser;
        }

        /// <summary>
        /// Logins the specified user name.
        /// </summary>
        /// <param name="username">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="rememberMe">The remember me.</param>
        /// <returns></returns>
        public ResponseModel Login(string username, string password, bool rememberMe)
        {
            string message;
            bool result = ValidateUser(username, password, rememberMe, out message);

            ResponseModel response = new ResponseModel
            {
                Result = result,
                Message = message
            };

            return response;
        }

        public static bool ValidateUser(string username, string password, bool persistentCookie, out string message)
        {
            // Controllo delle credenziali
            var loginServices = new LoginServices();
            if (!loginServices.LoginUser(username, password, out message))
                return false;

            Users LoggedUser = new LoggedUserServices().GetLoggedUser();
            UserModel User = new UserModel();

            User.ID = LoggedUser.ID;
            User.Username = LoggedUser.Username;
            User.FirstName = LoggedUser.FirstName;
            User.LastName = LoggedUser.LastName;
            User.Roles = LoggedUser.Roles_Users.Select(s => (enRole)s.Roles.IdRole).ToList();
            User.Language = LoggedUser.Languages;

            string userId = User.ID.Adapt<string>();
            string serializedUser = JsonConvert.SerializeObject(User);

            var authTicket = new FormsAuthenticationTicket(
                1, // version
                userId, // user name or user id
                DateTime.Now, // created
                DateTime.Now.AddMinutes(120), // expires
                persistentCookie, // persistent?
                serializedUser // can be used to store user data
                );

            var encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            HttpContext.Current.Response.Cookies.Add(authCookie);

            return true;
        }

        /// <summary>
        /// Logouts the user.
        /// </summary>
        public void Logout()
        {
            var loginServices = new LoginServices();
            loginServices.LogoutUser(string.Empty, false);
        }
    }
}
