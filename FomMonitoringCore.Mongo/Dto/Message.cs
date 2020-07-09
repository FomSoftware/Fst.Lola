using System.ComponentModel.DataAnnotations.Schema;
using FomMonitoringCore.DataProcessing.Dto;

namespace FomMonitoringCore.Mongo.Dto
{
    [Table("messages")]
    public class Message : BaseModel
    {
        public MessageMachine[] message { get; set; }
    }
}