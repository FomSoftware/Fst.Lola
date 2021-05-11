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
    [Authorize(Roles = Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer + "," + Common.Demo)]
    public class MesController : Controller
    {
        private IPlantMessagesViewService _plantMessagesViewService;
        private IContextService _contextService;
        private IMesViewService _mesViewService;

        public MesController(IPlantMessagesViewService plantMessagesViewService, IContextService contextService, IMesViewService mesViewService)
        {
            _plantMessagesViewService = plantMessagesViewService;
            _contextService = contextService;
            _mesViewService = mesViewService;
        }

        public ActionResult Index()
        {
            if (!_contextService.InitializeMesLevel())
            {
                //sbagliato, la action Logout su MesController non esiste
                return RedirectToAction("Logout", "Account", new { returnUrl = string.Empty, exception = 3 });
            }
            _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);

            ContextModel context = _contextService.GetContext();
            MesViewModel mes = _mesViewService.GetMes(context);

            return View("Mes", mes);
        }


        public ActionResult PlantMessages()
        {
            _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);
            ContextModel context = _contextService.GetContext();
            context.ActualPage = enPage.PlantMessages;
            PlantMessagesViewModel mes = _plantMessagesViewService.GetPlantMessages(context);

            return View("PlantMessages", mes);

        }


    }
}