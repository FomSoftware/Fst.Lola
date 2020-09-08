using System;
using System.Web;
using System.Web.Security;
using UserManager.Framework.Common;
using UserManager.Framework.Model;
using UserManager.Gateway;

namespace UserManager.Service.Concrete
{
    public class LoginServices : ILoginServices
    {
        private readonly IUserServices _userService;
        private readonly IUsers _usersGateway;
        private readonly IAuditLogin _auditLogin;
        private readonly ILoggedUserServices _loggedUserServices;

        public LoginServices(IUserServices userServices, IUsers usersGateway, IAuditLogin auditLogin, ILoggedUserServices loggedUserServices)
        {
            _userService = userServices;
            _usersGateway = usersGateway;
            _auditLogin = auditLogin;
            _loggedUserServices = loggedUserServices;
        }

        #region Private Variables
        private const string EncryptKey = "RedWhiteBlueGreenYellow";
        #endregion

        #region Public Methods

         


        /**
         * Funzione per effettuare il login dell'utente criptando la password
         */

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


        /**
         * Inserisce il record di audit login
         */
        private void InsertLogAuditRecord(bool accessed, string username, FomMonitoringCore.SqlServer.Users user, string message)
        {
            var userId = Guid.Empty;
            if (null != user) { userId = user.ID; }
            _auditLogin.InsertAuditLogin(accessed, username, userId, message);
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
                var user = _loggedUserServices.GetLoggedUser();
                var realUser = _userService.GetUser(user.Username);

                return realUser.LastDateUpdatePassword == null;
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
