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
        /// Restituisce il modello Users salvato a database
        /// </summary>
        /// <returns>User</returns>
        FomMonitoringCore.SqlServer.Users GetUser(string Username);

        FomMonitoringCore.SqlServer.Users GetUserById(string Id);



    }
}
