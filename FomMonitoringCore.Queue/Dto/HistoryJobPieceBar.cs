namespace FomMonitoringCore.Queue.Dto
{
    public class HistoryJobPieceBar : BaseModel
    {
        public FomMonitoringCore.DataProcessing.Dto.HistoryJobMachine[] HistoryJobMachine { get; set; }
        public FomMonitoringCore.DataProcessing.Dto.PieceMachine[] PieceMachine { get; set; }
        public FomMonitoringCore.DataProcessing.Dto.BarMachine[] BarMachine { get; set; }
    }
}