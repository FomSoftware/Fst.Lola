using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Service.API;
using FomMonitoringCore.Service.API.Concrete;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http;

namespace FomMonitoringApi.Controllers
{
    [JwtAuthentication]
    public class UpdateController : ApiController
    {
        private readonly IJsonDataService _jsonDataService;

        public UpdateController()
        {
            _jsonDataService = new JsonDataService();
        }

        public UpdateController(IJsonDataService jsonDataService)
        {
            _jsonDataService = jsonDataService;
        }

        [HttpPost]
        public object Machine(object data)
        {
            bool result = false;
            JObject json = new JObject();
            if (data != null)
            {
                string jsonSerialized = JsonConvert.SerializeObject(data);
                result = _jsonDataService.AddJsonData(jsonSerialized, false);
                json = JObject.Parse(jsonSerialized);
            }
            json.AddFirst(new JProperty("imported", result));
            return Json(json);
        }

        [HttpPost]
        public object MachineCumulative(object data)
        {
            bool result = false;
            JObject json = new JObject();
            if (data != null)
            {
                string jsonSerialized = JsonConvert.SerializeObject(data);
                result = _jsonDataService.AddJsonData(jsonSerialized, true);
                json = JObject.Parse(jsonSerialized);
            }
            json.AddFirst(new JProperty("imported", result));
            return Json(json);
        }

        [HttpPost]
        public object MachineRealTime(object data)
        {
            bool result = false;
            JObject json = new JObject();
            if (data != null)
            {
                string jsonSerialized = JsonConvert.SerializeObject(data);
                result = _jsonDataService.ElaborateJsonData(jsonSerialized);
                json = JObject.Parse(jsonSerialized);
            }
            json.AddFirst(new JProperty("imported", result));
            return Json(json);
        }

        [HttpPost]
        public object MachineReset(object data)
        {
            bool result = false;
            JObject json = new JObject();
            if (data != null)
            {
                string jsonSerialized = JsonConvert.SerializeObject(data);
                result = _jsonDataService.ResetMachineData(jsonSerialized);
                json = JObject.Parse(jsonSerialized);
            }
            json.AddFirst(new JProperty("isReset", result));
            return Json(json);
        }

        [HttpPost]
        private object Client(object data)
        {
            return Json(data);
        }
    }
}
