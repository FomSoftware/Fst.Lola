using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager.Service.Concrete
{
    public class CacheService
    {
        #region Common Method

        private static string ReorganizeKey(string key)
        {
            string machineName = System.Web.HttpContext.Current.Server.MachineName;
            machineName = machineName.Trim();
            return string.Format("{0}_{1}", machineName, key);
        }

        public static void SetValue<T>(string key, T value)
        {
            System.Web.HttpContext.Current.Cache.Insert(ReorganizeKey(key), value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
        }

        public static void RemoveValue(string key)
        {
            System.Web.HttpContext.Current.Cache.Remove(ReorganizeKey(key));
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
