using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository
{
    public class HistoryPieceRepository : GenericRepository<HistoryPiece>, IHistoryPieceRepository
    {
        public HistoryPieceRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
