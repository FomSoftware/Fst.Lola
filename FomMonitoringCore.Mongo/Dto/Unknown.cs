using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FomMonitoringCore.Mongo.Dto
{
    [Table("unknown")]
    public class Unknown : BaseModel
    {

        public Unknown(BaseModel data)
        {
            DateReceived = data.DateReceived;
            info = data.info;
            IsCumulative = data.IsCumulative;
        }

        public string EntityUnknown { get; set; }
        public Dictionary<string,string> ErrorDataVariablesList { get; set; }
        public Dictionary<string, string> ErrorMessages { get; set; }
        public Dictionary<string, string> ErrorHistoryJobPieceBar { get; set; }
        public Dictionary<string, string> ErrorStates { get; set; }
        public Dictionary<string, string> ErrorTool { get; set; }
    }
}