using System;
using System.Web;
using System.Web.Security;
using UserManager.Framework.Common;
using UserManager.Framework.Model;
using UserManager.Gateway.Concrete;

namespace UserManager.Service.Concrete
{
    public class LoginServices : ILoginServices
    {
        private readonly IUserServices _userService;
        private readonly IUsers _usersGateway;

        public LoginServices(IUserServices userServices, IUsers usersGateway)
        {
            _userService = userServices;
            _usersGateway = usersGateway;
        }

        #region Private Variables
        private const string EncryptKey = "RedWhiteBlueGreenYellow";
        #endregion

        #region Public Methods
        /**
         * Funzione per effettuare il login dell'utente
         */
        public bool LoginUser(string username, string password, out string message)
        {
            return LoginUser(username, password, out message, true);
        }
        public bool LoginUser(string username, string password, out string message, bool persistUserObject)
        {
            return LoginUser(username, password, "", out message, persistUserObject);
        }
        public bool LoginUser(string username, string password, string domain, out string message, bool persistUserObject)
        {
            message = string.Empty;

            //Controllo che non siano vuote username e password
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                return ManageLoginUser(username, password, domain, out message, persistUserObject);

            message = "Username o Password sono stati inseriti vuoti. Si prega di controllare";
            return false;

        }

        /**
         * Funzione per effettuare il login dell'utente criptando la password
         */
        public bool LoginUserWithEncryptedPassword(string username, string password, out string message)
        {
            return LoginUserWithEncryptedPassword(username, password, out message, true);
        }
        public bool LoginUserWithEncryptedPassword(string username, string password, out string message, bool persistUserObject)
        {
            return LoginUserWithEncryptedPassword(username, password, "", out message, persistUserObject);
        }
        public bool LoginUserWithEncryptedPassword(string username, string password, string domain, out string message, bool persistUserObject)
        {
            message = string.Empty;

            //Controllo che non siano vuote username e password
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                message = "Username o Password sono stati inseriti vuoti. Si prega di controllare";
                return false;
            }

            // Crypt password and perform authentication
            //return ManageLoginUser(Username, DecryptPassword(Password), Domain, out Message, PersistUserObject);
            return ManageLoginUser(username, EncryptPassword(password), domain, out message, persistUserObject);
        }

        /**
         * Effettua il login dell'utente senza passargli la password
         */
        public bool ManageLoginUserWithoutPassword(string username, out string message, bool persistUserObject)
        {
            if (_usersGateway.LoginUserWithoutPassword(username, out message, out var user))
            {
                if (persistUserObject) { SessionsVariables.SetLoggedUser(user); }
                InsertLogAuditRecord(true, user.Username, user, message);
                return true;
            }

            InsertLogAuditRecord(false, username, user, message);
            return false;
        }
        public bool ManageLoginUserWithoutPassword(string username, string domain, out string message, bool persistUserObject)
        {
            if (_usersGateway.LoginUserWithoutPassword(username, domain, out message, out var user))
            {
                if (persistUserObject) { SessionsVariables.SetLoggedUser(user); }
                InsertLogAuditRecord(true, user.Username, user, message);
                return true;
            }

            InsertLogAuditRecord(false, username, user, message);
            return false;
        }
        public bool ManageLoginUserWithoutPassword(Guid userId, out string message, bool persistUserObject)
        {
            if (_usersGateway.LoginUserWithoutPassword(userId, out message, out var user))
            {
                if (persistUserObject) { SessionsVariables.SetLoggedUser(user); }
                InsertLogAuditRecord(true, user.Username, user, message);
                return true;
            }

            InsertLogAuditRecord(false, userId.ToString(), user, message);
            return false;
        }

        /**
         * Inserisce il record di audit login
         */
        private void InsertLogAuditRecord(bool accessed, string username, FomMonitoringCore.SqlServer.Users user, string message)
        {
            var userId = Guid.Empty;
            if (null != user) { userId = user.ID; }
            AuditLogin.InsertAuditLogin(accessed, username, userId, message);
        }

