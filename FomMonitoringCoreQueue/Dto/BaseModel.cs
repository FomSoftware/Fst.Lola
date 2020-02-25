using FomMonitoringCore.DataProcessing.Dto;

namespace FomMonitoringCoreQueue.Dto
{
    public class BaseModel
    {
        public string ObjectId { get; set; }
        public InfoMachine[] InfoMachine { get; set; }
    }
}
