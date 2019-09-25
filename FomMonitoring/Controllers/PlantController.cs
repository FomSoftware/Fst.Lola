using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FomMonitoring.Controllers
{
    [SessionWeb]
    [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Administrator + "," + Common.Customer)]
    public class PlantController : Controller
    {
        // GET: Plant
        [Route("{lang}/PlantManager")]
        public ActionResult PlantManager()
        {
            ContextService.InitializeAdminLevel();

            ContextModel context = ContextService.GetContext();
            HeaderViewModel header = SharedViewService.GetHeader(context);

            ContextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);

            return View(header);
        }
    }
}