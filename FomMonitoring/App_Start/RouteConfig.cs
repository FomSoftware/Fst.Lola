using System.Web.Mvc;
using System.Web.Routing;

namespace FomMonitoring
{
    public class RouteConfig
    {
        public static string UrlPrefix { get { return "ajax"; } }
        public static string UrlPrefixRelative { get { return "~/ajax"; } }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "DefaultLocalized",
                url: "{lang}/{controller}/{action}/{id}",
                constraints: new
                {
                    lang = @"(\w{2})"
                },
                defaults: new
                {
                    lang = "en",
                    //controller = "Machine",
                    //action = "Index",
                    controller = "Account",
                    action = "Logout",
                    id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "ChangeLocale",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    id = UrlParameter.Optional
                }
            );
        }
    }
}
