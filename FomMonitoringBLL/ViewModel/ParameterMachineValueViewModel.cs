using System;

namespace FomMonitoringBLL.ViewModel
{

    public class ParameterMachineValueViewModel
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public int ParameterMachineId { get; set; }
        public string Keyword { get; set; }
        public string VarNumber { get; set; }
        public decimal? VarValue { get; set; }
        public DateTime UtcDateTime { get; set; }
    }
}
