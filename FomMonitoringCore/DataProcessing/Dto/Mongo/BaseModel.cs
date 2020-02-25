using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json.Linq;

namespace FomMonitoringCore.DataProcessing.Dto.Mongo
{
    public class BaseModel
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId Id { get; set; }
        public DateTime? DateReceived { get; set; }
        public InfoMachine[] info { get; set; }
        public bool IsCumulative { get; set; }

    }


    [Table("unknown")]
    public class Unknown : BaseModel
    {

        public Unknown(BaseModel data)
        {
            DateReceived = data.DateReceived;
            info = data.info;
            IsCumulative = data.IsCumulative;
        }

        public BsonDocument EntityUnknown { get; set; }
        public Dictionary<string,string> ErrorDataVariablesList { get; set; }
        public Dictionary<string, string> ErrorMessages { get; set; }
        public Dictionary<string, string> ErrorHistoryJobPieceBar { get; set; }
        public Dictionary<string, string> ErrorStates { get; set; }
        public Dictionary<string, string> ErrorTool { get; set; }
    }
}