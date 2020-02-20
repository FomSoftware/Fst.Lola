using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository.SQL
{
    public class MachineModelRepository : GenericRepository<MachineModel>, IMachineModelRepository
    {
        public MachineModelRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
