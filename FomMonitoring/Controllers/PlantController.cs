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
    [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Administrator + "," + Common.Customer + "," + Common.Demo)]
    public class PlantController : Controller
    {
        private IContextService _contextService;
        private IFaqService _faqService;

        public PlantController (IContextService contextService, IFaqService faqService)
        {
            _contextService = contextService;
            _faqService = faqService;
        }
        // GET: Plant
        [Route("{lang}/PlantManager")]
        public ActionResult PlantManager()
        {
            _contextService.InitializePlantManagerLevel();

            ContextModel context = _contextService.GetContext();
            HeaderViewModel header = SharedViewService.GetHeader(context);
            header.Faqs = _faqService.GetInternalFaqs();
            _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);

            return View(header);
        }
    }
}