using FomMonitoringCore.DAL;
using System.Linq;

namespace FomMonitoringCore.Repository
{
    public class MessagesIndexRepository : GenericRepository<MessagesIndex>, IMessagesIndexRepository
    {
        public MessagesIndexRepository(IFomMonitoringEntities context) : base(context)
        {

        }

        public MessagesIndex GetByCodeCategory(string code, int category)
        {
            var query = context.Set<MessagesIndex>().FirstOrDefault(f => f.MessageCode == code && f.MessageCategoryId == category);

            return query;
        }
    }
}
