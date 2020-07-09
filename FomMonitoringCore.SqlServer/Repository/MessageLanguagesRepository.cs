namespace FomMonitoringCore.SqlServer.Repository
{
    public class MessageLanguagesRepository : GenericRepository<MessageLanguages>, IMessageLanguagesRepository
    {
        public MessageLanguagesRepository(IFomMonitoringEntities context) : base(context)
        {

        }

    }
}
