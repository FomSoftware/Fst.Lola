using System.Collections.Generic;
using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository
{
    public interface IParameterMachineValueRepository : IGenericRepository<ParameterMachineValue>
    {
        IEnumerable<ParameterMachineValue> GetByParameters(int idMachine, int? idPanel = null, int? idCluster = null);
    }
}
