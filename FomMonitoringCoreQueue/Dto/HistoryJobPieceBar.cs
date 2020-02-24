namespace FomMonitoringCoreQueue.Dto
{
    public class HistoryJobPieceBar : BaseModel
    {
        public FomMonitoringCore.DataProcessing.Dto.HistoryJob HistoryJobMachine { get; set; }
    }
}