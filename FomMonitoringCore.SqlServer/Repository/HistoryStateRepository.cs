namespace FomMonitoringCore.SqlServer.Repository
{
    public class HistoryStateRepository : GenericRepository<HistoryState>, IHistoryStateRepository
    {
        public HistoryStateRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
