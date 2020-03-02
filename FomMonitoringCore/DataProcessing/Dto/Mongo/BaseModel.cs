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
        public InfoMachine[] info { get; set; }
        public bool IsCumulative { get; set; }
        public DateTime? DateSendedQueue { get; set; }
        public DateTime? DateStartElaboration { get; set; }
        public DateTime? DateEndElaboration { get; set; }
        public bool? ElaborationSuccesfull { get; set; }

    }
}