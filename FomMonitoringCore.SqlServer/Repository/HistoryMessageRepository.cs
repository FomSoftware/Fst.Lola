using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.SqlServer.Repository
{
    public class HistoryMessageRepository : GenericRepository<HistoryMessage>, IHistoryMessageRepository
    {
        public HistoryMessageRepository(IFomMonitoringEntities context) : base(context)
        {

        }

        public IEnumerable<HistoryMessage> GetHistoryMessage(int idMachine, DateTime start, DateTime end, int? machineGroup = null)
        {
            //devo eliminare i messaggi di errore più vecchi di 15 giorni
            DateTime now = DateTime.UtcNow;
            DateTime startError = start;
            if ((now - start).TotalDays > 15)
            {
                startError = now.AddDays(-15);
            }
               

            var query = context.Set<HistoryMessage>()
                .AsNoTracking()
                .Include("MessagesIndex")
                .AsNoTracking().Where(m => m.MachineId == idMachine && m.Day <= end && m.MessagesIndex.IsVisibleLOLA
                                                                    && m.MessagesIndex.IsPeriodicM == false 
                                                                    && m.MessagesIndex.IsDisabled == false  
                                                                    && m.MessagesIndex.MessageCode != null
                                                                    && ((m.Day >= startError && m.MessagesIndex.MessageTypeId == 11) || (m.Day >= start && m.MessagesIndex.MessageTypeId != 11)));


            if (machineGroup.HasValue)
            {
                query = query.Where(m => m.MessagesIndex != null && m.MessagesIndex.MachineGroupId == machineGroup);
            }

            return query.ToList();
        }
    }
}