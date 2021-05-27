using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Service;

namespace FomMonitoring.Controllers
{
    [SessionWeb]
    public class UserSettingController : Controller
    {
        private readonly IContextService _contextService;
        private readonly IFaqService _faqService;
        private readonly ISharedViewService _sharedViewService;

        public UserSettingController(IContextService contextService, IFaqService faqService, ISharedViewService sharedViewService)
        {
            _contextService = contextService;
            _faqService = faqService;
            _sharedViewService = sharedViewService;
        }

        // GET: UserSetting
        public ActionResult Index()
        {
            if (!_contextService.InitializeUserSettingLevel())
                return RedirectToAction("Logout", "Account", new {returnUrl = string.Empty, exception = 6});

            var context = _contextService.GetContext();
            var header = _sharedViewService.GetHeader(context);
            header.Faqs = _faqService.GetInternalFaqs();
            _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);
            return View(header);
        }
    }
}