namespace FomMonitoringCoreQueue.Dto
{
    public class HistoryJob : BaseModel
    {
        public FomMonitoringCore.DataProcessing.Dto.HistoryJob HistoryJobMachine { get; set; }
    }
}