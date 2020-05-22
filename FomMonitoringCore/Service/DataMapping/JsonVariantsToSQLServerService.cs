using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.DAL;
using FomMonitoringCore.DAL_SQLite;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Uow;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MessageMachine = FomMonitoringCore.DAL.MessageMachine;

namespace FomMonitoringCore.Service.DataMapping
{
    public class JsonVariantsToSqlServerService : IJsonVariantsToSqlServerService
    {
        private readonly IFomMonitoringEntities _context;
        private readonly IMessageService _messageService;

        public JsonVariantsToSqlServerService(IFomMonitoringEntities context, IUnitOfWork unitOfWork,
            IMessageService messageService)
        {
            _messageService = messageService;
            _context = context;
        }

        public bool MappingJsonParametersToSqlServer(JsonDataModel jsonDataModel)
        {
            var result = false;
            try
            {
                var json = JsonConvert.DeserializeObject<JObject>(jsonDataModel.Json);

                var tt = json.Root.First(n => n.Path.ToLower() == "info");

                var ii = JsonConvert.DeserializeObject<List<info>>(JsonConvert.SerializeObject(tt.First))
                    .FirstOrDefault();

                var mac = _context.Set<Machine>().FirstOrDefault(m => m.Serial == ii.MachineSerial);
                if (mac == null)
                    return false;

                var pms = _context.Set<ParameterMachine>().AsNoTracking().Where(p =>
                    p.MachineModelId == mac.MachineModelId).ToList();

                var oldValues = _context.Set<ParameterMachineValue>().Where(p =>
                        p.MachineId == mac.Id).GroupBy(o => o.VarNumber).Select(o => o.OrderByDescending(p => p.UtcDateTime).FirstOrDefault()).ToList();
                
                foreach (var token in json.Root)
                    switch (token.Path.ToLower())
                    {
                        case "variableslist":
                            var variableList =
                                JsonConvert.DeserializeObject<List<JsonVariablesModel>>(
                                    JsonConvert.SerializeObject(token.First));
                            foreach (var var in variableList)
                            {
                                var.UtcDateTime = var.UtcDateTime.Year < 1900 ? DateTime.UtcNow : var.UtcDateTime;
                                if (var.Values != null && var.Values.Count > 0)
                                {
                                    var addedEntities = new List<dynamic>();
                                    var.Values.ForEach(value =>
                                    {
                                        var pm = pms.FirstOrDefault(p => p.VarNumber == value.VariableNumber);
                                        if (pm == null)
                                            return;

                                        //ordino per data e poi per id perchè spesso arrivano valori diversi con la stessa data
                                        var previousValue = oldValues.FirstOrDefault(p => p.VarNumber == value.VariableNumber)?.VarValue;
                                        var exists = oldValues.Any(p => p.VarNumber == value.VariableNumber && p.UtcDateTime.Date == var.UtcDateTime.Date);

                                        if (pm.Historicized == null || pm.Historicized == "1") //storicizzazione attivata
                                        {
                                            if (!exists) //se non esiste un record per la giornata del valore della variabile lo aggiungo
                                            {
                                                var pmv = new
                                                {
                                                    MachineId = mac.Id,
                                                    ParameterMachineId = pm.Id,
                                                    UtcDateTime = var.UtcDateTime,
                                                    VarNumber = value.VariableNumber,
                                                    VarValue = value.VariableValue,
                                                    VariableResetDate = value.VariableResetDate
                                                };
                                                addedEntities.Add(pmv);
                                            }
                                            else //se esiste ne aggiorno il valore
                                            {
                                                var pmv = oldValues.First(p => p.VarNumber == value.VariableNumber && p.UtcDateTime.Date == var.UtcDateTime.Date);
                                                pmv.VarValue = value.VariableValue;

                                            }

                                        }
                                        else //storicizzazione non attivata
                                        {
                                            var pmv = oldValues.FirstOrDefault(p => p.VarNumber == value.VariableNumber); //prendo il valore più recente 

                                            if (pmv != null) //se esiste già un record lo aggiorno, altrimento lo aggiungo
                                            {
                                                pmv.UtcDateTime = var.UtcDateTime;
                                                pmv.VarValue = value.VariableValue;
                                            }
                                            else
                                            {
                                                var nVar = new
                                                {
                                                    MachineId = mac.Id,
                                                    ParameterMachineId = pm.Id,
                                                    UtcDateTime = var.UtcDateTime,
                                                    VarNumber = value.VariableNumber,
                                                    VarValue = value.VariableValue
                                                    
                                                };

                                                addedEntities.Add(nVar);
                                            }
                                        }


                                        if (!string.IsNullOrEmpty(pm.ThresholdMax) && pm.ThresholdMax != "0" ||
                                            !string.IsNullOrEmpty(pm.ThresholdMin) && pm.ThresholdMin != "0")
                                            CheckVariableTresholds(mac, pm, value, previousValue, var.UtcDateTime);
                                    });


                                    if (addedEntities.Any())
                                        _context.Set<ParameterMachineValue>().AddRange(addedEntities.Select(a => new ParameterMachineValue
                                        {
                                            MachineId = a.MachineId,
                                            ParameterMachineId = a.ParameterMachineId,
                                            UtcDateTime = a.UtcDateTime,
                                            VarNumber = a.VarNumber,
                                            VarValue = a.VarValue
                                        }).ToList());

                                    addedEntities.Clear();
                                }
                            }

                            break;
                    }

                result = true;
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), jsonDataModel.Id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }


        public void CheckVariableTresholds(Machine machine,
            ParameterMachine par, JsonVariableValueModel value, decimal? oldValue, DateTime utcDatetime)
        {
            if (machine == null || par == null || value == null)
                return;
            if (oldValue != null && oldValue >= value.VariableValue)
                return;

            //controllo se il valore oltrepassa la soglia e non esiste già un msessaggio lo inserisco                                      
            var min = Convert.ToDecimal(par.ThresholdMin);
            var max = Convert.ToDecimal(par.ThresholdMax);

            if (value.VariableValue < min || value.VariableValue > max)
            {
                var mes = _context.Set<MessageMachine>().FirstOrDefault(mm =>
                    mm.MachineId == machine.Id && mm.MessagesIndex.IsPeriodicM &&
                    mm.MessagesIndex.MessageCode == par.ThresholdLabel);
                if (mes == null)
                    _messageService.InsertMessageMachine(machine, par.ThresholdLabel, utcDatetime);
                else if (oldValue != null)
                    if (mes.Day < utcDatetime)
                    {
                        //verifico se il precedente valore era sotto la soglia inserisco un nuovo messaggio
                        if (oldValue >= min && oldValue <= max)
                        {
                            _messageService.InsertMessageMachine(machine, par.ThresholdLabel, utcDatetime);
                        }
                        //in questo caso ero sopra la soglia e continuo ad essere sopra la soglia
                        // controllo se il valore del parametro ha superato il prossimo multiplo del valore max
                        else
                        {
                            var valOld = oldValue ?? 0;
                            var valNew = value.VariableValue;
                            if (valOld > max && valNew > valOld)
                            {
                                var multiploOld = Math.Floor(valOld / max);
                                var multiploNew = Math.Floor(valNew / max);
                                if (multiploNew > multiploOld)
                                    _messageService.InsertMessageMachine(machine, par.ThresholdLabel, utcDatetime);
                            }
                        }
                    }
            }
        }
    }
}