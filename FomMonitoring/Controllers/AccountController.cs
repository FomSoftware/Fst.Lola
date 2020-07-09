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
using FomMonitoringCore.SqlServer;

namespace FomMonitoring.Controllers
{
    public class AccountController : Controller
    {
        private readonly IContextService _contextService;
        private readonly IFomMonitoringEntities _dbContext;
        private readonly IAccountService _accountService;
        private readonly IJsonAPIClientService _jsonApiClientService;

        public AccountController(IContextService contextService, IFomMonitoringEntities dbContext, IAccountService accountService, IJsonAPIClientService jsonApiClientService)
        {
            _contextService = contextService;
            _dbContext = dbContext;
            _accountService = accountService;
            _jsonApiClientService = jsonApiClientService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, int exception = 0)
        {


            var isDemo = bool.Parse(ApplicationSettingService.GetWebConfigKey("DemoMode"));
            ActionResult result = null;

            if (isDemo)
            {
                LoginModel model = new LoginModel();
                model.Username = ApplicationSettingService.GetWebConfigKey("DemoUsername");
                model.Password = ApplicationSettingService.GetWebConfigKey("DemoPassword");
                model.RememberMe = true;
                result = Connect(model, returnUrl);
            }
            else
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
            Disconnect();
            RedirectToRouteResult redirect = null;

            switch (exception)
            {
                case 0:
                    redirect = RedirectToAction("Login", new {returnUrl });
                    break;
                case 1:
                case 3:
                case 4:
                    redirect = RedirectToAction("Login", new { returnUrl = string.Empty, exception });
                    break;
                case 2:
                    redirect = RedirectToAction("Login", new {returnUrl, exception });
                    break;
            }

            return redirect;
        }

        #region Helpers

        private void Disconnect()
        {
            FormsAuthentication.SignOut();
            _accountService.Logout();
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
            if (ModelState.IsValid)
            {
                enLoginResult remoteLoginResult = enLoginResult.NotExists;

                /* REMOTE LOGIN */
                var remoteLogin = bool.Parse(ApplicationSettingService.GetWebConfigKey("RemoteLogin"));
                if (remoteLogin)
                {
                    remoteLoginResult = _jsonApiClientService.ValidateCredentialsViaRemoteApi(model.Username, model.Password);
                }

                switch (remoteLoginResult)
                {
                    case enLoginResult.Ok:
                    case enLoginResult.NotExists:
                        var localLoginResult = _accountService.Login(model.Username, model.Password, true, (remoteLoginResult == enLoginResult.Ok));
                        if (localLoginResult.Result)
                        {
                            FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                            if (_contextService.InitializeContext())
                            {
                                ClearNotificationTable();
                                return RedirectToLocal(returnUrl);
                            }
                            
                            ModelState.AddModelError("", Resource.LoginProblem);
                        }

                        if(remoteLoginResult == enLoginResult.Ok && localLoginResult.Result == false)
                        {
                            ModelState.AddModelError("", Resource.NoMachine);
                        }
                        else
                        {
                            if (localLoginResult.Message == "User is not enabled")
                            {
                                ModelState.AddModelError("", Resource.UserNotEnabled);
                            }
                            else
                            {
                                ModelState.AddModelError("", Resource.PassNotValid);
                            }
                        }
                        break;

                    case enLoginResult.Disabled:
                        ModelState.AddModelError("", Resource.UserExpired);
                        break;

                    case enLoginResult.WrongPassword:
                        ModelState.AddModelError("", Resource.PassNotValid);
                        break;

                    default:
                        ModelState.AddModelError("", Resource.LoginProblem);
                        break;
                }


                return View(model);
            }

            ModelState.AddModelError("", Resource.UserPassNotValid);
            return View(model);
        }

        private void ClearNotificationTable()
        {
            var user = _accountService.GetLoggedUser();
            var toRemove = _dbContext.Set<MessageMachineNotification>().Where(m => m.UserId == user.ID.ToString());
            if (toRemove.Any())
            {
                _dbContext.Set<MessageMachineNotification>().RemoveRange(toRemove);
                _dbContext.SaveChanges();
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            var context = _contextService.GetContext();

            if (context == null)
                return RedirectToAction("Logout");

            var url = "/Machine";
            returnUrl = returnUrl == "/" ? url : returnUrl;

            if (!string.IsNullOrEmpty(returnUrl) && returnUrl != url && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            RedirectToRouteResult result = null;
            //Fabiana: le parti commentate vanno bene ma il bug non è stato quotato, per rilasciarlo scommentare anche Users.cs linea 32 e 40
            switch (context.User.Role)
            {
                case enRole.Administrator:
                    result = RedirectToAction("Index", "Mes");
                    //result = RedirectToAction("Index", "Mes", new RouteValueDictionary { { "lang", context.ActualLanguage.InitialsLanguage } });
                    break;

                case enRole.HeadWorkshop:
                    result = RedirectToAction("Index", "Mes");
                    //result = RedirectToAction("Index", "Mes", new RouteValueDictionary { { "lang", context.ActualLanguage.InitialsLanguage } });
                    break;

                case enRole.Operator:
                    result = RedirectToAction("Index", "Machine");
                    //result = RedirectToAction("Index", "Machine", new RouteValueDictionary { { "lang", context.ActualLanguage.InitialsLanguage } });
                    break;

                case enRole.Customer:
                    result = RedirectToAction("Index", "Mes");
                    //result = RedirectToAction("Index", "Mes", new RouteValueDictionary { { "lang", context.ActualLanguage.InitialsLanguage } });
                    break;

                default:
                    result = RedirectToAction("Logout", new { exception = 1 });
                    break;
            }

            return result;
        }

        #endregion
    }
}