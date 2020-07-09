using System;
using System.Collections.Generic;

namespace FomMonitoringCore.SqlServer.Repository
{
    public interface IMessageMachineRepository : IGenericRepository<MessageMachine>
    {
        IEnumerable<MessageMachine> GetMachineMessages(int idMachine, DateTime start, DateTime end, int? machineGroup = null, bool includePeriodicMsg = false, bool onlyVisible = false);
    }

    public interface IHistoryMessageRepository : IGenericRepository<HistoryMessage>
    {
        IEnumerable<HistoryMessage> GetHistoryMessage(int idMachine, DateTime start, DateTime end, int? machineGroup = null);
    }
}
