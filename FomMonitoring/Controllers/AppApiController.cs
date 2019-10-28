using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace FomMonitoring.Controllers
{
    [SessionApi]
    public class AppApiController : ApiController
    {
        private IContextService _contextService;
        private IPlantMessagesViewService _plantMessagesViewService;
        private IMachineViewService _machineViewService;
        private IMesViewService _mesViewService;

        public AppApiController(
            IPlantMessagesViewService plantMessagesViewService, 
            IMachineViewService machineViewService,
            IContextService contextService,
            IMesViewService mesViewService)
        {
            _contextService = contextService;
            _plantMessagesViewService = plantMessagesViewService;
            _machineViewService = machineViewService;
            _mesViewService = mesViewService;
        }

        [HttpPost]
        [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetMachineViewModel")]
        public HttpResponseMessage GetMachineViewModel(FilterViewModel filters)
        {
            if (filters.machine != null)
            {
                bool isCorrect = _contextService.CheckSecurityParameterApi(filters.machine.id, enCheckParam.Machine);

                if (!isCorrect)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);

                _contextService.SetActualMachine(filters.machine.id);
            }

            if (filters.period != null)
                _contextService.SetActualPeriod(filters.period.start, filters.period.end);

            _contextService.CheckLastUpdate();

            ContextModel context = _contextService.GetContext();
            MachineViewModel machine = _machineViewService.GetMachine(context);

            return Request.CreateResponse(HttpStatusCode.OK, machine, MediaTypeHeaderValue.Parse("application/json"));
        }


        [HttpPost]
        [Authorize(Roles = Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetMesViewModel")]
        public HttpResponseMessage GetMesViewModel([FromBody]int plantID)
        {
            bool isCorrect = _contextService.CheckSecurityParameterApi(plantID, enCheckParam.Plant);

            if (!isCorrect)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            _contextService.SetActualPlant(plantID);

            ContextModel context = _contextService.GetContext();
            MesViewModel mes = _mesViewService.GetMes(context);

            return Request.CreateResponse(HttpStatusCode.OK, mes, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetPlantMessagesViewModel")]
        public HttpResponseMessage GetPlantMessagesViewModel(FilterViewModel filters)
        {
            if (filters.period != null)
                _contextService.SetActualPeriod(filters.period.start, filters.period.end);

            ContextModel context = _contextService.GetContext();
            PlantMessagesViewModel mes = _plantMessagesViewService.GetPlantMessages(context);

            return Request.CreateResponse(HttpStatusCode.OK, mes, MediaTypeHeaderValue.Parse("application/json"));
        }
    }
}