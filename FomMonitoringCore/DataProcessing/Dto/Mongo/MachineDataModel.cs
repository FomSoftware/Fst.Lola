using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FomMonitoringCore.DataProcessing.Dto.Mongo
{
    [Table("json")]
    public class MachineDataModel : BaseModel
    {
        
        public VariablesList[] variablesList { get; set; }
        public Message[] message { get; set; }

    }

    [Table("info")]
    public class Info : BaseModel
    {

    }

    [Table("state")]
    public class State : BaseModel {

        public StateMachine[] state { get; set; }
    }


    [Table("historyJobPieceBar")]
    public class HistoryJobPieceBar : BaseModel
    {
        public HistoryJobMachine[] historyjob { get; set; }
        public PieceMachine[] piece { get; set; }
        public BarMachine[] bar { get; set; }
    }

    [Table("tool")]
    public class Tool : BaseModel
    {
        public ToolMachine[] tool { get; set; }
    }


    [Table("variablesList")]
    public class VariablesList : BaseModel
    {
        public VariablesListMachine[] variablesList { get; set; }
    }

    [Table("messages")]
    public class Message : BaseModel
    {
        public MessageMachine[] messages { get; set; }
    }
}
