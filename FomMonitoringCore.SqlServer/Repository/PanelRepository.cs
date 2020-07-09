namespace FomMonitoringCore.SqlServer.Repository
{
    public class PanelRepository : GenericRepository<Panel>, IPanelRepository
    {
        public PanelRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
