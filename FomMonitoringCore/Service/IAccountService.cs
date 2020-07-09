using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface IAccountService
    {
        /// <summary>
        /// Get logged user.
        /// </summary>
        /// <returns></returns>
        UserModel GetLoggedUser();

        /// <summary>
        /// Logins the specified user name.
        /// </summary>
        /// <param name="username">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="rememberMe">The remember me.</param>
        /// <returns></returns>
        ResponseModel Login(string username, string password, bool rememberMe, bool remoteAuthentication = false);

        bool LoginApi(string username, string password);

        /// <summary>
        /// Logouts the user.
        /// </summary>
        void Logout();
    }
}