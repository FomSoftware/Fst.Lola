using System.Globalization;
using FomMonitoringResources;

namespace FomMonitoringCore.Service
{

    public static class LocalizationService
    {

        public static string GetResource(string key)
        {
            var str = new System.Resources.ResourceManager(typeof(Resource)).GetString(key);
            return string.IsNullOrWhiteSpace(str) ? $"[{key}]" : str;
        }

        public static string GetResource(string key, CultureInfo cultureInfo)
        {
            var str = new System.Resources.ResourceManager(typeof(Resource)).GetString(key, cultureInfo);
            return string.IsNullOrWhiteSpace(str) ? $"[{key}]" : str;
        }
    }
}
