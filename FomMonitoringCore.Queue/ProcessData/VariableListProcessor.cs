using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Autofac;
using FomMonitoringCore.Queue.Dto;
using FomMonitoringCore.Service;
using FomMonitoringCore.SqlServer;
using FomMonitoringCore.Uow;
using MessageMachine = FomMonitoringCore.SqlServer.MessageMachine;

namespace FomMonitoringCore.Queue.ProcessData
{
    public class VariableListProcessor : IProcessor<VariablesList>
    {
        private readonly ILifetimeScope _parentScope;


        public VariableListProcessor(ILifetimeScope parentScope)
        {
            _parentScope = parentScope.BeginLifetimeScope();
        }

        public bool ProcessData(VariablesList data)
        {

            using (var scope = _parentScope.BeginLifetimeScope())
            using (var context = scope.Resolve<IFomMonitoringEntities>())
            using (var unitOfWork = new UnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction(context);
                    var messageService = scope.Resolve<IMessageService>();
                    var serial = data.InfoMachine.FirstOrDefault()?.MachineSerial;
                    var mac = context.Set<Machine>().AsNoTracking().FirstOrDefault(m => m.Serial == serial);
                    if (mac == null)
                        return false;

                    var pms = context.Set<ParameterMachine>().Include("ParameterMachineThreshold").Where(p =>
                        p.MachineModelId == mac.MachineModelId).ToList();

                    var maxDate = context.Set<ParameterMachineValue>().Where(p =>
                                      p.MachineId == mac.Id).OrderByDescending(n => n.UtcDateTime).FirstOrDefault()
                                  ?.UtcDateTime ??
                                  DateTime.MinValue;

                    var variableValues = context.Set<ParameterMachineValue>().Where(p =>
                        p.MachineId == mac.Id &&
                        DbFunctions.TruncateTime(maxDate) == DbFunctions.TruncateTime(p.UtcDateTime)).ToList();


                    foreach (var var in data.VariablesListMachine)
                    {
                        var.UtcDateTime = var.UtcDateTime.Year < 1900 ? DateTime.UtcNow : var.UtcDateTime;
                        if (var.Values == null || !var.Values.Any())
                            continue;

                        var.Values.ToList().ForEach(value =>
                        {


                            var pm = pms.SingleOrDefault(p =>
                                p.VarNumber == value.VariableNumber && p.MachineModelId == mac.MachineModelId);

                            if (pm == null)
                                return;

                            var lastReset =
                                value.VariableResetDate.HasValue && value.VariableResetDate.Value.Year < 1900
                                    ? null
                                    : value.VariableResetDate;

                            var previousValue = variableValues.FirstOrDefault(p => p.VarNumber == value.VariableNumber)
                                ?.VarValue;

                            if (pm.Historicized == null || pm.Historicized == "1")
                            {
                                var day = var.UtcDateTime.Date;
                                var values = variableValues.Where(p => p.ParameterMachineId == pm.Id).ToList();

                                var dayValues = values.Where(p =>
                                    p.UtcDateTime.Date == day.Date).ToList();

                                ParameterMachineValue dayValue = null;
                                if (dayValues.Any())
                                {
                                    dayValue = values.OrderByDescending(n => n.Id).FirstOrDefault();
                                }

                                if (dayValue != null)
                                {
                                    if (dayValues.Any(dv => dv.Id != dayValues.Max(n => n.Id)))
                                        context.Set<ParameterMachineValue>()
                                            .RemoveRange(
                                                dayValues.Where(dv => dv.Id != dayValues.Max(n => n.Id)));

                                    //dayValue.UtcDateTime = var.UtcDateTime;
                                    if (lastReset != null)
                                    {
                                        AddResetValue(context, pm, (DateTime) lastReset, value.VariableValue,
                                            dayValue.VarValue, mac.Id);
                                    }

                                    dayValue.UtcDateTime = var.UtcDateTime;
                                    dayValue.VarValue = value.VariableValue;
                                    dayValue.ParameterMachineId = pm.Id;
                                    dayValue.ParameterMachine = pm;
                                }
                                else
                                {
                                    context.Set<ParameterMachineValue>().Add(
                                        new ParameterMachineValue
                                        {
                                            MachineId = mac.Id,
                                            ParameterMachineId = pm.Id,
                                            UtcDateTime = lastReset ?? var.UtcDateTime,
                                            VarNumber = value.VariableNumber,
                                            VarValue = value.VariableValue
                                        });
                                    context.SaveChanges();

                                    if (lastReset != null)
                                    {
                                        AddResetValue(context, pm, (DateTime) lastReset, value.VariableValue,
                                            previousValue, mac.Id);
                                        context.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                var values = variableValues.Where(p => p.ParameterMachineId == pm.Id).ToList();

                                ParameterMachineValue pmv = null;
                                if (values.Any())
                                {
                                    pmv = values.OrderByDescending(n => n.Id).FirstOrDefault();
                                    var valuesToRemove =
                                        values.Where(dv => pmv != null && dv.Id != pmv.Id).ToList();
                                    if (valuesToRemove.Any())
                                        context.Set<ParameterMachineValue>().RemoveRange(valuesToRemove);
                                }

                                if (pmv != null)
                                {
                                    //pmv.UtcDateTime = var.UtcDateTime;
                                    if (lastReset != null)
                                    {
                                        pmv.UtcDateTime = (DateTime) lastReset;
                                        AddResetValue(context, pm, (DateTime) lastReset, value.VariableValue,
                                            pmv.VarValue, mac.Id);
                                    }
                                    else
                                    {
                                        pmv.UtcDateTime = var.UtcDateTime;
                                    }

                                    pmv.VarValue = value.VariableValue;
                                    pmv.ParameterMachineId = pm.Id;
                                    pmv.ParameterMachine = pm;
                                }
                                else
                                {
                                    context.Set<ParameterMachineValue>().Add(
                                        new ParameterMachineValue
                                        {
                                            MachineId = mac.Id,
                                            ParameterMachineId = pm.Id,
                                            UtcDateTime = lastReset ?? var.UtcDateTime,
                                            VarNumber = value.VariableNumber,
                                            VarValue = value.VariableValue
                                        });
                                    context.SaveChanges();

                                    if (lastReset != null)
                                    {
                                        AddResetValue(context, pm, (DateTime) lastReset, value.VariableValue,
                                            null,
                                            mac.Id);
                                        context.SaveChanges();
                                    }


                                }
                            }

                            foreach (var threshold in pm.ParameterMachineThreshold)
                            {
                                if (threshold.ThresholdMax != 0 || threshold.ThresholdMin != 0)
                                    CheckVariableTresholds(context, messageService, mac, threshold, value,
                                        previousValue, var.UtcDateTime);
                            }

                            context.SaveChanges();



                        });
                    }

                    context.SaveChanges();
                    unitOfWork.CommitTransaction();

                    return true;
                }

                catch (Exception ex)
                {
                    unitOfWork.RollbackTransaction();
                    throw ex;

                }
            }
        }
    


        private void AddResetValue(IFomMonitoringEntities context,  ParameterMachine parameterMachine, DateTime lastReset, decimal? variableValue, decimal? valueBeforeReset, int idMachine)
        {
            if(!parameterMachine.ParameterResetValue.Any(pm => pm.ResetDate == lastReset && pm.MachineId == idMachine))
                context.Set<ParameterResetValue>().Add(
                    new ParameterResetValue()
                    {
                        ParameterMachineId = parameterMachine.Id,
                        ResetDate = (DateTime)lastReset,
                        ResetValue = variableValue,
                        ValueBeforeReset = valueBeforeReset,
                        MachineId = idMachine
                    });
        }

        public void CheckVariableTresholds(IFomMonitoringEntities context, IMessageService messageService, Machine machine,
            ParameterMachineThreshold par, DataProcessing.Dto.Value value, decimal? oldValue, DateTime utcDatetime)
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
                var mes = context.Set<MessageMachine>().AsNoTracking().FirstOrDefault(mm =>
                    mm.MachineId == machine.Id && mm.MessagesIndex != null &&
                    mm.MessagesIndex.Id == par.MessagesIndex.Id);
                if (mes == null)
                    messageService.InsertMessageMachine(machine, par.MessagesIndex, utcDatetime);
                else if (oldValue != null)
                    if (mes.Day < utcDatetime)
                    {
                        //verifico se il precedente valore era sotto la soglia inserisco un nuovo messaggio
                        if (oldValue >= min && oldValue <= max)
                        {
                            messageService.InsertMessageMachine(machine, par.MessagesIndex, utcDatetime);
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
                                    messageService.InsertMessageMachine(machine, par.MessagesIndex, utcDatetime);
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