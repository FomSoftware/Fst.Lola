using UserManager.Service.Concrete;

namespace UserManager.Framework.Common
{
    public static class SessionsVariables
    {
        #region Session Variable

        public static bool UseCacheInsteadOfSession()
        {
            bool result = false;
            var cacheKeyValue = ApplicationSettingsService.GetWebConfigKey("UseCacheInsteadOfSession");

            if (bool.TryParse(cacheKeyValue, out result))
                return result;
            else
                return false;
        }

        #region LoggedUser

        public static DAL.Users GetLoggedUser()
        {
            if (SessionsVariables.UseCacheInsteadOfSession())
                return CacheService.GetValue<DAL.Users>("LoggedUser");
            else
                return SessionServices.GetValue<DAL.Users>("LoggedUser");
        }

        public static void SetLoggedUser(DAL.Users User)
        {
            if (SessionsVariables.UseCacheInsteadOfSession())
                CacheService.SetValue<DAL.Users>("LoggedUser", User);
            else
                SessionServices.SetValue<DAL.Users>("LoggedUser", User);
        }

        public static void RemoveLoggedUser()
        {
            if (SessionsVariables.UseCacheInsteadOfSession())
                CacheService.RemoveValue("LoggedUser");
            else
                SessionServices.RemoveValue("LoggedUser");
        }

        public static void ClearSession()
        {
            if (SessionsVariables.UseCacheInsteadOfSession())
                CacheService.Clear();
            else
                SessionServices.Clear();
        }

        #endregion

        #region SAML Session ID

        public static string GetSAMLSessionID()
        {
            if (SessionsVariables.UseCacheInsteadOfSession())
                return CacheService.GetValue<string>("SAMLSessionID");
            else
                return SessionServices.GetValue<string>("SAMLSessionID");
        }

        public static void SetSAMLSessionID(string SAMLSessionID)
        {
            if (SessionsVariables.UseCacheInsteadOfSession())
                CacheService.SetValue<string>("SAMLSessionID", SAMLSessionID);
            else
                SessionServices.SetValue<string>("SAMLSessionID", SAMLSessionID);
        }

        public static void RemoveSAMLSessionID()
        {
            if (SessionsVariables.UseCacheInsteadOfSession())
                CacheService.RemoveValue("SAMLSessionID");
            else
                SessionServices.RemoveValue("SAMLSessionID");
        }

        #endregion

        #endregion
    }
}
