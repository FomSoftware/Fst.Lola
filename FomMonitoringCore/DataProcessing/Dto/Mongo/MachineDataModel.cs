using System.ComponentModel.DataAnnotations.Schema;

namespace FomMonitoringCore.DataProcessing.Dto.Mongo
{
    [Table("json")]
    public class MachineDataModel : BaseModel
    {
        public bool IsCumulative { get; set; }
        public StateMachine[] state { get; set; }
        public InfoMachine[] info { get; set; }
        public VariablesList[] variablesList { get; set; }
        public Bar[] bar { get; set; }
        public HistoryJob[] historyjob { get; set; }
        public Piece[] piece { get; set; }
        public Spindle[] spindle { get; set; }
        public Tool[] tool { get; set; }
        public Message[] message { get; set; }

    }
}
