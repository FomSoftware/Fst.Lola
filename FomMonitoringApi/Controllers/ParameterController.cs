using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Framework.Model.Xml;
using FomMonitoringCore.Service.API;
using FomMonitoringCore.Service.API.Concrete;
using System.Web.Http;

namespace FomMonitoringApi.Controllers
{
    [JwtAuthentication]
    public class ParameterController : ApiController
    {
        private readonly IXmlDataService _xmlDataService;

        public ParameterController()
        {
            _xmlDataService = new XmlDataService();
        }

        [HttpPost]
        public IHttpActionResult Load(ParametersMachineModelXml pmm)
        {
            _xmlDataService.AddOrUpdateMachineParameter(pmm);
            return Ok();
        }
    }
}
