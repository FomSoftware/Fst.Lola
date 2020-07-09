using FomMonitoringCore.Mongo.Dto;
using MongoDB.Driver;

namespace FomMonitoringCore.Mongo
{
    public interface IMongoDbContext
    {
        IMongoCollection<T> DbSet<T>() where T : BaseModel;

    }
}