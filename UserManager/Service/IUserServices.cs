using System;

namespace UserManager.Service
{
    public interface IUserServices
    {
        /// <summary>
        /// Use this method to change users password
        /// </summary>
        /// <param name="UserID">UserID that want change password</param>
        /// <param name="NewPassword">The new password</param>
        /// <param name="OldPassword">The old password</param>
        /// <param name="Message">Identify the message of the operation</param>
        /// <returns>Return operation result</returns>
        bool ChangePassword(Guid UserID, string NewPassword, string OldPassword, out string Message);

        /// <summary>
        /// Get a collection of DAL.Users take from database
        /// </summary>
        /// <returns>Return DAL.Users collection</returns>
        System.Collections.Generic.List<FomMonitoringCore.SqlServer.Users> GetAllUsers();

        /// <summary>
        /// Get a collection of DAL.Users take from database
        /// </summary>
        /// <returns>Return DAL.Users collection</returns>
        System.Collections.Generic.List<FomMonitoringCore.SqlServer.Users> GetListOfNonDeletedUsers(FomMonitoringCore.SqlServer.FomMonitoringEntities ModelloEntity);

        /// <summary>
        /// Get a collection of DAL.Users take from database specifyng the roles
        /// </summary>
        /// <returns>Return DAL.Users collection</returns>
        System.Collections.Generic.List<FomMonitoringCore.SqlServer.Users> GetAllUsers(UserManager.Framework.Common.Enumerators.UserRole UserRole);

        /// <summary>
        /// Get a collection of DAL.Users take from database specifyng the roles
        /// </summary>
        /// <returns>Return DAL.Users collection</returns>
        System.Collections.Generic.List<FomMonitoringCore.SqlServer.Users> GetAllUsers(string UserRoleName);

        /// <summary>
        /// Get dot.net culture string from user id
        /// </summary>
        /// <param name="UserID">User Guid</param>
        /// <returns>dot.net culture string like 'it-IT'</returns>
        string GetDotNetCultureFromUserID(Guid UserID);

        /// <summary>
        /// Permette di aggiungere un utente nel database
        /// </summary>
        /// <param name="User"></param>
        void CreateUser(FomMonitoringCore.SqlServer.Users User);

        /// <summary>
        /// Permette di aggiornare i dati di un utente presente nel database
        /// </summary>
        /// <param name="User"></param>
        void ModifyUser(FomMonitoringCore.SqlServer.Users User);


         ///<summary>
         ///Permette di cancellare un utente dal database
         ///</summary>
         ///<param name="User"></param>
        void DeleteUser(FomMonitoringCore.SqlServer.Users User);

        ///<summary>
        ///Permette di cancellare un utente dal database
        ///</summary>
        ///<param name="User"></param>
        void DeleteUser(Guid UserID);

        /// <summary>
        /// Restituisce il modello Users salvato a database
        /// </summary>
        /// <returns>User</returns>
        FomMonitoringCore.SqlServer.Users GetUser(Guid UserID);

        /// <summary>
        /// Restituisce il modello Users salvato a database
        /// </summary>
        /// <returns>User</returns>
        FomMonitoringCore.SqlServer.Users GetUser(string Username);

        /// <summary>
        /// Restituisce il modello Users salvato a database
        /// </summary>
        /// <returns>User</returns>
        FomMonitoringCore.SqlServer.Users GetUser(string Username, string Password);

    }
}
