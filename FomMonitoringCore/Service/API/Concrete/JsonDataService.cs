using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.Service.API.Concrete
{
    public class JsonDataService : IJsonDataService
    {
        private readonly IFomMonitoringEntities _context;

        public JsonDataService(IFomMonitoringEntities context)
        {
            _context = context;
        }

        public bool AddJsonData(string json, bool isCumulative)
        {
            var result = false;
            try
            {
                var jsonData = new JsonData
                {
                    Json = json,
                    IsCumulative = isCumulative
                };
                _context.Set<JsonData>().Add(jsonData);
                _context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), json, isCumulative.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public bool ElaborateJsonData(string json)
        {
            var result = false;
            try
            {

                    var jObject = JsonConvert.DeserializeObject<JObject>(json);
                    var token = jObject.Root.First;
                    if (token.Path.ToLower() == "currentstate")
                    {
                        var currentStateDynamic = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(token.First)).First();
                        string machineSerial = currentStateDynamic.MachineSerial;
                        var machine = _context.Set<Machine>().FirstOrDefault(f => f.Serial == machineSerial);
                        if (machine != null)
                        {
                            var currentState = _context.Set<CurrentState>().FirstOrDefault(f => f.MachineId == machine.Id);
                            var currentStateExists = true;
                            if (currentState == null)
                            {
                                currentState = new CurrentState();
                                currentStateExists = false;
                            }
                            currentState.JobCode = currentStateDynamic.JobCode;
                            currentState.JobProducedPieces = (int?)currentStateDynamic.JobProducedPieces;
                            currentState.JobTotalPieces = (int?)currentStateDynamic.JobTotalPieces;
                            currentState.LastUpdated = (currentStateDynamic.LastUpdated.Value is string || currentStateDynamic.LastUpdated.Value == null || ((DateTime)currentStateDynamic.LastUpdated).Year < 1900 ? (DateTime?)null : (DateTime)currentStateDynamic.LastUpdated);
                            currentState.MachineId = machine.Id;
                            currentState.NextMaintenanceService = (currentStateDynamic.NextMaintenanceService.Value is string || currentStateDynamic.NextMaintenanceService.Value == null || ((DateTime)currentStateDynamic.NextMaintenanceService).Year < 1900 ? (DateTime?)null : (DateTime)currentStateDynamic.NextMaintenanceService);
                            currentState.Operator = currentStateDynamic.Operator;
                            currentState.Plant = currentStateDynamic.Plant;
                            currentState.StateId = (int?)currentStateDynamic.State;
                            currentState.StateTransitionCode = currentStateDynamic.StateTransitionCode;
                            currentState.StateTransitionDescription = currentStateDynamic.StateTransitionDescription;
                            if (!currentStateExists)
                            {
                                _context.Set<CurrentState>().Add(currentState);
                            }
                            _context.SaveChanges();
                            result = true;
                        }
                    
                }
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), json);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public bool ResetMachineData(string json)
        {
            var result = false;
            try
            {

                    var jObject = JsonConvert.DeserializeObject<JObject>(json);
                    var token = jObject.Root.First;
                    if (token.Path.ToLower() == "machinereset")
                    {
                        var currentStateDynamic = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(token.First)).First();
                        string machineSerial = currentStateDynamic.MachineSerial;
                        var machine = _context.Set<Machine>().FirstOrDefault(f => f.Serial == machineSerial);
                        if (machine != null)
                        {
                            result = _context.usp_CleanMachineData(machine.Id).Select(s => s.Value).FirstOrDefault() == 1;
                            _context.SaveChanges();
                        }
                        else
                        {
                            result = true;
                        }
                    }
                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), json);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }
    }
}
