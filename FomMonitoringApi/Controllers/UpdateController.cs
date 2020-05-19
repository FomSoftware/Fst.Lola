using System;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Service.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http;
using FomMonitoringCoreQueue.Forwarder;

namespace FomMonitoringApi.Controllers
{
    [JwtAuthentication]
    public class UpdateController : ApiController
    {
        private readonly IJsonDataService _jsonDataService;
        private readonly IQueueForwarder _queueForwarder;

        public UpdateController(IJsonDataService jsonDataService, IQueueForwarder queueForwarder)
        {
            _jsonDataService = jsonDataService;
            _queueForwarder = queueForwarder;
        }

        [HttpPost]
        public object Machine(object data)
        {
            try
            {

                var json = new JObject();
                if (data != null)
                {
                    var jsonSerialized = JsonConvert.SerializeObject(data);
                    _queueForwarder.Forward(jsonSerialized);
                }

                json.AddFirst(new JProperty("imported", true));
                return Json(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public object MachineCumulative(object data)
        {
            var result = false;
            var json = new JObject();
            if (data != null)
            {
                var jsonSerialized = JsonConvert.SerializeObject(data);
                result = _jsonDataService.AddJsonData(jsonSerialized, true);
                json = JObject.Parse(jsonSerialized);
            }
            json.AddFirst(new JProperty("imported", result));
            return Json(json);
        }

        [HttpPost]
        public object MachineRealTime(object data)
        {
            var result = false;
            var json = new JObject();
            if (data != null)
            {
                var jsonSerialized = JsonConvert.SerializeObject(data);
                result = _jsonDataService.ElaborateJsonData(jsonSerialized);
                json = JObject.Parse(jsonSerialized);
            }
            json.AddFirst(new JProperty("imported", result));
            return Json(json);
        }

        [HttpPost]
        public object MachineReset(object data)
        {
            var result = false;
            var json = new JObject();
            if (data != null)
            {
                var jsonSerialized = JsonConvert.SerializeObject(data);
                result = _jsonDataService.ResetMachineData(jsonSerialized);
                json = JObject.Parse(jsonSerialized);
            }
            json.AddFirst(new JProperty("isReset", result));
            return Json(json);
        }


    }
}
