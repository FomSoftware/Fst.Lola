﻿using System;
using System.Net;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Service.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http;
using FomMonitoringCore.Queue.Forwarder;

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
                    bool valid = _queueForwarder.Forward(jsonSerialized);
                    if(!valid)
                        return StatusCode(HttpStatusCode.PreconditionFailed);
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



    }
}
