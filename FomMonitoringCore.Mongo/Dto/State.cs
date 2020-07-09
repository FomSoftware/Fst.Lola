using System.ComponentModel.DataAnnotations.Schema;
using FomMonitoringCore.DataProcessing.Dto;

namespace FomMonitoringCore.Mongo.Dto
{
    [Table("state")]
    public class State : BaseModel {

        public StateMachine[] state { get; set; }
    }
}