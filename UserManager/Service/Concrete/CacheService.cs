﻿using System;
using System.Collections;

namespace UserManager.Service.Concrete
{
    public class CacheService
    {
        #region Common Method

        private static string ReorganizeKey(string key)
        {
            var machineName = System.Web.HttpContext.Current.Server.MachineName;
            machineName = machineName.Trim();
            return string.Format("{0}_{1}", machineName, key);
        }

        public static void SetValue<T>(string key, T value)
        {
            System.Web.HttpContext.Current.Cache.Insert(ReorganizeKey(key), value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
        }


        public static T GetValue<T>(string key)
        {
            if (System.Web.HttpContext.Current.Cache != null && null != System.Web.HttpContext.Current.Cache[key])
            {
                return (T)System.Web.HttpContext.Current.Cache.Get(ReorganizeKey(key));
            }
            else
            {
                return default(T);
            }
        }

        public static void Clear()
        {
            foreach (DictionaryEntry entry in System.Web.HttpContext.Current.Cache)
            {
                System.Web.HttpContext.Current.Cache.Remove(entry.Key.ToString());
            }
        }

        #endregion
    }
}
