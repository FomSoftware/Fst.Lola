﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.SqlServer.Repository
{
    public class MessageMachineRepository : GenericRepository<MessageMachine>, IMessageMachineRepository
    {
        public MessageMachineRepository(IFomMonitoringEntities context) : base(context)
        {

        }

        public IEnumerable<MessageMachine> GetMachineMessages(int idMachine, DateTime start, DateTime end, int? machineGroup = null, 
            bool includePeriodicMsg = false, bool onlyVisible = false, List<int> messTypes = null, bool timeLimit = true)
        {
            //devo eliminare i messaggi di errore più vecchi di 15 giorni
            DateTime now = DateTime.UtcNow;
            DateTime startError = start;

            if (timeLimit && (now - start).TotalDays > 15)
                startError = now.AddDays(-15);

            var query = 
                    Queryable.Where(context.Set<MessageMachine>()
                            .AsNoTracking()
                            .Include("MessagesIndex")
                            .AsNoTracking(), m => m.MachineId == idMachine 
                                                  && m.Day >= startError
                                                  && m.Day <= end);

            if (messTypes != null)
                query = query.Where(m => messTypes.Contains(m.MessagesIndex.MessageTypeId));
            
            if(includePeriodicMsg == false)
            {
                query = Queryable.Where(query, m => m.MessagesIndex != null && m.MessagesIndex.IsPeriodicM == false);
            }

            if (onlyVisible)
            {
                query = Queryable.Where(query, m => m.MessagesIndex != null && m.MessagesIndex.IsVisibleLOLA);
            }

            if (machineGroup.HasValue)
            {
                query = Queryable.Where(query, m => m.MessagesIndex != null && m.MessagesIndex.MachineGroupId == machineGroup);
            }

            return query.ToList();
        }
    }
}
