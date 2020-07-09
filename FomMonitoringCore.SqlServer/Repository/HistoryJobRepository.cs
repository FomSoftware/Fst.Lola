namespace FomMonitoringCore.SqlServer.Repository
{
    public class HistoryJobRepository : GenericRepository<HistoryJob>, IHistoryJobRepository
    {
        public HistoryJobRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
