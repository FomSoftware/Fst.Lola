using FomMonitoringCore.Framework.Common;
using System.Web.Mvc;

namespace FomMonitoring.Controllers
{
    [SessionWeb]
    [Authorize(Roles = Common.Administrator)]
    public class AdminController : Controller
    {
        // GET: Account
        // [Route("{lang}/UserManager")]
        public ActionResult UserManager()
        {
            return View();
        }
    }
}