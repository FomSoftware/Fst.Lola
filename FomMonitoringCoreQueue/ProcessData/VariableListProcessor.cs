using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Dto;
using MessageMachine = FomMonitoringCore.DAL.MessageMachine;

namespace FomMonitoringCoreQueue.ProcessData
{
    public class VariableListProcessor : IProcessor<VariablesList>
    {
        private readonly ILifetimeScope _parentScope;
        private IFomMonitoringEntities _context;
        private IMessageService _messageService;


        public VariableListProcessor(ILifetimeScope parentScope)
        {
            _parentScope = parentScope;
        }
        public bool ProcessData(VariablesList data)
        {
            try
            {
                using (var threadLifetime = _parentScope.BeginLifetimeScope())
                using (var _context = threadLifetime.Resolve<IFomMonitoringEntities>())
                using (var messageService = threadLifetime.Resolve<IMessageService>())
                {
                    var serial = data.InfoMachine.FirstOrDefault()?.MachineSerial;
                    var mac = _context.Set<Machine>().FirstOrDefault(m => m.Serial == serial);
                    if (mac == null)
                        return false;

                    var pms = _context.Set<ParameterMachine>().AsNoTracking().Where(p =>
                        p.MachineModelId == mac.MachineModelId).ToList();

                    var oldValues = _context.Set<ParameterMachineValue>().Where(p =>
                            p.MachineId == mac.Id).GroupBy(o => o.VarNumber)
                        .Select(o => o.OrderByDescending(p => p.UtcDateTime).FirstOrDefault()).ToList();

                    foreach (var var in data.VariablesListMachine)
                    {
                        var.UtcDateTime = var.UtcDateTime.Year < 1900 ? DateTime.UtcNow : var.UtcDateTime;
                        if (var.Values == null || !var.Values.Any())
                            continue;

                        var addedEntities = new List<dynamic>();
                        var.Values.AsParallel().ForAll(value =>
                        {
                            var pm = pms.FirstOrDefault(p => p.VarNumber == value.VariableNumber);
                            if (pm == null)
                                return;

                            //ordino per data e poi per id perchè spesso arrivano valori diversi con la stessa data
                            var previousValue = oldValues.FirstOrDefault(p => p.VarNumber == value.VariableNumber)
                                ?.VarValue;
                            var exists = oldValues.Any(p =>
                                p.VarNumber == value.VariableNumber && p.UtcDateTime == var.UtcDateTime);

                            if ((pm.Historicized == null || pm.Historicized == "1") && !exists)
                            {
                                var pmv = new
                                {
                                    MachineId = mac.Id,
                                    ParameterMachineId = pm.Id,
                                    var.UtcDateTime,
                                    VarNumber = value.VariableNumber,
                                    VarValue = value.VariableValue
                                };
                                addedEntities.Add(pmv);
                            }
                            else if (pm.Historicized == "0" || exists)
                            {
                                var pmv = oldValues.FirstOrDefault(p => p.VarNumber == value.VariableNumber);

                                if (pmv != null)
                                {
                                    if (pm.Historicized == "0" && pmv.UtcDateTime < var.UtcDateTime)
                                    {
                                        pmv.UtcDateTime = var.UtcDateTime;

                                        pmv.VarValue = value.VariableValue;
                                    }
                                    else
                                    {
                                        if (pm.Historicized != "0" && exists)
                                        {
                                            pmv.VarValue = value.VariableValue;
                                        }
                                    }
                                }
                                else
                                {
                                    var nVar = new
                                    {
                                        MachineId = mac.Id,
                                        ParameterMachineId = pm.Id,
                                        var.UtcDateTime,
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
                            _context.Set<ParameterMachineValue>().AddRange(addedEntities.Select(a =>
                                new ParameterMachineValue
                                {
                                    MachineId = a.MachineId,
                                    ParameterMachineId = a.ParameterMachineId,
                                    UtcDateTime = a.UtcDateTime,
                                    VarNumber = a.VarNumber,
                                    VarValue = a.VarValue
                                }).ToList());

                        addedEntities.Clear();
                    }

                    mac.LastUpdate = DateTime.UtcNow;
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void CheckVariableTresholds(Machine machine,
            ParameterMachine par, FomMonitoringCore.DataProcessing.Dto.Value value, decimal? oldValue, DateTime utcDatetime)
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
                var mes = _context.Set<MessageMachine>().AsNoTracking().FirstOrDefault(mm =>
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
                            var valOld = (decimal)oldValue;
                            var valNew = value.VariableValue;
                            if (valOld > max && valNew > valOld && max > 0)
                            {
                                var multiploOld = Math.Floor(valOld / max);
                                var multiploNew = Math.Floor((valNew ?? 0) / max);
                                if (multiploNew > multiploOld)
                                    _messageService.InsertMessageMachine(machine, par.ThresholdLabel, utcDatetime);
                            }
                        }
                    }
            }
        }

        public void Dispose()
        {
            _parentScope?.Dispose();
            _context?.Dispose();
            _messageService?.Dispose();
        }
    }
}