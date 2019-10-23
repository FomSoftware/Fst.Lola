using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository
{
    public class MessageTranslationRepository : GenericRepository<MessageTranslation>, IMessageTranslationRepository
    {
        public MessageTranslationRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
