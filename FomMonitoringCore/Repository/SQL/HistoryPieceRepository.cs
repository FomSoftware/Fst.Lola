using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository.SQL
{
    public class HistoryPieceRepository : GenericRepository<HistoryPiece>, IHistoryPieceRepository
    {
        public HistoryPieceRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
