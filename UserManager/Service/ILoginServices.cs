using System;

namespace UserManager.Service
{
    public interface ILoginServices
    {
        #region Login methods    
        
        bool LoginUserWithEncryptedPassword(string Username, string Password, out string Message, bool PersistUserObject);
        bool LoginUserWithEncryptedPassword(string Username, string Password, string Domain, out string Message, bool PersistUserObject);
        #endregion

        bool ManageLoginUserWithoutPassword(string Username, out string Message, bool PersistUserObject);
        bool IsFirstLogin();

        
        /// <summary>
        /// Questo metodo esegue il logout dell'utente dall'applicazione ripulendo la sessione, la cache e tutte le
        /// informazioni assegnate
        /// </summary>
        /// <param name="FromToken">Parametro per indicare il percorso da cui è arrivata la richiesta di log</param>
        void LogoutUser(string FromToken, bool PerformRedirectToLoginPage = true);
        
        string EncryptPassword(string psw);

        string DecryptPassword(string EncryptedPsw);
    }
}
