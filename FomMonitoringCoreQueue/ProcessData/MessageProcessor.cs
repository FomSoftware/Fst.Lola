using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Dto;
using Mapster;

namespace FomMonitoringCoreQueue.ProcessData
{
    public class MessageProcessor : IProcessor<Message>
    {
        private readonly ILifetimeScope _parentScope;


        public MessageProcessor(ILifetimeScope parentScope)
        {
            _parentScope = parentScope;
        }

        public bool ProcessData(Message data)
        {

            try
            {
                using (var threadLifetime = _parentScope.BeginLifetimeScope())
                using (var _context = threadLifetime.Resolve<IFomMonitoringEntities>())
                {

                    var serial = data.InfoMachine.FirstOrDefault()?.MachineSerial;
                    var mac = _context.Set<Machine>().AsNoTracking().FirstOrDefault(m => m.Serial == serial);

                    if (mac == null)
                        return false;

                    var cat = _context.Set<MachineModel>().Find(mac.MachineModelId)?.MessageCategoryId;

                    //var maxDate = _context.Set<MessageMachine>()
                    //    .Any(m => m.MachineId == mac.Id && m.Day != null && m.IsPeriodicMsg != true) ? _context.Set<MessageMachine>().Where(m => m.MachineId == mac.Id && m.Day != null && m.IsPeriodicMsg != true).Max(et => et.Day ?? DateTime.MinValue) : DateTime.MinValue;

                    //var messages = data.MessageMachine.Where(w => w.Time > maxDate).ToList();

                    //devo eliminare quei messaggi che hanno isLolaVisible = 0 da mdb
                    var messageMachine = new List<MessageMachine>();

                    foreach (var mm in data.MessageMachine)
                    {
                        if (_context.Set<MessageMachine>().Any(msg =>
                            msg.MessagesIndex.MessageCode == mm.Code && msg.Operator == mm.Operator &&
                            msg.Day == mm.Time)) continue;

                        var message = mm.BuildAdapter().AddParameters("machineId", mac.Id)
                            .AdaptToType<MessageMachine>();
                        var msgIndex = _context.Set<MessagesIndex>()
                            .FirstOrDefault(f => f.MessageCode == mm.Code && f.MessageCategoryId == cat);
                        message.Id = 0;
                        if (msgIndex == null)
                            continue;
                        var old = _context.Set<MessageMachine>().Count(a => a.MessagesIndexId == msgIndex.Id &&
                                                                            a.MachineId == message.MachineId &&
                                                                            a.StartTime.Value.CompareTo(
                                                                                message.StartTime.Value) == 0);
                        if (old != 0)
                            continue;
                        message.MessagesIndexId = msgIndex.Id;
                        message.MessagesIndex = msgIndex;
                        messageMachine.Add(message);

                    }

                    //IS-754 escludere tutti quelli che hanno isLolaVisible = false && type error o warning 
                    messageMachine = messageMachine.Where(f => f.MessagesIndex != null
                                                               && f.MessagesIndex.IsVisibleLOLA
                                                               && f.MessagesIndex.MessageType != null
                                                               && (f.MessagesIndex.MessageType.Id == 11 ||
                                                                   f.MessagesIndex.MessageType.Id == 12)).ToList();
                    if (messageMachine.Any())
                    {
                        _context.Set<MessageMachine>().AddRange(messageMachine);

                    }
                    

                    _context.SaveChanges();
                    _context.usp_HistoricizingMessages(mac.Id);


                    _context.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            _parentScope?.Dispose();
        }
    }
}