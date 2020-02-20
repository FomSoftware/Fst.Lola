using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository.SQL
{
    public class HistoryJobRepository : GenericRepository<HistoryJob>, IHistoryJobRepository
    {
        public HistoryJobRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
