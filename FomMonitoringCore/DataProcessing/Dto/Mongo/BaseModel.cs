using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace FomMonitoringCore.DataProcessing.Dto.Mongo
{
    public class BaseModel
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId Id { get; set; }
        public DateTime? DateReceived { get; set; }

    }
}