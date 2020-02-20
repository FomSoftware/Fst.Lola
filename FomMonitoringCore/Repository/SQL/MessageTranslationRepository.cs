using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository.SQL
{
    public class MessageTranslationRepository : GenericRepository<MessageTranslation>, IMessageTranslationRepository
    {
        public MessageTranslationRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
