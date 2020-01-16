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
        private readonly IContextService _contextService;
        private readonly IPlantMessagesViewService _plantMessagesViewService;
        private readonly IMachineViewService _machineViewService;
        private readonly IMesViewService _mesViewService;
        private readonly INotificationViewService _notificationManagerViewService;
        private readonly IPanelParametersViewService _panelParametersViewService;
        private readonly IMessagesViewService _messagesViewService;
        private readonly IEfficiencyViewService _efficiencyViewService;
        private readonly IProductivityViewService _productivityViewService;
        private readonly IJobsViewService _jobsViewService;
        private readonly IMaintenanceViewService _maintenanceViewService;
        private readonly IXToolsViewService _xToolsViewService;
        private readonly ISpindleViewService _spindleViewService;
        private readonly IToolsViewService _toolsViewService;

        public AppApiController(
            IMessagesViewService messagesViewService,
            IPlantMessagesViewService plantMessagesViewService, 
            IMachineViewService machineViewService,
            IContextService contextService,
            IMesViewService mesViewService,
            INotificationViewService notificationManagerViewService,
            IPanelParametersViewService panelParametersViewService,
            IEfficiencyViewService efficiencyViewService,
            IProductivityViewService productivityViewService,
            IJobsViewService jobsViewService,
            IMaintenanceViewService maintenanceViewService,
            IXToolsViewService xToolsViewService,
            IToolsViewService toolsViewService,
            ISpindleViewService spindleViewService)
        {
            _contextService = contextService;
            _plantMessagesViewService = plantMessagesViewService;
            _machineViewService = machineViewService;
            _mesViewService = mesViewService;
            _notificationManagerViewService = notificationManagerViewService;
            _panelParametersViewService = panelParametersViewService;
            _messagesViewService = messagesViewService;
            _efficiencyViewService = efficiencyViewService;
            _productivityViewService = productivityViewService;
            _jobsViewService = jobsViewService;
            _maintenanceViewService = maintenanceViewService;
            _xToolsViewService = xToolsViewService;
            _spindleViewService = spindleViewService;
            _toolsViewService = toolsViewService;
        }

        [HttpPost]
        [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetMachineViewModel")]
        public HttpResponseMessage GetMachineViewModel(FilterViewModel filters)
        {
            if (filters.machine != null)
            {
                var isCorrect = _contextService.CheckSecurityParameterApi(filters.machine.id, enCheckParam.Machine);

                if (!isCorrect)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);

                _contextService.SetActualMachine(filters.machine.id);
            }

            if (filters.period != null)
                _contextService.SetActualPeriod(filters.period.start, filters.period.end);

            _contextService.CheckLastUpdate();

            var context = _contextService.GetContext();
            var machine = _machineViewService.GetMachine(context);

            return Request.CreateResponse(HttpStatusCode.OK, machine, MediaTypeHeaderValue.Parse("application/json"));
        }


        [HttpPost]
        [Authorize(Roles = Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetMesViewModel")]
        public HttpResponseMessage GetMesViewModel([FromBody]int plantID)
        {
            var isCorrect = _contextService.CheckSecurityParameterApi(plantID, enCheckParam.Plant);

            if (!isCorrect)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            _contextService.SetActualPlant(plantID);

            var context = _contextService.GetContext();
            var mes = _mesViewService.GetMes(context);

            return Request.CreateResponse(HttpStatusCode.OK, mes, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.HeadWorkshop + "," + Common.Operator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetNotificationsViewModel")]
        public HttpResponseMessage GetNotificationsViewModel()
        {
          
            var context = _contextService.GetContext();
            var notification = _notificationManagerViewService.GetNotification(context);

            return Request.CreateResponse(HttpStatusCode.OK, notification, MediaTypeHeaderValue.Parse("application/json"));
        }


        [HttpPost]
        [Authorize(Roles = Common.HeadWorkshop + "," + Common.Operator + "," + Common.Customer)]
        [Route("ajax/AppApi/SetNotificationRead")]
        public HttpResponseMessage SetNotificationRead([FromBody]int idNotification)
        {

            var context = _contextService.GetContext();
            _notificationManagerViewService.SetNotificationAsRead(idNotification, context);

            return Request.CreateResponse(HttpStatusCode.OK, new {}, MediaTypeHeaderValue.Parse("application/json"));
        }


        [HttpPost]
        [Authorize(Roles = Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetPlantMessagesViewModel")]
        public HttpResponseMessage GetPlantMessagesViewModel(FilterViewModel filters)
        {
            if (filters.period != null)
                _contextService.SetActualPeriod(filters.period.start, filters.period.end);

            var context = _contextService.GetContext();
            var mes = _plantMessagesViewService.GetPlantMessages(context);

            return Request.CreateResponse(HttpStatusCode.OK, mes, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetMachineEfficiencyViewModel")]
        public HttpResponseMessage GetMachineEfficiencyViewModel(FilterViewModel filters)
        {
            if (filters.period != null)
            {
                _contextService.SetActualPeriod(filters.period.start, filters.period.end);
                _contextService.SetActualMachineGroup(filters.machineGroup);
            }
            
            var context = _contextService.GetContext();
            var efficiency = _efficiencyViewService.GetEfficiency(context);

            return Request.CreateResponse(HttpStatusCode.OK, efficiency, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetMachineProductivityViewModel")]
        public HttpResponseMessage GetMachineProductivityViewModel(FilterViewModel filters)
        {
            if (filters.period != null)
            {
                _contextService.SetActualPeriod(filters.period.start, filters.period.end);
                _contextService.SetActualMachineGroup(filters.machineGroup);
            }

            var context = _contextService.GetContext();
            var productivity = _productivityViewService.GetProductivity(context);

            return Request.CreateResponse(HttpStatusCode.OK, productivity, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetMachineJobViewModel")]
        public HttpResponseMessage GetMachineJobViewModel(FilterViewModel filters)
        {
            if (filters.period != null)
            {
                _contextService.SetActualPeriod(filters.period.start, filters.period.end);
                _contextService.SetActualMachineGroup(filters.machineGroup);
            }

            var context = _contextService.GetContext();
            var jobs = _jobsViewService.GetJobs(context);

            return Request.CreateResponse(HttpStatusCode.OK, jobs, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetMachineMaintenanceViewModel")]
        public HttpResponseMessage GetMachineMaintenanceViewModel(FilterViewModel filters)
        {
            if (filters.period != null)
            {
                _contextService.SetActualPeriod(filters.period.start, filters.period.end);
                _contextService.SetActualMachineGroup(filters.machineGroup);
            }

            var context = _contextService.GetContext();
            var maintenance = _maintenanceViewService.GetMessages(context);

            return Request.CreateResponse(HttpStatusCode.OK, maintenance, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetMachineParametersViewModel")]
        public HttpResponseMessage GetMachineParametersViewModel(FilterViewModel filters)
        {
            if (filters.period != null)
            {
                _contextService.SetActualPeriod(filters.period.start, filters.period.end);
                _contextService.SetActualMachineGroup(filters.machineGroup);
            }

            var context = _contextService.GetContext();
            var parameters = _panelParametersViewService.GetParameters(context);

            return Request.CreateResponse(HttpStatusCode.OK, parameters, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetMachineMessagesViewModel")]
        public HttpResponseMessage GetMachineMessagesViewModel(FilterViewModel filters)
        {
            if (filters.period != null)
            {
                _contextService.SetActualPeriod(filters.period.start, filters.period.end);
                _contextService.SetActualMachineGroup(filters.machineGroup);
            }
               

            var context = _contextService.GetContext();
            var messages = _messagesViewService.GetMessages(context);

            return Request.CreateResponse(HttpStatusCode.OK, messages, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetParametersValueViewModel")]
        public HttpResponseMessage GetParametersValueViewModel(FilterViewModel filters)
        {

            var context = _contextService.GetContext();
            if (filters.panelId == (int) enPanel.Multispindle)
            {
                var result = _panelParametersViewService.GetMultiSpindleVueModel(context.ActualMachine, filters.cluster);

                return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);

        }

        [HttpPost]
        [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetMachineXToolsViewModel")]
        public HttpResponseMessage GetMachineXToolsViewModel(FilterViewModel filters)
        {

            if (filters.period != null)
            {
                _contextService.SetActualPeriod(filters.period.start, filters.period.end);
                _contextService.SetActualMachineGroup(filters.machineGroup);
            }

            var context = _contextService.GetContext();
            var xTools = _xToolsViewService.GetXTools(context);

            return Request.CreateResponse(HttpStatusCode.OK, xTools, MediaTypeHeaderValue.Parse("application/json"));

        }


        [HttpPost]
        [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetMachineXSpindlesViewModel")]
        public HttpResponseMessage GetMachineXSpindlesViewModel(FilterViewModel filters)
        {

            if (filters.period != null)
            {
                _contextService.SetActualPeriod(filters.period.start, filters.period.end);
                _contextService.SetActualMachineGroup(filters.machineGroup);
            }

            var context = _contextService.GetContext();
            var xSpindles = _spindleViewService.GetXSpindles(context);

            return Request.CreateResponse(HttpStatusCode.OK, xSpindles, MediaTypeHeaderValue.Parse("application/json"));

        }

        [HttpPost]
        [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetMachineToolsViewModel")]
        public HttpResponseMessage GetMachineToolsViewModel(FilterViewModel filters)
        {
            if (filters.period != null)
            {
                _contextService.SetActualPeriod(filters.period.start, filters.period.end);
                _contextService.SetActualMachineGroup(filters.machineGroup);
            }

            var context = _contextService.GetContext();
            var tools = _toolsViewService.GetTools(context);

            return Request.CreateResponse(HttpStatusCode.OK, tools, MediaTypeHeaderValue.Parse("application/json"));

        }

        [HttpPost]
        [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator + "," + Common.Customer)]
        [Route("ajax/AppApi/GetMachineToolsBlitzViewModel")]
        public HttpResponseMessage GetMachineToolsBlitzViewModel(FilterViewModel filters)
        {
            if (filters.period != null)
            {
                _contextService.SetActualPeriod(filters.period.start, filters.period.end);
                _contextService.SetActualMachineGroup(filters.machineGroup);
            }

            var context = _contextService.GetContext();
            var xTools = _xToolsViewService.GetXTools(context);

            return Request.CreateResponse(HttpStatusCode.OK, xTools, MediaTypeHeaderValue.Parse("application/json"));

        }
    }
}