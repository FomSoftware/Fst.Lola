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
                    var mac = _context.Set<Machine>().FirstOrDefault(m => m.Serial == serial);

                    if (mac == null)
                        return false;

                    var cat = _context.Set<MachineModel>().Find(mac.MachineModelId)?.MessageCategoryId;

                    var maxDate = _context.Set<MessageMachine>()
                        .Where(m => m.MachineId == mac.Id && m.Day != null).Max(et => et.Day);

                    var messages = data.MessageMachine.Where(w => w.Time > (maxDate ?? new DateTime())).ToList();

                    //devo eliminare quei messaggi che hanno isLolaVisible = 0 da mdb
                    var messageMachine = new List<MessageMachine>();

                    foreach (var mm in messages)
                    {
                        var message = mm.BuildAdapter().AddParameters("machineId", mac.Id)
                            .AdaptToType<MessageMachine>();
                        var msg = _context.Set<MessagesIndex>()
                            .FirstOrDefault(f => f.MessageCode == mm.Code && f.MessageCategoryId == cat);
                        message.Id = 0;
                        if (msg == null)
                            continue;
                        var old = _context.Set<MessageMachine>().Count(a => a.MessagesIndexId == msg.Id &&
                                                                            a.MachineId == message.MachineId &&
                                                                            a.StartTime.Value.CompareTo(
                                                                                message.StartTime.Value) == 0);
                        if (old != 0)
                            continue;
                        message.MessagesIndexId = msg.Id;
                        message.MessagesIndex = msg;
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

                    mac.LastUpdate = DateTime.UtcNow;

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