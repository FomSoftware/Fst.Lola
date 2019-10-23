using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository
{
    public class HistoryJobRepository : GenericRepository<HistoryJob>, IHistoryJobRepository
    {
        public HistoryJobRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
