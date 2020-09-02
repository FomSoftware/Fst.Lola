using UserManager.Service.Concrete;

namespace UserManager.Framework.Common
{
    public static class SessionsVariables
    {
        #region Session Variable

        public static bool UseCacheInsteadOfSession()
        {
            var cacheKeyValue = ApplicationSettingsService.GetWebConfigKey("UseCacheInsteadOfSession");

            return bool.TryParse(cacheKeyValue, out var result) && result;
        }

        #region LoggedUser

        public static FomMonitoringCore.SqlServer.Users GetLoggedUser()
        {
            return UseCacheInsteadOfSession() ? CacheService.GetValue<FomMonitoringCore.SqlServer.Users>("LoggedUser") : SessionServices.GetValue<FomMonitoringCore.SqlServer.Users>("LoggedUser");
        }

        public static void SetLoggedUser(FomMonitoringCore.SqlServer.Users user)
        {
            if (UseCacheInsteadOfSession())
                CacheService.SetValue("LoggedUser", user);
            else
                SessionServices.SetValue("LoggedUser", user);
        }

        public static void ClearSession()
        {
            if (UseCacheInsteadOfSession())
                CacheService.Clear();
            else
                SessionServices.Clear();
        }

        #endregion


        #endregion
    }
}
