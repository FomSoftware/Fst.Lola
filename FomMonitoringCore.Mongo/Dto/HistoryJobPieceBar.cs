using System.ComponentModel.DataAnnotations.Schema;
using FomMonitoringCore.DataProcessing.Dto;

namespace FomMonitoringCore.Mongo.Dto
{
    [Table("historyJobPieceBar")]
    public class HistoryJobPieceBar : BaseModel
    {
        public HistoryJobMachine[] historyjob { get; set; }
        public PieceMachine[] piece { get; set; }
        public BarMachine[] bar { get; set; }
    }
}