namespace FomMonitoringCore.SqlServer.Repository
{
    public class MachineRepository : GenericRepository<Machine>, IMachineRepository
    {
        public MachineRepository(IFomMonitoringEntities context) : base(context)
        {

        }
        
    }
}
