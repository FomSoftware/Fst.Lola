using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Web.Mvc;

namespace FomMonitoring.Controllers
{
    [SessionWeb]
    [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
    public class AdminController : Controller
    {
        // GET: Account
        [Route("{lang}/UserManager")]
        public ActionResult UserManager()
        {
            ContextModel context = ContextService.GetContext();
            HeaderViewModel header = SharedViewService.GetHeader(context);

            return View(header);
        }
    }
}