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
            LoggedUser.Role = User.Roles_Users.Select(s => (enRole)s.Roles.IdRole).FirstOrDefault();
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
        public ResponseModel Login(string username, string password, bool rememberMe, bool remoteAuthentication = false)
        {
            string message;
            bool result = false;

            // Controllo delle credenziali
            var loginServices = new LoginServices();
            if (remoteAuthentication)
                result = loginServices.ManageLoginUserWithoutPassword(username, out message, true);
            else
                result = loginServices.LoginUserWithEncryptedPassword(username, password, out message, true);

            if (result)
            {
                UserModel User = new AccountService().GetLoggedUser();
                string userId = User.ID.Adapt<string>();
                string serializedUser = JsonConvert.SerializeObject(User);

                var authTicket = new FormsAuthenticationTicket(
                    1, // version
                    userId, // user name or user id
                    DateTime.Now, // created
                    DateTime.Now.AddMinutes(120), // expires
                    rememberMe, // persistent?
                    serializedUser // can be used to store user data
                    );

                var encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                HttpContext.Current.Response.Cookies.Add(authCookie);
            }

            ResponseModel response = new ResponseModel
            {
                Result = result,
                Message = message
            };

            return response;
        }

        public static UserModel LoginApi(string username, string password)
        {
            string message;

            var loginServices = new LoginServices();
            if (loginServices.LoginUserWithEncryptedPassword(username, password, out message, true))
                return new AccountService().GetLoggedUser();

            return null;
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
