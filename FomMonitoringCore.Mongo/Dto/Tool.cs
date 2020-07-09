using System.ComponentModel.DataAnnotations.Schema;
using FomMonitoringCore.DataProcessing.Dto;

namespace FomMonitoringCore.Mongo.Dto
{
    [Table("tool")]
    public class Tool : BaseModel
    {
        public ToolMachine[] tool { get; set; }
    }
}