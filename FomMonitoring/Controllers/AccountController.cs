using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringCore.Service.APIClient;
using FomMonitoringCore.Service.APIClient.Concrete;
using FomMonitoringResources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.SqlServer;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net.Mime;

namespace FomMonitoring.Controllers
{
    public class AccountController : Controller
    {
        private readonly IContextService _contextService;
        private readonly IFomMonitoringEntities _dbContext;
        private readonly IAccountService _accountService;
        private readonly IJsonAPIClientService _jsonApiClientService;
        private readonly IUserManagerService _userManagerService;

        public AccountController(IContextService contextService, IUserManagerService userManagerService,
            IFomMonitoringEntities dbContext, IAccountService accountService, IJsonAPIClientService jsonApiClientService)
        {
            _contextService = contextService;
            _dbContext = dbContext;
            _accountService = accountService;
            _jsonApiClientService = jsonApiClientService;
            _userManagerService = userManagerService;
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, int exception = 0)
        {
            var isDemo = bool.Parse(ApplicationSettingService.GetWebConfigKey("DemoMode"));
            ActionResult result = null;

            LoginModel model = new LoginModel();
            setInitialModel(model);

            if (isDemo)
            {
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

                if (returnUrl == null)
                {
                    return View(model);
                }

                ViewBag.ReturnUrl = returnUrl;
                result = View();
            }

            return result;
        }

        private void setInitialModel(LoginModel model)
        {
            model.AllLanguages = _userManagerService.GetLanguages().OrderBy(o => o.IdLanguage).ToList();

            string lang = (string)Request.RequestContext.RouteData.Values["lang"];
            if (lang != null)
            {
                foreach (var lingua in model.AllLanguages)
                {
                    if (lingua.InitialsLanguage == lang)
                    {
                        model.ActualLanguage = lingua;
                        break;
                    }
                }
            }
            else
            {
                model.ActualLanguage = model.AllLanguages.FirstOrDefault();
            }

            model.ExternalFaqs = _dbContext.Set<Faq>().Where(f => f.IsVisible == true).OrderBy(f => f.Order).ToList();

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

            setInitialModel(model);
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

        [HttpPost]
        [AllowAnonymous]
        [Route("Account/InviaSupportReq")]
        public ActionResult InviaSupportReq(SupportViewModel supportReq)
        {
            try
            {
                string body = $"<b>Nome e Cognome: </b> {supportReq.Nome} <br>";
                body += $"<b>Azienda: </b>{supportReq.Azienda} <br>";
                body += $"<b>Telefono: </b>{supportReq.Prefisso} {supportReq.Telefono}<br>";
                body += $"<b>Email: </b>{supportReq.Email}<br>";
                body += $"<b>Nome Macchina: </b>{supportReq.NomeMacchina} <br>";
                body += $"<b>Seriale: </b>{supportReq.Seriale} <br>";
                body += $"<b>Testo: </b>{supportReq.Testo} <br>";
                var message = new MailMessage(ApplicationSettingService.GetWebConfigKey("EmailFromAddress"),
                    ApplicationSettingService.GetWebConfigKey("EmailSupportAddress"),
                    "Richiesta di supporto LOLA", body );
                message.IsBodyHtml = true;
                
                //invio copia all'utente per ricevuta
                var message2 = new MailMessage(ApplicationSettingService.GetWebConfigKey("EmailFromAddress"),
                    supportReq.Email,
                    "Richiesta di supporto a LOLA inviata",
                    "Hai inviato la seguente richiesta di supporto: <br>" + body);
                message2.IsBodyHtml = true;
                
                if (supportReq.File != null && supportReq.File.Length > 0 && supportReq.File[0] != null)
                {
                    if (supportReq.File[0].ContentLength > 3 * 1024 * 1024 ||
                        !(supportReq.File[0].FileName.ToLower().EndsWith(".jpg") ||
                          supportReq.File[0].FileName.ToLower().EndsWith(".jpeg") ||
                          supportReq.File[0].FileName.ToLower().EndsWith(".png") ||
                          supportReq.File[0].FileName.ToLower().EndsWith(".doc") ||
                          supportReq.File[0].FileName.ToLower().EndsWith(".docx")))
                    {
                        return Json(new
                        {
                            result = false,
                            msg = Resource.FileNotValid
                        }, JsonRequestBehavior.AllowGet);
                    }

                    Attachment attachment = new Attachment(supportReq.File[0].InputStream, supportReq.File[0].FileName);
                    message.Attachments.Add(attachment);
                    message2.Attachments.Add(attachment);

                }
                EmailSender.SendEmail(message);
                EmailSender.SendEmail(message2);

                object returnObj = new
                {
                    result = true
                };
                
                return Json(returnObj, JsonRequestBehavior.AllowGet);
            }
            catch (InvalidOperationException ex)
            {
                object returnObj = new
                {
                    result = false,
                    msg = ex.Message
                };

                return Json(returnObj, JsonRequestBehavior.AllowGet);
            }
        }
    }
}