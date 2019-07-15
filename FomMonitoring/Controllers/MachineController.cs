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
        public ActionResult Index()
        {
            if (!ContextService.InitializeMachineLevel())
                return RedirectToAction("Logout", new { returnUrl = string.Empty, exception = 4 });

            ContextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);

            ContextModel context = ContextService.GetContext();
            MachineViewModel machine = MachineViewService.GetMachine(context);

            return View(machine);
        }

        [Route("{lang}/Machine/Index/{MachineID}")]
        public ActionResult Index(int MachineID)
        {
            try
            {
                if (!ContextService.InitializeMachineLevel(MachineID))
                    return RedirectToAction("Logout", new { returnUrl = string.Empty, exception = 4 });

                bool isCorrect = ContextService.CheckSecurityParameterApi(MachineID, enCheckParam.Machine);

                if (!isCorrect)
                    return RedirectToAction("Logout", new { returnUrl = string.Empty, exception = 1 });

                ContextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);

                ContextModel context = ContextService.GetContext();
                MachineViewModel machine = MachineViewService.GetMachine(context);

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
            bool res = MaintenanceViewService.IgnoreMessage(MessageID);

            ContextModel context = ContextService.GetContext();

            MaintenanceViewModel mvm = MaintenanceViewService.GetMessages(context);

            return Json(mvm);

            
        }

    }
}