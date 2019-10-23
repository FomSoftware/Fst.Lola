using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository
{
    public class SpindleRepository : GenericRepository<Spindle>, ISpindleRepository
    {
        public SpindleRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
