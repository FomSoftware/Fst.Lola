using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Framework.Model.Xml;
using FomMonitoringCore.Service.API;
using FomMonitoringCore.Service.API.Concrete;
using System.Threading.Tasks;
using System.Web.Http;

namespace FomMonitoringApi.Controllers
{
    [JwtAuthentication]
    public class ParameterController : ApiController
    {
        private readonly IXmlDataService _xmlDataService;

        public ParameterController(IXmlDataService xmlDataService)
        {
            _xmlDataService = xmlDataService;
        }

        [HttpPost]
        public async Task<IHttpActionResult> Load(ParametersMachineModelXml pmm)
        {
            await _xmlDataService.AddOrUpdateMachineParameterAsync(pmm);
            return Ok();
        }
    }
}
