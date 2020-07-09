namespace FomMonitoringCore.SqlServer.Repository
{
    public class MachineModelRepository : GenericRepository<MachineModel>, IMachineModelRepository
    {
        public MachineModelRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
