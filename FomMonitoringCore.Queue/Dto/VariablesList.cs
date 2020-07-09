using System.Collections.Generic;

namespace FomMonitoringCore.Queue.Dto
{

    public class VariablesList : BaseModel
    {
        public IEnumerable<FomMonitoringCore.DataProcessing.Dto.VariablesListMachine> VariablesListMachine { get; set; }
    }
}