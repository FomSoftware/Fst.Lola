using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository.SQL
{
    public class MachineGroupRepository : GenericRepository<MachineGroup>, IMachineGroupRepository
    {
        public MachineGroupRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }

}
