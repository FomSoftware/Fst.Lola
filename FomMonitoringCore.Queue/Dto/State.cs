using FomMonitoringCore.DataProcessing.Dto;

namespace FomMonitoringCore.Queue.Dto
{
    public class State : BaseModel
    {
        public StateMachine[] StateMachine { get; set; }
    }
}