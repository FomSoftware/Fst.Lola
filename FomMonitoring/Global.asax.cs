using FomMonitoring.App_Start;
using FomMonitoringCore.Framework.Common;
using Mapster;
using System;
using System.Globalization;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace FomMonitoring
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ControllerBuilder.Current.SetControllerFactory(new DefaultControllerFactory(new I18nControllerActivator()));
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(MapsterConfig)));
            GlobalFilters.Filters.Add(new AjaxBaseUrlActionFilter(), 0);
            log4net.Config.XmlConfigurator.Configure();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs args)
        {
            string[] rolesArray = new string[] { "UnRole" };
            try
            {
                if (Context.User != null)
                {
                    var cookie = Request.Cookies[Context.User.Identity.Name];
                    GenericPrincipal gp = new GenericPrincipal(Context.User.Identity, cookie.Value.Contains(",") ? cookie.Value.Split(',') : new string[] { cookie.Value });
                    Context.User = gp;
                }
            }
            catch
            {
                GenericPrincipal gp = new GenericPrincipal(Context.User.Identity, rolesArray);
                Context.User = gp;
            }
        }

        protected void Application_PostAuthorizeRequest()
        {
            bool isWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(RouteConfig.UrlPrefixRelative);
            if (isWebApiRequest)
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();

            HttpException httpException = exception as HttpException;
            if (httpException == null)
            {
                httpException = HttpException.CreateFromLastError(exception.Message);
            }
            HttpContext.Current.Response.RedirectToRoute(new RouteValueDictionary(new { controller = "Error", error = httpException.GetHttpCode() }));
            // clear error on server
            Server.ClearError();
        }
    }

    public class I18nControllerActivator : IControllerActivator
    {
        private string _DefaultLanguage = "en";

        public IController Create(RequestContext requestContext, Type controllerType)
        {
            //Get the {language} parameter in the RouteData
            string lang = (string)requestContext.RouteData.Values["lang"];
            string segment1 = requestContext.HttpContext.Request.Url.Segments.Length > 1 ? requestContext.HttpContext.Request.Url.Segments[1] : string.Empty;
            string segment2 = requestContext.HttpContext.Request.Url.Segments.Length > 2 ? requestContext.HttpContext.Request.Url.Segments[2] : string.Empty;
            lang = lang == null && segment1.Length == 3 ? segment1.Substring(0, 2) : (lang == null && segment2.Length == 3 ? segment2.Substring(0, 2) : lang);
            lang = lang ?? _DefaultLanguage;

            try
            {
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            }
            catch (Exception)
            {
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(_DefaultLanguage);
            }

            requestContext.RouteData.Values["lang"] = lang;
            return DependencyResolver.Current.GetService(controllerType) as IController;
        }
    }
}
