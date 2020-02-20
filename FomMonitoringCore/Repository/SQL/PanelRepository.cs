using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository.SQL
{
    public class PanelRepository : GenericRepository<Panel>, IPanelRepository
    {
        public PanelRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
