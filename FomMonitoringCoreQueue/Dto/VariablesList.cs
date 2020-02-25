using System.Collections.Generic;

namespace FomMonitoringCoreQueue.Dto
{

    public class VariablesList : BaseModel
    {
        public IEnumerable<FomMonitoringCore.DataProcessing.Dto.VariablesListMachine> VariablesListMachine { get; set; }
    }
}