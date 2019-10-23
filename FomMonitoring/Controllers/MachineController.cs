using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Web.Mvc;

namespace FomMonitoring.Controllers
{
    [SessionWeb]
    [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
    public class MachineController : Controller
    {
        private IMachineViewService _machineViewService;
        private IMaintenanceViewService _maintenanceViewService;
        private IContextService _contextService;

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
            if (!_contextService.InitializeMachineLevel())
                return RedirectToAction("Logout", new { returnUrl = string.Empty, exception = 4 });

            _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);

            ContextModel context = _contextService.GetContext();
            MachineViewModel machine = _machineViewService.GetMachine(context);

            return View(machine);
        }

        [Route("{lang}/Machine/Index/{MachineID}")]
        public ActionResult Index(int MachineID)
        {
            try
            {
                if (!_contextService.InitializeMachineLevel(MachineID))
                    return RedirectToAction("Logout", new { returnUrl = string.Empty, exception = 4 });

                bool isCorrect = _contextService.CheckSecurityParameterApi(MachineID, enCheckParam.Machine);

                if (!isCorrect)
                    return RedirectToAction("Logout", new { returnUrl = string.Empty, exception = 1 });

                _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);

                ContextModel context = _contextService.GetContext();
                MachineViewModel machine = _machineViewService.GetMachine(context);

                return View(machine);
            }
            catch(Exception ex)
            {
                Debugger.Break();
                throw ex;
            }
        }

        [Route("Machine/IgnoreMessage/{MessageID}")]
        public ActionResult IgnoreMessage(int MessageID)
        {           
            bool res = _maintenanceViewService.IgnoreMessage(MessageID);

            ContextModel context = _contextService.GetContext();

            MaintenanceViewModel mvm = _maintenanceViewService.GetMessages(context);

            return Json(mvm);

            
        }

    }
}