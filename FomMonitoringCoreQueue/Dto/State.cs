using System.Collections.Generic;
using FomMonitoringCore.DataProcessing.Dto;

namespace FomMonitoringCoreQueue.Dto
{
    public class State : BaseModel
    {
        public IEnumerable<StateMachine> StateMachine { get; set; }
    }
}