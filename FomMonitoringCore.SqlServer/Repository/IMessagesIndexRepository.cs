namespace FomMonitoringCore.SqlServer.Repository
{
    public interface IMessagesIndexRepository : IGenericRepository<MessagesIndex>
    {
        MessagesIndex GetByCodeCategory(string code, int category);
    }
}
