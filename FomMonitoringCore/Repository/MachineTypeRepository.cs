using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository
{
    public class MachineTypeRepository : GenericRepository<MachineType>, IMachineTypeRepository
    {
        public MachineTypeRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
