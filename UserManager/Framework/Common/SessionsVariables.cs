using UserManager.Service.Concrete;

namespace UserManager.Framework.Common
{
    public static class SessionsVariables
    {
        #region Session Variable

        public static bool UseCacheInsteadOfSession()
        {
            var result = false;
            var cacheKeyValue = ApplicationSettingsService.GetWebConfigKey("UseCacheInsteadOfSession");

            if (bool.TryParse(cacheKeyValue, out result))
                return result;
            else
                return false;
        }

        #region LoggedUser

        public static FomMonitoringCore.SqlServer.Users GetLoggedUser()
        {
            if (UseCacheInsteadOfSession())
                return CacheService.GetValue<FomMonitoringCore.SqlServer.Users>("LoggedUser");
            else
                return SessionServices.GetValue<FomMonitoringCore.SqlServer.Users>("LoggedUser");
        }

        public static void SetLoggedUser(FomMonitoringCore.SqlServer.Users user)
        {
            if (UseCacheInsteadOfSession())
                CacheService.SetValue("LoggedUser", user);
            else
                SessionServices.SetValue("LoggedUser", user);
        }

        public static void RemoveLoggedUser()
        {
            if (UseCacheInsteadOfSession())
                CacheService.RemoveValue("LoggedUser");
            else
                SessionServices.RemoveValue("LoggedUser");
        }

        public static void ClearSession()
        {
            if (UseCacheInsteadOfSession())
                CacheService.Clear();
            else
                SessionServices.Clear();
        }

        #endregion

        #region SAML Session ID

        public static string GetSamlSessionId()
        {
            if (UseCacheInsteadOfSession())
                return CacheService.GetValue<string>("SAMLSessionID");
            else
                return SessionServices.GetValue<string>("SAMLSessionID");
        }

        public static void SetSamlSessionId(string samlSessionId)
        {
            if (UseCacheInsteadOfSession())
                CacheService.SetValue("SAMLSessionID", samlSessionId);
            else
                SessionServices.SetValue("SAMLSessionID", samlSessionId);
        }

        public static void RemoveSamlSessionId()
        {
            if (UseCacheInsteadOfSession())
                CacheService.RemoveValue("SAMLSessionID");
            else
                SessionServices.RemoveValue("SAMLSessionID");
        }

        #endregion

        #endregion
    }
}
