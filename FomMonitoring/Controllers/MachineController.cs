using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Service;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Web.Mvc;

namespace FomMonitoring.Controllers
{
    [SessionWeb]
    [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer + "," + Common.Demo)]
    public class MachineController : Controller
    {
        private readonly IMachineViewService _machineViewService;
        private readonly IMaintenanceViewService _maintenanceViewService;
        private readonly IContextService _contextService;

        public MachineController(
            IMachineViewService machineViewService, 
            IMaintenanceViewService maintenanceViewService,
            IContextService contextService)
        {
            _machineViewService = machineViewService;
            _maintenanceViewService = maintenanceViewService;
            _contextService = contextService;
        }

        public ActionResult Index()
        {
            try
            {

                if (!_contextService.InitializeMachineLevel())
                    return RedirectToAction("Logout", "Account", new { returnUrl = string.Empty, exception = 4 });

                _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);

                var context = _contextService.GetContext();
                var machine = _machineViewService.GetMachine(context);

                return View(machine);
            }
            catch(Exception ex)
            {
                Debugger.Break();
                throw ex;
            }
        }

        [Route("{lang}/Machine/Index/{MachineID}")]
        public ActionResult Index(int MachineID)
        {
            try
            {
                if (!_contextService.InitializeMachineLevel(MachineID))
                    return RedirectToAction("Logout", "Account", new { returnUrl = string.Empty, exception = 4 });

                var isCorrect = _contextService.CheckSecurityParameterApi(MachineID, enCheckParam.Machine);

                if (!isCorrect)
                    return RedirectToAction("Logout", "Account", new { returnUrl = string.Empty, exception = 1 });

                _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);

                var context = _contextService.GetContext();
                context.ActualMachineGroup = null;
                var machine = _machineViewService.GetMachine(context);

                return View(machine);
            }
            catch(Exception ex)
            {
                Debugger.Break();
                throw ex;
            }
        }


    }
}