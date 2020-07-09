using System.ComponentModel.DataAnnotations.Schema;
using FomMonitoringCore.DataProcessing.Dto;

namespace FomMonitoringCore.Mongo.Dto
{
    [Table("variablesList")]
    public class VariablesList : BaseModel
    {
        public VariablesListMachine[] variablesList { get; set; }
    }
}