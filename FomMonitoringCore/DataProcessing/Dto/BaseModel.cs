using System;

namespace FomMonitoringCore.DataProcessing.Dto
{
    public class BaseModel
    {
        public DateTime? DateSendedQueue { get; set; }
        public DateTime? DateStartElaboration { get; set; }
        public DateTime? DateEndElaboration { get; set; }
        public bool? ElaborationSuccesfull { get; set; }
    }
}
