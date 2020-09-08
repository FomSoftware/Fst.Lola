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


    }
}
