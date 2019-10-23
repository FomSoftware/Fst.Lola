using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository
{
    public class ParameterMachineRepository : GenericRepository<ParameterMachine>, IParameterMachineRepository
    {
        public ParameterMachineRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }

}
