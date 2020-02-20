using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository.SQL
{
    public class SpindleRepository : GenericRepository<Spindle>, ISpindleRepository
    {
        public SpindleRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
