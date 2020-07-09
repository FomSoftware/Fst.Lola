namespace FomMonitoringCore.Queue.Dto
{
    public class Tool : BaseModel
    {
        public FomMonitoringCore.DataProcessing.Dto.ToolMachine[] ToolMachine { get; set; }
    }
}