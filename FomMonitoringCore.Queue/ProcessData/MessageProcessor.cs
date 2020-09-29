﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FomMonitoringCore.Queue.Dto;
using FomMonitoringCore.SqlServer;
using Mapster;
using MessageMachine = FomMonitoringCore.SqlServer.MessageMachine;

namespace FomMonitoringCore.Queue.ProcessData
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
                    HistoricizingMessages(_context, mac.Id);

                    _context.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void HistoricizingMessages(IFomMonitoringEntities context, int idMachine)
        {
            var maxHpDate = context.Set<HistoryMessage>().Where(hp => hp.MachineId == idMachine)
                .OrderByDescending(a => a.Day).FirstOrDefault()?.Day;

            maxHpDate = maxHpDate?.Date ?? DateTime.MinValue;

            var historyMessages = context.Set<MessageMachine>()
                .Where(p => p.Day >= maxHpDate && p.MachineId == idMachine &&
                            (p.MessagesIndex.MessageTypeId == 11 ||
                             p.MessagesIndex.MessageTypeId == 12)).ToList()
                .GroupBy(g => new{ g.Day.Value.Date, g.Params, g.MessagesIndexId})
                .Select(n => new HistoryMessage
                {
                    Day = n.Key.Date,
                    Params = n.Key.Params,
                    MessagesIndexId = n.Key.MessagesIndexId,
                    MachineId = idMachine,
                    Period = n.Key.Date.Year * 10000 + n.Key.Date.Month * 100 + n.Key.Date.Day,
                    Count = n.Count(),
                    TypeHistory = "d"
                }).ToList();

            foreach (var cur in historyMessages)
            {
                var row = context.Set<HistoryMessage>().FirstOrDefault(hp => hp.MachineId == idMachine &&
                                                                           hp.Day == cur.Day &&
                                                                           hp.TypeHistory == cur.TypeHistory &&
                                                                           hp.MessagesIndexId == cur.MessagesIndexId &&
                                                                           hp.Params == cur.Params);
                if (row != null)
                {
                    row.Count = cur.Count;
                }
                else
                {
                    context.Set<HistoryMessage>().Add(cur);
                }
            }

        }

            public void Dispose()
        {
            _parentScope?.Dispose();
        }
    }
}