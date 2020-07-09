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
    [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Administrator + "," + Common.Customer)]
    public class PlantController : Controller
    {
        private IContextService _contextService;

        public PlantController (IContextService contextService)
        {
            _contextService = contextService;
        }
        // GET: Plant
        [Route("{lang}/PlantManager")]
        public ActionResult PlantManager()
        {
            _contextService.InitializePlantManagerLevel();

            ContextModel context = _contextService.GetContext();
            HeaderViewModel header = SharedViewService.GetHeader(context);

            _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);

            return View(header);
        }
    }
}