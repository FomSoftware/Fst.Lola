using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository
{
    public class ParameterMachineValueRepository : GenericRepository<ParameterMachineValue>, IParameterMachineValueRepository
    {
        public ParameterMachineValueRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }

}
