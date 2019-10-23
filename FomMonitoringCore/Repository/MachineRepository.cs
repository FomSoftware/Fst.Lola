using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository
{
    public class MachineRepository : GenericRepository<Machine>, IMachineRepository
    {
        public MachineRepository(IFomMonitoringEntities context) : base(context)
        {

        }
        
    }
}
