using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.Framework.Model
{
    public class ParameterMachineValueModel
    {
        public int VarNumber { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTime? UtcDateTime { get; set; }
    }
}
