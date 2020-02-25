namespace FomMonitoringCoreQueue.Dto
{
    public class Message : BaseModel
    {
        public FomMonitoringCore.DataProcessing.Dto.MessageMachine[] MessageMachine { get; set; }
    }
}