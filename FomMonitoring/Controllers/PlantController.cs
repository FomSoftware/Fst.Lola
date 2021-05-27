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
        private ISharedViewService _sharedViewService;

        public PlantController (IContextService contextService, IFaqService faqService, ISharedViewService sharedViewService)
        {
            _contextService = contextService;
            _faqService = faqService;
            _sharedViewService = sharedViewService;
        }
        // GET: Plant
        [Route("{lang}/PlantManager")]
        public ActionResult PlantManager()
        {
            _contextService.InitializePlantManagerLevel();

            ContextModel context = _contextService.GetContext();
            HeaderViewModel header = _sharedViewService.GetHeader(context);
            header.Faqs = _faqService.GetInternalFaqs();
            _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);

            return View(header);
        }
    }
}