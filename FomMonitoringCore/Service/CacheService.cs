using System;
using System.Web.Caching;

namespace FomMonitoringCore.Service
{
    public class CacheService
    {
        #region Common Method
        public static void SetCacheValue<T>(string key, T value)
        {
            System.Web.HttpContext.Current.Cache[key] = value;
        }

        public static void SetCacheValue<T>(string key, T value, string dependencyKey)
        {
            CacheDependency dependency = new CacheDependency(new string[] { }, new string[] { dependencyKey });
            System.Web.HttpContext.Current.Cache.Insert(key, value, dependency);
        }

        public static void RemoveCacheValue(string key)
        {
            System.Web.HttpContext.Current.Cache.Remove(key);
        }

        public static void CleanAllCache()
        {
            foreach (System.Collections.DictionaryEntry entry in System.Web.HttpContext.Current.Cache)
            {
                RemoveCacheValue((string)entry.Key);
            }
        }

        public static T GetCacheValue<T>(string key)
        {
            if (System.Web.HttpContext.Current.Cache[key] != null)
            {
                return (T)System.Web.HttpContext.Current.Cache[key];
            }
            else
            {
                return default(T);
            }
        }
        #endregion

        #region Private
        private static T Get<T>(Func<T> func, T _default)
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
