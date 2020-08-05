using System;
using System.Data.Entity;
using System.Linq;
using Autofac;
using FomMonitoringCore.Queue.Dto;
using FomMonitoringCore.Service;
using FomMonitoringCore.SqlServer;
using MessageMachine = FomMonitoringCore.SqlServer.MessageMachine;

namespace FomMonitoringCore.Queue.ProcessData
{
    public class VariableListProcessor : IProcessor<VariablesList>
    {
        private readonly ILifetimeScope _parentScope;
        private IFomMonitoringEntities _context;
        private IMessageService _messageService;


        public VariableListProcessor(ILifetimeScope parentScope)
        {
            _parentScope = parentScope;
            _context = parentScope.Resolve<IFomMonitoringEntities>();
            _messageService = parentScope.Resolve<IMessageService>();
        }
        public bool ProcessData(VariablesList data)
        {
                var serial = data.InfoMachine.FirstOrDefault()?.MachineSerial;
                    var mac = _context.Set<Machine>().AsNoTracking().FirstOrDefault(m => m.Serial == serial);
                    if (mac == null)
                        return false;

                    var pms = _context.Set<ParameterMachine>().AsNoTracking().Where(p =>
                        p.MachineModelId == mac.MachineModelId).ToList();

                    foreach (var var in data.VariablesListMachine)
                    {
                        var.UtcDateTime = var.UtcDateTime.Year < 1900 ? DateTime.UtcNow : var.UtcDateTime;
                        if (var.Values == null || !var.Values.Any())
                            continue;

                        var.Values.ToList().ForEach(value =>
                        {
                            var pm = pms.SingleOrDefault(p => p.VarNumber == value.VariableNumber && p.MachineModelId == mac.MachineModelId);
                            if (pm == null)
                                return;

                            DateTime? lastReset = value.VariableResetDate.HasValue && value.VariableResetDate.Value.Year < 1900 ? null : value.VariableResetDate;

                            var previousValue = _context.Set<ParameterMachineValue>().Where(p =>
                                    p.MachineId == mac.Id && p.VarNumber == value.VariableNumber).OrderByDescending(p => p.UtcDateTime).FirstOrDefault(p => p.VarNumber == value.VariableNumber)
                                ?.VarValue;
                            //var exists = oldValues.Any(p =>
                            //    p.VarNumber == value.VariableNumber && p.UtcDateTime == var.UtcDateTime);

                            if (pm.Historicized == null || pm.Historicized == "1")
                            {
                                var day = var.UtcDateTime.Date;
                                var values = _context.Set<ParameterMachineValue>().Where(p =>
                                    p.MachineId == mac.Id && p.ParameterMachineId == pm.Id);

                                var dayValues = values.Where(p =>
                                    DbFunctions.TruncateTime(p.UtcDateTime) == day).ToList();

                                ParameterMachineValue dayValue = null;
                                if (dayValues.Any())
                                {
                                    dayValue = _context.Set<ParameterMachineValue>().Find(dayValues.Max(n => n.Id));
                                }
                                
                                if (dayValue != null)
                                {
                                    if(dayValues.Any(dv => dv.Id != dayValues.Max(n => n.Id)))
                                        _context.Set<ParameterMachineValue>().RemoveRange(dayValues.Where(dv => dv.Id != dayValues.Max(n => n.Id)));

                                    //dayValue.UtcDateTime = var.UtcDateTime;
                                    if (lastReset != null)
                                    {
                                        AddResetValue(pm, (DateTime)lastReset, value.VariableValue, dayValue.VarValue, mac.Id);
                                    }
                                    dayValue.UtcDateTime = var.UtcDateTime;
                                    dayValue.VarValue = value.VariableValue;
                                }
                                else
                                {
                                    _context.Set<ParameterMachineValue>().Add(
                                        new ParameterMachineValue
                                        {
                                            MachineId = mac.Id,
                                            ParameterMachineId = pm.Id,
                                            UtcDateTime = lastReset ?? var.UtcDateTime,
                                            VarNumber = value.VariableNumber,
                                            VarValue = value.VariableValue
                                        });
                                    _context.SaveChanges();

                                    if (lastReset != null)
                                    {
                                        AddResetValue(pm, (DateTime)lastReset, value.VariableValue, previousValue, mac.Id);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                var values = _context.Set<ParameterMachineValue>().Where(p =>
                                    p.MachineId == mac.Id && p.ParameterMachineId == pm.Id).ToList();

                                ParameterMachineValue pmv = null;
                                if (values.Any())
                                {
                                    pmv = _context.Set<ParameterMachineValue>().Find(values.Max(n => n.Id));
                                    var valuesToRemove = values.Where(dv => dv.Id != values.Max(n => n.Id)).ToList();
                                    if (valuesToRemove.Any())
                                        _context.Set<ParameterMachineValue>().RemoveRange(valuesToRemove);
                                }
                                
                                if (pmv != null)
                                {
                                        //pmv.UtcDateTime = var.UtcDateTime;
                                    if (lastReset != null)
                                    {
                                        pmv.UtcDateTime = (DateTime)lastReset;
                                        AddResetValue(pm, (DateTime)lastReset, value.VariableValue, pmv.VarValue, mac.Id);
                                    }
                                    pmv.VarValue = value.VariableValue;
                                }
                                else
                                {
                                    _context.Set<ParameterMachineValue>().Add(
                                        new ParameterMachineValue
                                        {
                                            MachineId = mac.Id,
                                            ParameterMachineId = pm.Id,
                                            UtcDateTime = lastReset ?? var.UtcDateTime,
                                            VarNumber = value.VariableNumber,
                                            VarValue = value.VariableValue
                                        });
                                    _context.SaveChanges();

                                    if (lastReset != null)
                                    {
                                        AddResetValue(pm, (DateTime) lastReset, value.VariableValue, null, mac.Id);
                                        _context.SaveChanges();
                                    }
                                    

                                }
                            }

                            foreach (var threshold in pm.ParameterMachineThreshold)
                            {
                                if (!string.IsNullOrEmpty(pm.ThresholdMax) && pm.ThresholdMax != "0" ||
                                    !string.IsNullOrEmpty(pm.ThresholdMin) && pm.ThresholdMin != "0")
                                    CheckVariableTresholds(mac, threshold, value, previousValue, var.UtcDateTime);
                            }

                            _context.SaveChanges();
                        }) ;

                    }
                    _context.SaveChanges();


                    return true;
                
            

        }

        private void AddResetValue(ParameterMachine parameterMachine, DateTime lastReset, decimal? variableValue, decimal? valueBeforeReset, int idMachine)
        {
            if(!parameterMachine.ParameterResetValue.Any(pm => pm.ResetDate == lastReset && pm.MachineId == idMachine))
                _context.Set<ParameterResetValue>().Add(
                    new ParameterResetValue()
                    {
                        ParameterMachineId = parameterMachine.Id,
                        ResetDate = (DateTime)lastReset,
                        ResetValue = variableValue,
                        ValueBeforeReset = valueBeforeReset,
                        MachineId = idMachine
                    });
        }

        public void CheckVariableTresholds(Machine machine,
            ParameterMachineThreshold par, FomMonitoringCore.DataProcessing.Dto.Value value, decimal? oldValue, DateTime utcDatetime)
        {
            if (machine == null || par == null || value == null)
                return;
            if (oldValue != null && oldValue >= value.VariableValue)
                return;

            //controllo se il valore oltrepassa la soglia e non esiste già un msessaggio lo inserisco                                      
            var min = par.ThresholdMin;
            var max = par.ThresholdMax;

            if (value.VariableValue < min || value.VariableValue > max)
            {
                var mes = _context.Set<MessageMachine>().AsNoTracking().FirstOrDefault(mm =>
                    mm.MachineId == machine.Id && mm.MessagesIndex != null &&
                    mm.MessagesIndex.Id == par.MessagesIndex.Id);
                if (mes == null)
                    _messageService.InsertMessageMachine(machine, par.MessagesIndex.MessageCode, utcDatetime);
                else if (oldValue != null)
                    if (mes.Day < utcDatetime)
                    {
                        //verifico se il precedente valore era sotto la soglia inserisco un nuovo messaggio
                        if (oldValue >= min && oldValue <= max)
                        {
                            _messageService.InsertMessageMachine(machine, par.MessagesIndex.MessageCode, utcDatetime);
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
                                    _messageService.InsertMessageMachine(machine, par.MessagesIndex.MessageCode, utcDatetime);
                            }
                        }
                    }
            }
        }

        public void Dispose()
        {
            _parentScope?.Dispose();
        }
    }
}