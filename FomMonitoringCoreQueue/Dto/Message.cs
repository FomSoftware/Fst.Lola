namespace FomMonitoringCoreQueue.Dto
{
    public class Message : BaseModel
    {
        public FomMonitoringCore.DataProcessing.Dto.Message MessageMachine { get; set; }
    }
}