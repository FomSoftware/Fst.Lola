using System;
using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Globalization;
using System.Web.Mvc;

namespace FomMonitoring.Controllers
{
    [SessionWeb]
    [Authorize(Roles = Common.Assistance + "," + Common.Administrator + "," + Common.RandD + "," + Common.Brach + "," + Common.Dealers)]
    public class AssistanceController : Controller
    {
        private IContextService _contextService;
        private IAssistanceViewService _assistanceViewService;

        public AssistanceController(IContextService contextService, IAssistanceViewService assistanceViewService)
        {
            _contextService = contextService;
            _assistanceViewService = assistanceViewService;
        }

        public ActionResult Index()
        {
            if (!_contextService.InitializeAssistanceLevel())
            {
                //sbagliato, la action Logout su MesController non esiste
                return RedirectToAction("Logout", "Account", new { returnUrl = string.Empty, exception = 3 });
            }
            _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);

            ContextModel context = _contextService.GetContext();
            AssistanceViewModel model = _assistanceViewService.GetAssistance(context);
            return View("Index", model);
        }

        public ActionResult SignInAs(string MachineId, string CustomerId)
        {
            var ctx = _contextService.GetContext();
            if (ctx == null)
            {
                //sbagliato, la action Logout su MesController non esiste
                return RedirectToAction("Logout", "Account", new { returnUrl = string.Empty, exception = 3 });
            }

            if (!String.IsNullOrEmpty(MachineId))
            {
                int id = Int32.Parse(MachineId);
                ctx.AssistanceMachineId = id;
                _assistanceViewService.SetCompanyName(ctx);
                return RedirectToAction("Index", "Machine");
            }
            else if (!String.IsNullOrEmpty(CustomerId))
            {
                ctx.AssistanceUserId = CustomerId;
                _assistanceViewService.SetCompanyName(ctx);
                return RedirectToAction("Index", "Mes");
            }
           

            return RedirectToAction("Index");
        }

    }
}