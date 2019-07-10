using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.Framework.Model
{
    public class JsonVariablesModel
    {
        public int Id { get; set; }
        public DateTime UtcDateTime { get; set; }
        public List<JsonVariableValueModel> Values { get; set; }
    }
}
