using FomMonitoringCore.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.Repository
{
    public interface IMessageMachineRepository : IGenericRepository<MessageMachine>
    {
        IEnumerable<MessageMachine> GetMachineMessages(int idMachine, DateTime start, DateTime end, bool includePeriodicMsg = false, bool onlyVisible = false);
    }
}
