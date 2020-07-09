namespace FomMonitoringCore.SqlServer.Repository
{
    public class HistoryPieceRepository : GenericRepository<HistoryPiece>, IHistoryPieceRepository
    {
        public HistoryPieceRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
