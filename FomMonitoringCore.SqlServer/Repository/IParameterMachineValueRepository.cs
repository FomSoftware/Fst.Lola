using System.Collections.Generic;

namespace FomMonitoringCore.SqlServer.Repository
{
    public interface IParameterMachineValueRepository : IGenericRepository<ParameterMachineValue>
    {
        IEnumerable<ParameterMachineValue> GetByParameters(int idMachine, int? idPanel = null, int? idCluster = null);
    }
}
