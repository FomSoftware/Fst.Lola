using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FomMonitoringCore.DalMongoDb;
using FomMonitoringCore.DataProcessing.Dto.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FomMonitoringCore.Repository.MongoDb
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        public IMongoCollection<T> Collection { get; private set; }
        public GenericRepository(IMongoDbContext dbContext)
        {
            Collection = dbContext.DbSet<T>();
        }

        public T Find(object id)
        {
            if (!ObjectId.TryParse(id.ToString(), out var objectId))
            {
                return null;
            }
            var filterId = Builders<T>.Filter.Eq("_id", objectId);
            var model = Collection.Find(filterId).FirstOrDefault();
            return model;
        }

        public bool Update(T model)
        {
            var filterId = Builders<T>.Filter.Eq("_id", model.Id);
            var updated = Collection.FindOneAndReplace(filterId, model);
            return updated != null;
        }

        public void Create(T model)
        {
            Collection.InsertOne(model);
        }

        public bool Delete(object id)
        {
            if (!ObjectId.TryParse(id.ToString(), out var objectId))
            {
                return false;
            }
            var filterId = Builders<T>.Filter.Eq("_id", objectId);
            var deleted = Collection.FindOneAndDelete(filterId);
            return deleted != null;
        }

        public IEnumerable<T> Query()
        {
            return Collection.Find(FilterDefinition<T>.Empty).ToList();
        }

        public IEnumerable<T> Query(Expression<Func<T, bool>> filter)
        {
            return Collection.Find(filter).ToList();
        }
    }
}