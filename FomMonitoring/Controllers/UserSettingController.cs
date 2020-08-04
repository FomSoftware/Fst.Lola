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

        public UserSettingController(IContextService contextService)
        {
            _contextService = contextService;
        }

        // GET: UserSetting
        public ActionResult Index()
        {
            if (!_contextService.InitializeUserSettingLevel())
                return RedirectToAction("Logout", "Account", new {returnUrl = string.Empty, exception = 6});

            var context = _contextService.GetContext();
            var header = SharedViewService.GetHeader(context);

            _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);
            return View(header);
        }
    }
}