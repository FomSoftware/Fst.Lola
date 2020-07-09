using System.Collections.Generic;

namespace FomMonitoringCore.SqlServer.Repository
{
    public interface IParameterMachineRepository : IGenericRepository<ParameterMachine>
    {
        IEnumerable<ParameterMachine> GetByParameters(int machineModelId, int? idPanel = null, int? idCluster = null, bool tracked = true);

    }
}
