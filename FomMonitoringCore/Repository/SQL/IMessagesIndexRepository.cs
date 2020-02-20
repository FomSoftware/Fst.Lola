using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository.SQL
{
    public interface IMessagesIndexRepository : IGenericRepository<MessagesIndex>
    {
        MessagesIndex GetByCodeCategory(string code, int category);
    }
}
