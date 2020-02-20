using System;

namespace FomMonitoringCore.Service
{
    public class SessionService
    {
        #region Common Method

        public static void SetSessionValue<T>(string key, T value)
        {
            System.Web.HttpContext.Current.Session[key] = value;
        }

        public static void RemoveSessionValue(string key)
        {
            System.Web.HttpContext.Current.Session.Remove(key);
        }

        public static T GetSessionValue<T>(string key)
        {
            if (System.Web.HttpContext.Current.Session[key] != null)
            {
                return (T)System.Web.HttpContext.Current.Session[key];
            }
            else
            {
                return default(T);
            }
        }

        #endregion

        #region Private

        private static T Get<T>(
            Func<T> func,
            T _default
        )
        {
            try
            {
                return func();
            }
            catch
            {
                return _default;
            }
        }

        #endregion
    }
}
