﻿using FomMonitoringCore.Framework.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringCore.Service.API.Concrete
{
    public class JsonDataService : IJsonDataService
    {
        private readonly IFomMonitoringEntities _context;

        public JsonDataService(IFomMonitoringEntities context)
        {
            _context = context;
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
                            currentState.Operator = currentStateDynamic.Operator;
                            currentState.StateId = (int?)currentStateDynamic.State;
                            currentState.StateTransitionCode = currentStateDynamic.StateTransitionCode;
                            currentState.ResidueWorkingTime = currentStateDynamic.ResidueWorkingTime;
                            currentState.ResidueWorkingTimeJob = currentStateDynamic.ResidueWorkingTimeJob;
                            currentState.ResidueWorkingTimeBar = currentStateDynamic.ResidueWorkingTimeBar;
                            if (!currentStateExists)
                            {
                                _context.Set<CurrentState>().Add(currentState);
                            }

                            machine.LastUpdate = currentState.LastUpdated;
                            _context.SaveChanges();
                            result = true; 

                            /*var historyJobExist = _context.Set<HistoryJob>().Any(h =>
                                h.Code == currentState.JobCode && h.MachineId == machine.Id && h.Day != null &&
                                DbFunctions.TruncateTime(h.Day.Value) == DbFunctions.TruncateTime(currentState.LastUpdated));
                            if (!historyJobExist && currentState.JobCode?.Trim().Length > 0 && currentState.JobProducedPieces > 0)
                            {
                                var newJob = new HistoryJob
                                {
                                    Day = DateTime.UtcNow.Date,
                                    Code = currentState.JobCode,
                                    ElapsedTime = 0,
                                    MachineId = machine.Id,
                                    Period = (DateTime.UtcNow.Year * 10000) + (DateTime.UtcNow.Month * 100) + (DateTime.UtcNow.Day),
                                    PiecesProduced = currentState.JobProducedPieces,
                                    TotalPieces = currentState.JobTotalPieces,
                                    TypeHistory = "d"
                                };
                                _context.Set<HistoryJob>().Add(newJob);
                                _context.SaveChanges();
                            }*/

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
