using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace FomMonitoring.Controllers
{
    [SessionWeb]
    [Authorize(Roles = Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
    public class MesController : Controller
    {
        public ActionResult Index()
        {
            if (!ContextService.InitializeMesLevel())
                return RedirectToAction("Logout", new { returnUrl = string.Empty, exception = 3 });

            ContextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);

            ContextModel context = ContextService.GetContext();
            MesViewModel mes = MesViewService.GetMes(context);

            return View("Mes", mes);
        }

        
        public ActionResult PlantMessages()
        {
            ContextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);
            ContextModel context = ContextService.GetContext();
            context.ActualPage = enPage.PlantMessages;
            PlantMessagesViewModel mes = PlantMessagesViewService.GetPlantMessages(context);

            return View("PlantMessages", mes);

        }


    }
}