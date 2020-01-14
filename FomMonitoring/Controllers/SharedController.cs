using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Web.Mvc;

namespace FomMonitoring.Controllers
{
    [SessionWeb]
    [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Administrator + "," + Common.Customer)]
    public class SharedController : Controller
    {
        private IContextService _contextService;

        public SharedController(IContextService contextService)
        {
            _contextService = contextService;
        }

        public ActionResult _Header()
        {
            ContextModel context = _contextService.GetContext();
            HeaderViewModel header = SharedViewService.GetHeader(context);

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