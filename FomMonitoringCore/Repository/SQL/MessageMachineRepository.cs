using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.DAL;
using MessageMachine = FomMonitoringCore.DAL.MessageMachine;

namespace FomMonitoringCore.Repository.SQL
{
    public class MessageMachineRepository : GenericRepository<MessageMachine>, IMessageMachineRepository
    {
        public MessageMachineRepository(IFomMonitoringEntities context) : base(context)
        {

        }

        public IEnumerable<MessageMachine> GetMachineMessages(int idMachine, DateTime start, DateTime end, int? machineGroup = null, bool includePeriodicMsg = false, bool onlyVisible = false)
        {
            var query = 
                    Queryable.Where(context.Set<MessageMachine>()
                            .AsNoTracking()
                            .Include("MessagesIndex")
                            .AsNoTracking(), m => m.MachineId == idMachine && m.Day >= start && m.Day <= end);
            
            
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
