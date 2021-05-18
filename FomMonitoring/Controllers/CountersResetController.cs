using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringCore.SqlServer;

namespace FomMonitoring.Controllers
{
    [SessionWeb]
    public class CountersResetController : Controller
    {
        private readonly IContextService _contextService;
        private readonly IFaqService _faqService;
        private readonly IMachineService _machineService;

        public CountersResetController(IContextService contextService, IFaqService faqService, IMachineService machineService)
        {
            _contextService = contextService;
            _faqService = faqService;
            _machineService = machineService;
        }

        // GET: CountersReset
        public ActionResult Index()
        {
            if (!_contextService.InitializeCountersResetLevel())
                return RedirectToAction("Logout", "Account", new { returnUrl = string.Empty, exception = 6 });

            var context = _contextService.GetContext();
            _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);
            CountersResetViewModel model = new CountersResetViewModel();
            if (context.ActualMachine != null)
            {
                model.lista = _machineService.GetMachineCountersReset(context.ActualMachine.Id,
                    context.ActualLanguage.IdLanguage);
            }

            return View(model);
        }


    }
}