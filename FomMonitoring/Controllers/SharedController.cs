using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Web.Mvc;

namespace FomMonitoring.Controllers
{
    [SessionWeb]
    [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
    public class SharedController : Controller
    {
        public ActionResult _Header()
        {
            ContextModel context = ContextService.GetContext();
            HeaderViewModel header = SharedViewService.GetHeader(context);

            return PartialView(header);
        }

        public ActionResult _Toolbar()
        {
            ContextModel context = ContextService.GetContext();
            ToolbarViewModel toolbar = SharedViewService.GetToolbar(context);

            //if(context.ActualPage == enPage.Machine)
            //    return PartialView(toolbar);
            //else
            //    return null;

            return PartialView(toolbar);
        }
    }
}