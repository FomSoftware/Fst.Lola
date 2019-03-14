using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
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
        public bool AllowMultiple
        {
            get
            {
                return false;
            }
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            if (ContextService.GetContext() == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Unauthorized", context.Request);
                return;
            }
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return;
        }
    }

    public class SessionWebAttribute : System.Web.Mvc.ActionFilterAttribute
    {
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

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var ip = GetClientIp(request);
            var authorization = request.Headers.Authorization;

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

        private static bool ValidateToken(string token, out string machineSerial)
        {
            machineSerial = null;

            JwtManager jwtManager = new JwtManager();
            var simplePrinciple = jwtManager.GetPrincipal(token);

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
            string password = null;
            string username = null;
            machineSerial = null;

            bool result = true;

            Encoding encoding = Encoding.GetEncoding(0);
            string credentials = encoding.GetString(Convert.FromBase64String(token));
            string[] parameters = credentials.Split(new char[] { ':' });
            if (parameters.Length == 3)
            {
                username = parameters[0];
                password = parameters[1];
                machineSerial = parameters[2];

                LoginModel login = new LoginModel();
                login.Username = username;
                login.Password = password;

                BasicManager basicManager = new BasicManager();
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
            string machineSerial;

            if (ValidateToken(token, out machineSerial))
            {
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
            var applicationPath = System.Web.HttpContext.Current.Request.ApplicationPath;
            if (applicationPath == "/") applicationPath = "";

            filterContext.Controller.ViewBag.BaseUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + applicationPath;
        }
    }
}
