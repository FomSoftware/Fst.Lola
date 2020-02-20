using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository.SQL
{
    public class MachineTypeRepository : GenericRepository<MachineType>, IMachineTypeRepository
    {
        public MachineTypeRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
