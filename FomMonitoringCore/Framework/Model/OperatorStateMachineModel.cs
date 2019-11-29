using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCore.Framework.Common;

namespace FomMonitoringCore.Framework.Model
{
    public class OperatorStateMachineModel
    {
        public string Operator { get; set; }
        public long? TotalTime { get; set; }
        public Dictionary<int?, long?> StatesTime { get; set; }
        public int? CompletedCount { get; set; }

        public int? machineType { get; set; }

        public long? ProducingTime
        {
            get
            {
                if (StatesTime == null || StatesTime.Count == 0) return null;
                long? prod = StatesTime.ContainsKey((int?)enState.Production) ? StatesTime[(int?)enState.Production] : 0;
                if (machineType == null || machineType != (int) enMachineType.Troncatrice) return prod;
                long? manual = StatesTime.ContainsKey((int)enState.Manual) ? StatesTime[(int?)enState.Manual] : 0;
                if (manual != null)
                    return prod != null ? prod + manual : manual;
                return prod;
            }
        }
    }
}
