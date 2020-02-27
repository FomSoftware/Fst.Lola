using System.ComponentModel.DataAnnotations.Schema;

namespace FomMonitoringCore.DataProcessing.Dto.Mongo
{
    [Table("state")]
    public class State : BaseModel {

        public StateMachine[] state { get; set; }
    }
}