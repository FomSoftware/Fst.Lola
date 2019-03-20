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
        /// Passandoglio il guid della richiesta effettuta da un lato dell'applicazione cerca di eseguire il login dell'utente.
        /// La richiesta viene cancellata dal database se l'accesso ha avuto successo.
        /// </summary>
        /// <param name="RequestID">Il Guid della richiesta</param>
        /// <param name="PersistUserObject">true se l'utente deve essere salvato in sessione, false se l'utente non deve essere salvato</param>
        /// <returns>true l'utente è loggato e false se l'utente non è stato loggato</returns>
        bool ManageLoginUserOnRedirectAccessRequests(Guid RequestID, bool PersistUserObject);
        
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
