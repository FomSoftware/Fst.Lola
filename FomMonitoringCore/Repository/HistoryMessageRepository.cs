﻿using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository
{
    public class HistoryMessageRepository : GenericRepository<HistoryMessage>, IHistoryMessageRepository
    {
        public HistoryMessageRepository(IFomMonitoringEntities context) : base(context)
        {

        }

        public IEnumerable<HistoryMessage> GetHistoryMessage(int idMachine, DateTime start, DateTime end, int? machineGroup = null)
        {
            var query = context.Set<HistoryMessage>()
                .AsNoTracking()
                .Include("MessagesIndex")
                .AsNoTracking()
                .Where(m => m.MachineId == idMachine && m.Day >= start && m.Day <= end && m.MessagesIndex.IsVisibleLOLA == true
                            && m.MessagesIndex.IsPeriodicM == false && m.MessagesIndex.IsDisabled == false  && m.MessagesIndex.MessageCode != null);


            if (machineGroup.HasValue)
            {
                query = query.Where(m => m.MessagesIndex != null && m.MessagesIndex.MachineGroupId == machineGroup);
            }

            return query.ToList();
        }
    }
}