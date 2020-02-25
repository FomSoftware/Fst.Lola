using FomMonitoringCore.DataProcessing.Dto.Mongo;
using MongoDB.Driver;

namespace FomMonitoringCore.DalMongoDb
{
    public interface IMongoDbContext
    {
        IMongoCollection<T> DbSet<T>() where T : BaseModel;

    }
}