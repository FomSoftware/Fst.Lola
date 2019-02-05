using CommonCore.Service;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace FomMonitoringCore.Service.API.Concrete
{
    public class JsonDataService : IJsonDataService
    {
        public bool AddJsonData(string json, bool isCumulative)
        {
            bool result = false;
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required))
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    JsonData jsonData = new JsonData();
                    jsonData.Json = json;
                    jsonData.IsCumulative = isCumulative;
                    ent.JsonData.Add(jsonData);
                    ent.SaveChanges();
                    transaction.Complete();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format("{0}(json = '{1}', isCumulative = '{2}')", Common.GetStringLog(), json, isCumulative.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public bool ElaborateJsonData(string json)
        {
            bool result = false;
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    JObject jObject = JsonConvert.DeserializeObject<JObject>(json);
                    JToken token = jObject.Root.First;
                    if (token.Path.ToLower() == "currentstate")
                    {
                        dynamic currentStateDynamic = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(token.First)).First();
                        string machineSerial = currentStateDynamic.MachineSerial;
                        Machine machine = ent.Machine.FirstOrDefault(f => f.Serial == machineSerial);
                        if (machine != null)
                        {
                            CurrentState currentState = ent.CurrentState.FirstOrDefault(f => f.MachineId == machine.Id);
                            bool currentStateExists = true;
                            if (currentState == null)
                            {
                                currentState = new CurrentState();
                                currentStateExists = false;
                            }
                            currentState.JobCode = currentStateDynamic.JobCode;
                            currentState.JobProducedPieces = (int?)currentStateDynamic.JobProducedPieces;
                            currentState.JobTotalPieces = (int?)currentStateDynamic.JobTotalPieces;
                            currentState.LastUpdated = (currentStateDynamic.LastUpdated.Value == null || ((DateTime)currentStateDynamic.LastUpdated).Year < 1900 ? (DateTime?)null : (DateTime)currentStateDynamic.LastUpdated);
                            currentState.MachineId = machine.Id;
                            currentState.NextMaintenanceService = (currentStateDynamic.NextMaintenanceService.Value == null || ((DateTime)currentStateDynamic.NextMaintenanceService).Year < 1900 ? (DateTime?)null : (DateTime)currentStateDynamic.NextMaintenanceService);
                            currentState.Operator = currentStateDynamic.Operator;
                            currentState.Plant = currentStateDynamic.Plant;
                            currentState.StateId = (int?)currentStateDynamic.State;
                            currentState.StateTransitionCode = currentStateDynamic.StateTransitionCode;
                            currentState.StateTransitionDescription = currentStateDynamic.StateTransitionDescription;
                            if (!currentStateExists)
                            {
                                ent.CurrentState.Add(currentState);
                            }
                            ent.SaveChanges();
                            result = true;
                        }
                        transaction.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format("{0}(json = '{1}')", Common.GetStringLog(), json);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public bool ResetMachineData(string json)
        {
            bool result = false;
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required))
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    JObject jObject = JsonConvert.DeserializeObject<JObject>(json);
                    JToken token = jObject.Root.First;
                    if (token.Path.ToLower() == "machinereset")
                    {
                        dynamic currentStateDynamic = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(token.First)).First();
                        string machineSerial = currentStateDynamic.MachineSerial;
                        Machine machine = ent.Machine.FirstOrDefault(f => f.Serial == machineSerial);
                        if (machine != null)
                        {
                            result = ent.usp_CleanMachineData(machine.Id).Select(s => s.Value).FirstOrDefault() == 1;
                            ent.SaveChanges();
                        }
                        else
                        {
                            result = true;
                        }
                        transaction.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format("{0}(json = '{1}')", Common.GetStringLog(), json);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }
    }
}
