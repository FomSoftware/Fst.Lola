using FomMonitoringCore.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.Repository
{
    public class MessageMachineRepository : GenericRepository<MessageMachine>, IMessageMachineRepository
    {
        public MessageMachineRepository(IFomMonitoringEntities context) : base(context)
        {

        }

        public IEnumerable<MessageMachine> GetMachineMessages(int idMachine, DateTime start, DateTime end, int? machineGroup = null, bool includePeriodicMsg = false, bool onlyVisible = false)
        {
            var query = 
                    context.Set<MessageMachine>()
                        .AsNoTracking()
                        .Include("MessagesIndex")
                        .AsNoTracking()
                        .Where(m => m.MachineId == idMachine && m.Day >= start && m.Day <= end);
            
            
            if(includePeriodicMsg == false)
            {
                query = query.Where(m => m.MessagesIndex != null && m.MessagesIndex.IsPeriodicM == false);
            }

            if (onlyVisible)
            {
                query = query.Where(m => m.MessagesIndex != null && m.MessagesIndex.IsVisibleLOLA);
            }

            if (machineGroup.HasValue)
            {
                query = query.Where(m => m.MessagesIndex != null && m.MessagesIndex.MachineGroupId == machineGroup);
            }

            return query.ToList();
        }
    }
}
