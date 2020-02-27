using System.ComponentModel.DataAnnotations.Schema;

namespace FomMonitoringCore.DataProcessing.Dto.Mongo
{
    [Table("historyJobPieceBar")]
    public class HistoryJobPieceBar : BaseModel
    {
        public HistoryJobMachine[] historyjob { get; set; }
        public PieceMachine[] piece { get; set; }
        public BarMachine[] bar { get; set; }
    }
}