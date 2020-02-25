using System;

namespace FomMonitoringCore.DataProcessing.Dto
{
    public class VariablesListMachine : BaseModel
    {
        public int Id { get; set; }
        public DateTime UtcDateTime { get; set; }
        public Value[] Values { get; set; }
    }
}