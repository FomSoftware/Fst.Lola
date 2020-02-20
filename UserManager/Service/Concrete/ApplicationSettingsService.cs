using System;
using System.Collections.Specialized;

namespace UserManager.Service.Concrete
{
    public class ApplicationSettingsService
    {
        public static string GetWebConfigKey(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }

        public static string GetWebConfigKeyAndThrowExceptionOnNull(string key)
        {
            if (System.Configuration.ConfigurationManager.AppSettings[key] == null)
            {
                throw new Exception("Key Not Found");
            }
            else
            {
                return GetWebConfigKey(key);
            }
        }

        public static NameValueCollection GetWebConfigKeys()
        {
            return System.Configuration.ConfigurationManager.AppSettings;
        }
    }
}
