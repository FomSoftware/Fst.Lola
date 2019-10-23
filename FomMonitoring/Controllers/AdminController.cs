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
    [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
    public class AdminController : Controller
    {
        private IContextService _contextService;

        public AdminController (IContextService contextService)
        {
            _contextService = contextService;
        }
        // GET: Account
        [Route("{lang}/UserManager")]
        public ActionResult UserManager()
        {
            _contextService.InitializeAdminLevel();

            ContextModel context = _contextService.GetContext();
            HeaderViewModel header = SharedViewService.GetHeader(context);

            _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);

            return View(header);
        }
    }
}