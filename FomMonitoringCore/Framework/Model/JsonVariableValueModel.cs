using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.Framework.Model
{
    public class JsonVariableValueModel
    {
        public int VariableType { get; set; }
        public string VariableNumber { get; set; }
        public decimal VariableValue { get; set; }
    }
}
