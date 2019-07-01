using FomMonitoringCore.Framework.Config;
using System.Reflection;
using System.Web.Http;
using Mapster;

namespace FomMonitoringApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(MapsterConfig)));
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
