using System.Globalization;
using System.Web;
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

        public static HtmlString GetResourceForJs(string key)
        {
            string str1 = GetResource(key);
            HtmlString str = new HtmlString(HttpUtility.JavaScriptStringEncode(str1));
            return str;
        }

        public static string GetResource(string key, CultureInfo cultureInfo)
        {
            var str = new System.Resources.ResourceManager(typeof(Resource)).GetString(key, cultureInfo);
            return string.IsNullOrWhiteSpace(str) ? $"[{key}]" : str;
        }
    }
}
