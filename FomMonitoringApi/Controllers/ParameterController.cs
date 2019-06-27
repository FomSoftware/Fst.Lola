using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service.API;
using FomMonitoringCore.Service.API.Concrete;
using System.Web.Http;

namespace FomMonitoringApi.Controllers
{
    [AllowAnonymous]
    public class ParameterController : ApiController
    {
        private readonly IXmlDataService _xmlDataService;

        public ParameterController()
        {
            _xmlDataService = new XmlDataService();
        }

        [HttpPost]
        public IHttpActionResult Load(ParametersMachineModel pmm)
        {
            _xmlDataService.AddOrUpdateMachineParameter(pmm);
            return Ok();
        }
    }
}
