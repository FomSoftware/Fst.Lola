﻿using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Web.Mvc;

namespace FomMonitoring.Controllers
{
    [SessionWeb]
    [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Administrator + "," + Common.Customer + "," + Common.Demo + "," + Common.Assistance + "," + Common.RandD)]
    public class SharedController : Controller
    {
        private readonly IContextService _contextService;
        private readonly IFaqService _faqService;
        private readonly ISharedViewService _sharedViewService;

        public SharedController(IContextService contextService, IFaqService faqService, ISharedViewService sharedViewService)
        {
            _contextService = contextService;
            _faqService = faqService;
            _sharedViewService = sharedViewService;
        }

        public ActionResult _Header()
        {
            ContextModel context = _contextService.GetContext();
            HeaderViewModel header = _sharedViewService.GetHeader(context);
            header.Faqs = _faqService.GetInternalFaqs();
            return PartialView(header);
        }

        public ActionResult _Toolbar()
        {
            ContextModel context = _contextService.GetContext();
            ToolbarViewModel toolbar = SharedViewService.GetToolbar(context);

            //if(context.ActualPage == enPage.Machine)
            //    return PartialView(toolbar);
            //else
            //    return null;

            return PartialView(toolbar);
        }

        [ChildActionOnly]
        public ActionResult GetHtmlPage(string path)
        {
            path = Server.MapPath(path);
            return new FilePathResult(path, "text/html");
        }
    }
}