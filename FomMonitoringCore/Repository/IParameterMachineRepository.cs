using System.Collections.Generic;
using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository
{
    public interface IParameterMachineRepository : IGenericRepository<ParameterMachine>
    {
        IEnumerable<ParameterMachine> GetByParameters(int machineModelId, int? idPanel = null, int? idCluster = null, bool tracked = true);

    }
}
