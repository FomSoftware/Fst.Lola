﻿using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace FomMonitoring.Controllers
{
    [SessionApi]
    public class PlantManagerApiController : ApiController
    {
        private IContextService _contextService;
        private IPlantManagerViewService _plantService;

        public PlantManagerApiController(IContextService contextService, IPlantManagerViewService plantService)
        {
            _contextService = contextService;
            _plantService = plantService;
        }


        [HttpGet]
        [Authorize]
        [Route("ajax/PlantManagerApi/GetPlants")]
        public HttpResponseMessage GetPlants()
        {
            ContextModel context = _contextService.GetContext();
            PlantManagerViewModel plantManager = new PlantManagerViewModel();
            plantManager = _plantService.GetPlants(context);
            return Request.CreateResponse(HttpStatusCode.OK, plantManager, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpGet]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/PlantManagerApi/GetPlant/{id}")]
        public HttpResponseMessage GetPlant(int id)
        {
            PlantManagerViewModel plant = new PlantManagerViewModel();
            plant = _plantService.GetPlant(id);
            return Request.CreateResponse(HttpStatusCode.OK, plant, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpGet]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/PlantManagerApi/GetPlantByMachine/{idMachine}")]
        public HttpResponseMessage GetPlantByMachine(int idMachine)
        {
            ContextModel context = _contextService.GetContext();
            PlantManagerViewModel plant = new PlantManagerViewModel();
            plant = _plantService.GetPlantByMachine(idMachine);
            return Request.CreateResponse(HttpStatusCode.OK, plant, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpGet]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/PlantManagerApi/GetCustomers")]
        public HttpResponseMessage GetCustomers()
        {
            ContextModel context = _contextService.GetContext();
            List<string> customers = new List<string>();
            return Request.CreateResponse(HttpStatusCode.OK, customers, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/PlantManagerApi/InsertPlant")]
        public HttpResponseMessage InsertPlant(PlantViewModel plant)
        {
            try
            {
                ContextModel context = _contextService.GetContext();
                var result = _plantService.CreatePlant(plant, context);
                return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
            }
            catch (InvalidOperationException ex)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, ex, MediaTypeHeaderValue.Parse("application/json"));
            }
        }

        [HttpGet]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/PlantManagerApi/GetPlantsByCustomer/{id}")]
        public HttpResponseMessage GetPlantsByCustomer(string id)
        {
            var result = _plantService.GetPlantsByCustomer(id);
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpGet]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/PlantManagerApi/GetMachinesByPlant/{id}")]
        public HttpResponseMessage GetMachinesByPlant(int id)
        {
            var result = _plantService.GetMachinesByPlant(id);
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPut]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/PlantManagerApi/EditPlant")]
        public HttpResponseMessage EditPlant(PlantViewModel plant)
        {
            var result = _plantService.EditPlant(plant);
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpDelete]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/PlantManagerApi/DeletePlant/{id}")]
        public HttpResponseMessage DeletePlant(int id)
        {
            var result = _plantService.DeletePlant(id);
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }
    }
}