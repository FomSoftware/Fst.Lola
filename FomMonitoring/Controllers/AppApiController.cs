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
    [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator)]
    public class AppApiController : ApiController
    {
        [HttpPost]
        [Route("ajax/AppApi/GetMachineViewModel")]
        public HttpResponseMessage GetMachineViewModel(FilterViewModel filters)
        {
            if(filters.machine != null)
            {
                bool isCorrect = ContextService.CheckSecurityParameterApi(filters.machine.id, enCheckParam.Machine);

                if (!isCorrect)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);

                ContextService.SetActualMachine(filters.machine.id);
            }
         
            if (filters.period != null)
                ContextService.SetActualPeriod(filters.period.start, filters.period.end);

            ContextService.CheckLastUpdate();

            ContextModel context = ContextService.GetContext();
            MachineViewModel machine = MachineViewService.GetMachine(context);

            return Request.CreateResponse(HttpStatusCode.OK, machine, MediaTypeHeaderValue.Parse("application/json"));
        }


        [HttpPost]
        [Route("ajax/AppApi/GetMesViewModel")]
        public HttpResponseMessage GetMesViewModel([FromBody]int plantID)
        {
            bool isCorrect = ContextService.CheckSecurityParameterApi(plantID, enCheckParam.Plant);

            if (!isCorrect)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            ContextService.SetActualPlant(plantID);

            ContextModel context = ContextService.GetContext();
            MesViewModel mes = MesViewService.GetMes(context);

            return Request.CreateResponse(HttpStatusCode.OK, mes, MediaTypeHeaderValue.Parse("application/json"));
        }
    }
}