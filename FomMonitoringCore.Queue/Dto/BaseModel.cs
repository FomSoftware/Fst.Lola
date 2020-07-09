using FomMonitoringCore.DataProcessing.Dto;

namespace FomMonitoringCore.Queue.Dto
{
    public class BaseModel
    {
        public string ObjectId { get; set; }
        public InfoMachine[] InfoMachine { get; set; }
    }
}
