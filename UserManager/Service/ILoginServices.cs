using System;

namespace UserManager.Service
{
    public interface ILoginServices
    {
        #region Login methods
        bool LoginUser(string Username, string Password, out string Message);        
        bool LoginUser(string Username, string Password, out string Message, bool PersistUserObject);
        bool LoginUser(string Username, string Password, string Domain, out string Message, bool PersistUserObject);

        bool LoginUserWithEncryptedPassword(string Username, string Password, out string Message);        
        bool LoginUserWithEncryptedPassword(string Username, string Password, out string Message, bool PersistUserObject);
        bool LoginUserWithEncryptedPassword(string Username, string Password, string Domain, out string Message, bool PersistUserObject);
        #endregion

        bool ManageLoginUserWithoutPassword(string Username, out string Message, bool PersistUserObject);
        bool ManageLoginUserWithoutPassword(string Username, string Domain, out string Message, bool PersistUserObject);
        bool ManageLoginUserWithoutPassword(Guid UserID, out string Message, bool PersistUserObject);
        bool IsFirstLogin();
        bool IsPasswordExpired();

        
        /// <summary>
        /// Questo metodo esegue il logout dell'utente dall'applicazione ripulendo la sessione, la cache e tutte le
        /// informazioni assegnate
        /// </summary>
        /// <param name="FromToken">Parametro per indicare il percorso da cui è arrivata la richiesta di log</param>
        void LogoutUser(string FromToken, bool PerformRedirectToLoginPage = true);


        string GetUserDefaultHomePage();

        void RedirectUserToDefaultHomePage();
        
        void RedirectUserToLoginPage();

        bool UserObjectIsInSession();

        bool CheckUserAuthentication();

        /// <summary>
        /// Verifica se la username passata come parametro è già presente all'interno del database
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        bool CheckIfUsernameAlreadyExist(string Username);

        string EncryptPassword(string psw);

        string DecryptPassword(string EncryptedPsw);
    }
}
