using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace FomMonitoring.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        [AllowAnonymous]
        [Route("{lang}/Error/{error}")]
        public ActionResult Index(int error)
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)RouteData.Values["lang"]);
            ViewBag.Error = error.ToString();
            return View();
        }
    }
}