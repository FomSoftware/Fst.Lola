using System.ComponentModel.DataAnnotations.Schema;

namespace FomMonitoringCore.DataProcessing.Dto.Mongo
{
    [Table("tool")]
    public class Tool : BaseModel
    {
        public ToolMachine[] tools { get; set; }
    }
}