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

namespace FomMonitoringCore.Service.DataMapping
{
    public class JsonVariantsToSqlServerService : IJsonVariantsToSQLServerService
    {
        private readonly IFomMonitoringEntities _context;
        private readonly IMessageService _messageService;
        private readonly IUnitOfWork _unitOfWork;

        public JsonVariantsToSqlServerService(IFomMonitoringEntities context, IUnitOfWork unitOfWork,
            IMessageService messageService)
        {
            _messageService = messageService;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public bool MappingJsonVariantsToSQLite(JsonDataModel jsonDataModel)
        {
            var result = false;
            try
            {
                var json = JsonConvert.DeserializeObject<JObject>(jsonDataModel.Json);

                _unitOfWork.StartTransaction(_context);

                var tt = json.Root.First(n => n.Path.ToLower() == "info");

                var ii = JsonConvert.DeserializeObject<List<info>>(JsonConvert.SerializeObject(tt.First))
                    .FirstOrDefault();

                var mac = _context.Set<Machine>().FirstOrDefault(m => m.Serial == ii.MachineSerial);

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
                                    foreach (var value in var.Values)
                                    {
                                        var pm = _context.Set<ParameterMachine>().FirstOrDefault(p =>
                                            p.MachineModelId == mac.MachineModelId
                                            && p.VarNumber == value.VariableNumber);

                                        //ordino per data e poi per id perchè spesso arrivano valori diversi con la stessa data
                                        var previousValue = _context.Set<ParameterMachineValue>().Where(p =>
                                                p.MachineId == mac.Id && p.VarNumber == pm.VarNumber)
                                            .OrderByDescending(p => p.UtcDateTime).ThenByDescending(t => t.Id)
                                            .FirstOrDefault()?.VarValue;


                                        if (mac != null && pm != null &&
                                            (pm.Historicized == null || pm.Historicized == "1"))
                                        {
                                            var pmv = new ParameterMachineValue
                                            {
                                                MachineId = mac.Id,
                                                ParameterMachineId = pm.Id,
                                                UtcDateTime = var.UtcDateTime,
                                                VarNumber = value.VariableNumber,
                                                VarValue = value.VariableValue
                                            };
                                            _context.Set<ParameterMachineValue>().Add(pmv);
                                        }
                                        else if (mac != null && pm != null && pm.Historicized == "0")
                                        {
                                            var pmv = _context.Set<ParameterMachineValue>().FirstOrDefault(pp =>
                                                pp.MachineId == mac.Id
                                                && pp.VarNumber == value.VariableNumber);
                                            if (pmv != null)
                                            {
                                                pmv.UtcDateTime = var.UtcDateTime;
                                                pmv.VarValue = value.VariableValue;
                                            }
                                            else
                                            {
                                                pmv = new ParameterMachineValue
                                                {
                                                    MachineId = mac.Id,
                                                    ParameterMachineId = pm.Id,
                                                    UtcDateTime = var.UtcDateTime,
                                                    VarNumber = value.VariableNumber,
                                                    VarValue = value.VariableValue
                                                };
                                                _context.Set<ParameterMachineValue>().Add(pmv);
                                            }
                                        }

                                        _context.SaveChanges();

                                        if (!string.IsNullOrEmpty(pm.ThresholdMax) && pm.ThresholdMax != "0" ||
                                            !string.IsNullOrEmpty(pm.ThresholdMin) && pm.ThresholdMin != "0")
                                            checkVariableTresholds(mac, pm, value, previousValue, var.UtcDateTime);
                                    }
                            }

                            break;
                    }                          

                _unitOfWork.CommitTransaction();
                result = true;
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), jsonDataModel.Id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                _unitOfWork.RollbackTransaction();
            }

            return result;
        }


        public void checkVariableTresholds(Machine machine,
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
                    mm.MachineId == machine.Id && mm.IsPeriodicMsg == true &&
                    mm.Code == par.ThresholdLabel);
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