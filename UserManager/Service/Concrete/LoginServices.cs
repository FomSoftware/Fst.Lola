using System;
using System.Web;
using System.Web.Security;
using UserManager.Framework.Common;
using UserManager.Framework.Model;

namespace UserManager.Service.Concrete
{
    public class LoginServices : ILoginServices
    {
        #region Private Variables
        private const string _EncryptKey = "RedWhiteBlueGreenYellow";
        #endregion

        #region Public Methods
        /**
         * Funzione per effettuare il login dell'utente
         */
        public bool LoginUser(string Username, string Password, out string Message)
        {
            return LoginUser(Username, Password, out Message, true);
        }
        public bool LoginUser(string Username, string Password, out string Message, bool PersistUserObject)
        {
            return LoginUser(Username, Password, "", out Message, PersistUserObject);
        }
        public bool LoginUser(string Username, string Password, string Domain, out string Message, bool PersistUserObject)
        {
            Message = string.Empty;

            //Controllo che non siano vuote username e password
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                Message = "Username o Password sono stati inseriti vuoti. Si prega di controllare";
                return false;
            }

            return ManageLoginUser(Username, Password, Domain, out Message, PersistUserObject);
        }

        /**
         * Funzione per effettuare il login dell'utente criptando la password
         */
        public bool LoginUserWithEncryptedPassword(string Username, string Password, out string Message)
        {
            return LoginUserWithEncryptedPassword(Username, Password, out Message, true);
        }
        public bool LoginUserWithEncryptedPassword(string Username, string Password, out string Message, bool PersistUserObject)
        {
            return LoginUserWithEncryptedPassword(Username, Password, "", out Message, PersistUserObject);
        }
        public bool LoginUserWithEncryptedPassword(string Username, string Password, string Domain, out string Message, bool PersistUserObject)
        {
            Message = string.Empty;

            //Controllo che non siano vuote username e password
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                Message = "Username o Password sono stati inseriti vuoti. Si prega di controllare";
                return false;
            }

