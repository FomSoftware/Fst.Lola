using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringCore.Service.API;
using FomMonitoringCore.Service.API.Concrete;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;

namespace FomMonitoringCore.Framework.Common
{
    [AttributeUsage(AttributeTargets.All)]
    public class LabelAttribute : Attribute
    {
        public string Text { get; }

        public LabelAttribute(string text)
        {
            Text = text;
        }
    }

    public class SessionApiAttribute : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var requestScope = context.Request.GetDependencyScope();

            // Resolve the service you want to use.
            if (requestScope.GetService(typeof(IContextService)) is IContextService contextService && contextService.GetContext() == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Unauthorized", context.Request);
            }
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
        }
    }

    public class SessionWebAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public IContextService ContextService { get; set; }
        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            if (ContextService.GetContext() == null)
            {
                actionExecutingContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Logout", controller = "Account", exception = 2, returnUrl = actionExecutingContext.HttpContext.Request.Url.AbsolutePath })); // redirect to login action
            }
        }
    }

    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }
        public bool AllowMultiple => false;
        private IJwtManager JwtManager;
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var ip = GetClientIp(request);
            var authorization = request.Headers.Authorization;
            var requestScope = context.Request.GetDependencyScope();

            // Resolve the service you want to use.
            JwtManager = requestScope.GetService(typeof(IJwtManager)) as IJwtManager;
            if (authorization == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Authorization Value", request);
                return;
            }

            if (authorization.Scheme != "Bearer")
            {
                context.ErrorResult = new AuthenticationFailureResult("Wrong Authorization Scheme", request);
                return;
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token", request);
                return;
            }

            var token = authorization.Parameter;
            var principal = await AuthenticateJwtToken(token);

            if (principal == null)
                context.ErrorResult = new AuthenticationFailureResult("Invalid Token", request);

            else
                context.Principal = principal;
        }

        private string GetClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }

        private bool ValidateToken(string token, out string machineSerial)
        {
            machineSerial = null;
            
            var simplePrinciple = JwtManager.GetPrincipal(token);

            if (simplePrinciple == null)
                return false;

            var identity = simplePrinciple.Identity as ClaimsIdentity;

            if (identity == null)
                return false;

            if (!identity.IsAuthenticated)
                return false;

            var machineSerialClaim = identity.FindFirst(ClaimTypes.Name);
            machineSerial = machineSerialClaim?.Value;

            if (string.IsNullOrEmpty(machineSerial))
                return false;

            var exp = simplePrinciple.FindFirst("exp");
            // More validate to check whether machineSerial exists in system

            return true;
        }

        protected Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            string machineSerial;

            if (ValidateToken(token, out machineSerial))
            {
                // based on machineSerial to get more information from database in order to build local identity
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, machineSerial)
                    // Add more claims if needed: Roles, ...
                };

                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);

                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";

            context.ChallengeWith("Bearer", parameter);
        }
    }

    public class BasicAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Authorization Value", request);
                return;
            }

            if (authorization.Scheme != "Basic")
            {
                context.ErrorResult = new AuthenticationFailureResult("Wrong Authorization Scheme", request);
                return;
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Basic Token", request);
                return;
            }

            var token = authorization.Parameter;
            var principal = await AuthenticateBasicToken(token);

            if (principal == null)
                context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);

            else
                context.Principal = principal;
        }

        private static bool ValidateToken(string token, out string machineSerial)
        {
            machineSerial = null;

            bool result;

            var encoding = Encoding.GetEncoding(0);
            var credentials = encoding.GetString(Convert.FromBase64String(token));
            var parameters = credentials.Split(':');
            if (parameters.Length == 3)
            {
                var username = parameters[0];                
                machineSerial = parameters[1];
                var password = parameters[2];

                var login = new LoginModel
                {
                    Username = username,
                    Password = password
                };

                var basicManager = DependencyResolver.Current.GetService<IBasicManager>();
                result = basicManager.ValidateCredentials(login);
            }
            else
            {
                result = false;
            }

            return result;
        }

        protected Task<IPrincipal> AuthenticateBasicToken(string token)
        {
            if (!ValidateToken(token, out var machineSerial))
                return Task.FromResult<IPrincipal>(null);

            // based on machineSerial to get more information from database in order to build local identity
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, machineSerial)
                // Add more claims if needed: Roles, ...
            };

            var identity = new ClaimsIdentity(claims, "Basic");
            IPrincipal user = new ClaimsPrincipal(identity);

            return Task.FromResult(user);

        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";

            context.ChallengeWith("Basic", parameter);
        }
    }

    /// <summary>
    /// Filtro per creare nella viewbg il baseurl dell'applicazione
    /// </summary>
    public class AjaxBaseUrlActionFilter : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var applicationPath = HttpContext.Current.Request.ApplicationPath;
            if (applicationPath == "/") applicationPath = "";

            filterContext.Controller.ViewBag.BaseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + applicationPath;
        }
    }
}
