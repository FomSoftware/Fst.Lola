namespace FomMonitoringCore.SqlServer.Repository
{
    public class MachineGroupRepository : GenericRepository<MachineGroup>, IMachineGroupRepository
    {
        public MachineGroupRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }

}