            // Crypt password and perform authentication
            //return ManageLoginUser(Username, DecryptPassword(Password), Domain, out Message, PersistUserObject);
            return ManageLoginUser(Username, EncryptPassword(Password), Domain, out Message, PersistUserObject);
        }

        /**
         * Effettua il login dell'utente senza passargli la password
         */
        public bool ManageLoginUserWithoutPassword(string Username, out string Message, bool PersistUserObject)
        {
            DAL.Users User;
            if (DAL.Gateway.Concrete.Users.LoginUserWithoutPassword(Username, out Message, out User))
            {
                if (PersistUserObject) { Framework.Common.SessionsVariables.SetLoggedUser(User); }
                InsertLogAuditRecord(true, User.Username, User, Message);
                return true;
            }
            else
            {
                InsertLogAuditRecord(false, Username, User, Message);
                return false;
            }
        }
        public bool ManageLoginUserWithoutPassword(string Username, string Domain, out string Message, bool PersistUserObject)
        {
            DAL.Users User;
            if (DAL.Gateway.Concrete.Users.LoginUserWithoutPassword(Username, Domain, out Message, out User))
            {
                if (PersistUserObject) { Framework.Common.SessionsVariables.SetLoggedUser(User); }
                InsertLogAuditRecord(true, User.Username, User, Message);
                return true;
            }
            else
            {
                InsertLogAuditRecord(false, Username, User, Message);
                return false;
            }
        }
        public bool ManageLoginUserWithoutPassword(Guid UserID, out string Message, bool PersistUserObject)
        {
            DAL.Users User;
            if (DAL.Gateway.Concrete.Users.LoginUserWithoutPassword(UserID, out Message, out User))
            {
                if (PersistUserObject) { Framework.Common.SessionsVariables.SetLoggedUser(User); }
                InsertLogAuditRecord(true, User.Username, User, Message);
                return true;
            }
            else
            {
                InsertLogAuditRecord(false, UserID.ToString(), User, Message);
                return false;
            }
        }

        /**
         * Inserisce il record di audit login
         */
        private void InsertLogAuditRecord(bool Accessed, string Username, DAL.Users User, string Message)
        {
            Guid UserID = Guid.Empty;
            if (null != User) { UserID = User.ID; }
            DAL.Gateway.Concrete.AuditLogin.InsertAuditLogin(Accessed, Username, UserID, Message);
        }

        public string GetUserDefaultHomePage()
        {
            ILoggedUserServices MyLoggedUserServices = new LoggedUserServices();
            return MyLoggedUserServices.GetLoggedUserDefualtHomePage();
        }

        public void RedirectUserToDefaultHomePage()
        {
            string HomePage = GetUserDefaultHomePage();
            if (!string.IsNullOrEmpty(HomePage))
            {
                System.Web.HttpContext.Current.Response.Redirect(HomePage);
            }
        }

        public void RedirectUserToLoginPage()
        {
            FormsAuthentication.RedirectToLoginPage();
        }

        public bool UserObjectIsInSession()
        {
            DAL.Users User = Framework.Common.SessionsVariables.GetLoggedUser();
            if (null == User) { return false; }
            return true;
        }

        public bool ManageLoginUserOnRedirectAccessRequests(Guid RequestID, bool PersistUserObject)
        {
            DAL.Users User;
            string Message;

            if (DAL.Gateway.Concrete.Users.LoginUserWithRedirectAccessRequestID(RequestID, out Message, out User))
            {
                if (PersistUserObject) { Framework.Common.SessionsVariables.SetLoggedUser(User); }
                InsertLogAuditRecord(true, User.Username, User, Message);
                return true;
            }
            else
            {
                InsertLogAuditRecord(false, String.Format(RequestID.ToString()), User, Message);
                return false;
            }
        }

        public bool CheckUserAuthentication()
        {
            string Message;

            //Controllo se l'utente è autenticato
            bool IsAuthenticated = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (!IsAuthenticated)
            {
                return false;
            }
            else
            {
                ILoginServices myLoginServices = new LoginServices();

                if (!myLoginServices.UserObjectIsInSession())
                {
                    string sUserID = System.Web.HttpContext.Current.User.Identity.Name;
                    Guid gUserID = Guid.Empty;

                    if (Guid.TryParse(sUserID, out gUserID))
                    {
                        if (!myLoginServices.ManageLoginUserWithoutPassword(gUserID, out Message, true))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        public void LogoutUser(string FromToken, bool PerformRedirectToLoginPage = true)
        {
            DAL.Users User = new DAL.Users();

            //Esce dalla forms authentication
            try
            {
                User = SessionsVariables.GetLoggedUser();

                //Esce dalla forms authentication
                FormsAuthentication.SignOut();
                if (User != null && User.Username != null)
                {
                    InsertLogAuditRecord(true, User.Username, User, "Logout done succesfully");
                }
            }
            catch (Exception ex)
            {
                if (User != null && User.Username != null)
                {
                    InsertLogAuditRecord(true, User.Username, User, "Logout error");
                }
            }

            //Rimuove eventualmente il coockie
            HttpContext.Current.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);

            //Pulisce la sessione
            SessionsVariables.ClearSession();

            //Gestione Redirect
            //ExtensionMethods.CheckQueryStringAndRedirect(FromToken);
            if (PerformRedirectToLoginPage) FormsAuthentication.RedirectToLoginPage(FromToken);
        }

        public bool CheckIfUsernameAlreadyExist(string Username)
        {
            return DAL.Gateway.Concrete.Users.CheckIfUsernameAlreadyExist(Username);
        }

        public string EncryptPassword(string psw)
        {
            try
            {
                Encrypter wrapper = new Encrypter(_EncryptKey);
                string cipherText = wrapper.EncryptData(psw);
                return cipherText;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the encryption: {0}", ex.Message));
            }
        }

        public string DecryptPassword(string EncryptedPsw)
        {
            //DecryptData throws if the wrong password is used.
            Encrypter wrapper = new Encrypter(_EncryptKey);
            try
            {
                string plainPsw = wrapper.DecryptData(EncryptedPsw);
                return plainPsw;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the decryption: {0}", ex.Message));
            }
        }
        #endregion

        public bool IsFirstLogin()
        {
            try
            {
                LoggedUserServices service = new LoggedUserServices();
                DAL.Users User = service.GetLoggedUser();
                var realUser = new UserServices().GetUser(User.Username);

                if (realUser.LastDateUpdatePassword == null) return true;
                else return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool IsPasswordExpired()
        {
            try
            {
                LoggedUserServices service = new LoggedUserServices();
                DAL.Users User = service.GetLoggedUser();

                if (User.LastDateUpdatePassword == null) return false;

                int days = (DateTime.Now - (DateTime)User.LastDateUpdatePassword).Days;

                return (days >= 90) ? true : false;

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        #region Private Methods
        /**
         * Funzione privata common che esegue il login
         */
        private bool ManageLoginUser(string Username, string Password, out string Message, bool PersistUserObject)
        {
            return ManageLoginUser(Username, Password, "", out Message, PersistUserObject);
        }

        private bool ManageLoginUser(string Username, string Password, string Domain, out string Message, bool PersistUserObject)
        {
            DAL.Users User;
            if (DAL.Gateway.Concrete.Users.LoginUser(Username, Password, Domain, out Message, out User))
            {
                if (PersistUserObject) { Framework.Common.SessionsVariables.SetLoggedUser(User); }
                InsertLogAuditRecord(true, User.Username, User, Message);
                return true;
            }
            else
            {
                InsertLogAuditRecord(false, Username, User, Message);
                return false;
            }
        }
        #endregion
    }
}