        public string GetUserDefaultHomePage()
        {
            ILoggedUserServices myLoggedUserServices = new LoggedUserServices();
            return myLoggedUserServices.GetLoggedUserDefualtHomePage();
        }

        public void RedirectUserToDefaultHomePage()
        {
            var homePage = GetUserDefaultHomePage();
            if (!string.IsNullOrEmpty(homePage))
            {
                HttpContext.Current.Response.Redirect(homePage);
            }
        }

        public void RedirectUserToLoginPage()
        {
            FormsAuthentication.RedirectToLoginPage();
        }

        public bool UserObjectIsInSession()
        {
            var user = SessionsVariables.GetLoggedUser();
            return null != user;
        }


        public bool CheckUserAuthentication()
        {
            //Controllo se l'utente è autenticato
            var isAuthenticated = HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated;

            if (!isAuthenticated)
            {
                return false;
            }
            

            if (UserObjectIsInSession())
                return true;

            var sUserId = HttpContext.Current.User.Identity.Name;

            return Guid.TryParse(sUserId, out var gUserId) && ManageLoginUserWithoutPassword(gUserId, out _, true);

        }

        public void LogoutUser(string fromToken, bool performRedirectToLoginPage = true)
        {
            var user = new FomMonitoringCore.SqlServer.Users();

            //Esce dalla forms authentication
            try
            {
                user = SessionsVariables.GetLoggedUser();

                //Esce dalla forms authentication
                FormsAuthentication.SignOut();
                if (user?.Username != null)
                {
                    InsertLogAuditRecord(true, user.Username, user, "Logout done succesfully");
                }
            }
            catch (Exception ex)
            {
                if (user?.Username != null)
                {
                    InsertLogAuditRecord(true, user.Username, user, "Logout error");
                }
            }

            //Rimuove eventualmente il coockie
            HttpContext.Current.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);

            //Pulisce la sessione
            SessionsVariables.ClearSession();

            //Gestione Redirect
            //ExtensionMethods.CheckQueryStringAndRedirect(FromToken);
            if (performRedirectToLoginPage) FormsAuthentication.RedirectToLoginPage(fromToken);
        }

        public bool CheckIfUsernameAlreadyExist(string username)
        {
            return _usersGateway.CheckIfUsernameAlreadyExist(username);
        }

        public string EncryptPassword(string psw)
        {
            try
            {
                var wrapper = new Encrypter(EncryptKey);
                var cipherText = wrapper.EncryptData(psw);
                return cipherText;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the encryption: {ex.Message}");
            }
        }

        public string DecryptPassword(string encryptedPsw)
        {
            //DecryptData throws if the wrong password is used.
            var wrapper = new Encrypter(EncryptKey);
            try
            {
                var plainPsw = wrapper.DecryptData(encryptedPsw);
                return plainPsw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the decryption: {ex.Message}");
            }
        }
        #endregion

        public bool IsFirstLogin()
        {
                var service = new LoggedUserServices();
                var user = service.GetLoggedUser();
                var realUser = _userService.GetUser(user.Username);

                return realUser.LastDateUpdatePassword == null;
        }

        public bool IsPasswordExpired()
        {
            var service = new LoggedUserServices();
            var user = service.GetLoggedUser();

            if (user.LastDateUpdatePassword == null) return false;

            var days = (DateTime.Now - (DateTime)user.LastDateUpdatePassword).Days;

            return days >= 90;
        }

        #region Private Methods
 

        private bool ManageLoginUser(string username, string password, string domain, out string message, bool persistUserObject)
        {
            if (_usersGateway.LoginUser(username, password, domain, out message, out var user))
            {
                if (persistUserObject) { SessionsVariables.SetLoggedUser(user); }
                InsertLogAuditRecord(true, user.Username, user, message);
                return true;
            }

            InsertLogAuditRecord(false, username, user, message);
            return false;
        }
        #endregion
    }
}
