using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository
{
    public class MessageLanguagesRepository : GenericRepository<MessageLanguages>, IMessageLanguagesRepository
    {
        public MessageLanguagesRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
