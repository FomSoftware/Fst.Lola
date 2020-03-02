using System.ComponentModel.DataAnnotations.Schema;

namespace FomMonitoringCore.DataProcessing.Dto.Mongo
{
    [Table("messages")]
    public class Message : BaseModel
    {
        public MessageMachine[] message { get; set; }
    }
}