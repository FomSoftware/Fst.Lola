using System.Collections.Generic;
using FomMonitoringCore.DataProcessing.Dto;

namespace FomMonitoringCoreQueue.Dto
{
    public class State : BaseModel
    {
        public StateMachine[] StateMachine { get; set; }
    }
}