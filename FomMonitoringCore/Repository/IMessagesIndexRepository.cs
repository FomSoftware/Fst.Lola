using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository
{
    public interface IMessagesIndexRepository : IGenericRepository<MessagesIndex>
    {
        MessagesIndex GetByCodeCategory(string code, int category);
    }
}
