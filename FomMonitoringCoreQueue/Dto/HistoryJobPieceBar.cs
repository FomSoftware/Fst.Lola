namespace FomMonitoringCoreQueue.Dto
{
    public class HistoryJobPieceBar : BaseModel
    {
        public FomMonitoringCore.DataProcessing.Dto.HistoryJobMachine[] HistoryJobMachine { get; set; }
    }
}