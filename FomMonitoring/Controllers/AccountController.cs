using CommonCore.Service;
using FomMonitoring.Models;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringCore.Service.APIClient;
using FomMonitoringCore.Service.APIClient.Concrete;
using FomMonitoringResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FomMonitoring.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, int exception = 0)
        {
            string isDemo = ApplicationSettingService.GetWebConfigKey("Demo");
            ActionResult result = null;
            if (!bool.Parse(isDemo))
            {
                switch (exception)
                {
                    case 1:
                        ModelState.AddModelError("", Resource.Unauthorized);
                        break;
                    case 2:
                        ModelState.AddModelError("", Resource.SessionTimeout);
                        break;
                    case 3:
                        ModelState.AddModelError("", Resource.NoPlant);
                        break;
                    case 4:
                        ModelState.AddModelError("", Resource.NoMachine);
                        break;
                    default:
                        break;
                }
                ViewBag.ReturnUrl = returnUrl;
                result = View();
            }
            else
            {
                LoginModel model = new LoginModel();
                model.Username = ApplicationSettingService.GetWebConfigKey("DemoUser");
                model.RememberMe = true;
                result = Connect(model, returnUrl);
            }
            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", Resource.UserPassNotValid);
                return View(model);
            }

            var result = Connect(model, returnUrl);
            return result;
        }

        [AllowAnonymous]
        public ActionResult Logout(string returnUrl, int exception = 0)
        {
            //exception = exception == 0 && Request.Cookies.Count > 1 && Request.Cookies.Get(FormsAuthentication.FormsCookieName) == null ? 2 : exception;
            //exception = exception == 0 && Request.Cookies.Count == 1 && Request.Cookies.AllKeys.FirstOrDefault(f => f.StartsWith("__RequestVerificationToken")) == null ? 1 : exception;
            Disconnect();
            RedirectToRouteResult redirect = null;
            switch (exception)
            {
                case 0:
                    redirect = RedirectToAction("Login", new { returnUrl = returnUrl });
                    break;
                case 1:
                case 3:
                case 4:
                    redirect = RedirectToAction("Login", new { returnUrl = string.Empty, exception = exception });
                    break;
                case 2:
                    redirect = RedirectToAction("Login", new { returnUrl = returnUrl, exception = exception });
                    break;
            }
            return redirect;
        }

        #region Helpers

        private void Disconnect()
        {
            FormsAuthentication.SignOut();
            AccountService accountService = new AccountService();
            accountService.Logout();
            CacheService.CleanAllCache();
            List<HttpCookie> cookies = new List<HttpCookie>();
            foreach (var cookie in Request.Cookies)
            {
                HttpCookie c = new HttpCookie(cookie.ToString());
                c.Expires = DateTime.Now.AddDays(-1);
                cookies.Add(c);
            }
            foreach (var cookie in cookies)
            {
                Response.Cookies.Add(cookie);
            }
        }

        private ActionResult Connect(LoginModel model, string returnUrl)
        {

            string isTest = ApplicationSettingService.GetWebConfigKey("Test");
            if (bool.Parse(isTest))
            {
                model.Password = ApplicationSettingService.GetWebConfigKey("DefaultPassword");
            }
            AccountService accountService = new AccountService();
            if (ModelState.IsValid && accountService.Login(model.Username, model.Password, true).Result)
            {
                FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                if (ContextService.InitializeContext())
                {
                    return RedirectToLocal(returnUrl);
                }
                ModelState.AddModelError("", Resource.LoginProblem);
                return View(model);
            }
            ModelState.AddModelError("", Resource.UserPassNotValid);
            return View(model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            ContextModel context = ContextService.GetContext();

            if (context != null)
            {
                string url = "/Machine";
                returnUrl = returnUrl == "/" ? url : returnUrl;
                if (!string.IsNullOrEmpty(returnUrl) && returnUrl != url && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                RedirectToRouteResult result = null;
                switch (context.User.Roles.First())
                {
                    case enRole.Administrator:
                        result = RedirectToAction("Index", "Mes");
                        break;
                    //case enRole.Assistance:
                    //    result = RedirectToAction("Index", "Machine");
                    //    break;
                    //case enRole.Customer:
                    //    result = RedirectToAction("Index", "UserManager");
                    //    break;
                    case enRole.HeadWorkshop:
                        result = RedirectToAction("Index", "Mes");
                        break;
                    case enRole.Operator:
                        result = RedirectToAction("Index", "Machine");
                        break;
                    default:
                        result = RedirectToAction("Logout", new { exception = 1 });
                        break;
                }

                return result;
            }
            return RedirectToAction("Logout");
        }

        #endregion
    }
}