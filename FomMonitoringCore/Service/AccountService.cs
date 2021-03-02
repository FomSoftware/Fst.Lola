using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using UserManager.Service;
using UserManager.Service.Concrete;

namespace FomMonitoringCore.Service
{
    public class AccountService : IAccountService
    {
        private readonly ILoginServices _loginServices;
        private readonly IUserServices _userServices;
        private readonly ILoggedUserServices _loggedUserServices;

        public AccountService(ILoginServices loginServices, IUserServices userServices, ILoggedUserServices loggedUserServices)
        {
            _loginServices = loginServices;
            _userServices = userServices;
            _loggedUserServices = loggedUserServices;
        }

        /// <summary>
        /// Get logged user.
        /// </summary>
        /// <returns></returns>
        public UserModel GetLoggedUser()
        {
            var LoggedUser = new UserModel();
            
            var User = _loggedUserServices.GetLoggedUser();

            if (!User.Enabled)
                return null;

            LoggedUser.ID = User.ID;
            LoggedUser.Username = User.Username;
            LoggedUser.FirstName = User.FirstName;
            LoggedUser.LastName = User.LastName;
            LoggedUser.CompanyName = User.CompanyName;
            LoggedUser.Email = User.Email;
            LoggedUser.TimeZone = User.TimeZone;
            LoggedUser.Role = User.Roles_Users.Select(s => (enRole)s.Roles.IdRole).FirstOrDefault();
            LoggedUser.Language = User.Languages != null ? new LanguagesModel {
                DotNetCulture = User.Languages.DotNetCulture,
                ID = User.Languages.ID,
                IdLanguage = User.Languages.IdLanguage,
                InitialsLanguage = User.Languages.InitialsLanguage,
                Name = User.Languages.Name
            } : null;
            LoggedUser.LastDateUpdatePassword = User.LastDateUpdatePassword;
            LoggedUser.TimeZone = User.TimeZone;
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
            var result = false;

            // Controllo delle credenziali
            if (remoteAuthentication)
                result = _loginServices.ManageLoginUserWithoutPassword(username, out message, true);
            else
                result = _loginServices.LoginUserWithEncryptedPassword(username, password, out message, true);

            if (result)
            {
                var User = GetLoggedUser();                
                var userId = User.ID.Adapt<string>();
                var serializedUser = JsonConvert.SerializeObject(User, Formatting.Indented, 
                    new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    MaxDepth = 1
            }
                );

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

            var response = new ResponseModel
            {
                Result = result,
                Message = message    
            };

            return response;
        }

        public bool LoginApi(string username, string password)
        {
            return _loginServices.LoginUserWithEncryptedPassword(username, password, out _, false) && _userServices.GetUser(username).Roles_Users.Any(a => a.Roles.IdRole == (int)enRole.UserApi);
        }

        /// <summary>
        /// Logouts the user.
        /// </summary>
        public void Logout()
        {
            _loginServices.LogoutUser(string.Empty, false);
        }
    }
}
