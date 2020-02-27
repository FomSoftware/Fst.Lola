using System.ComponentModel.DataAnnotations.Schema;

namespace FomMonitoringCore.DataProcessing.Dto.Mongo
{
    [Table("variablesList")]
    public class VariablesList : BaseModel
    {
        public VariablesListMachine[] variablesList { get; set; }
    }
}