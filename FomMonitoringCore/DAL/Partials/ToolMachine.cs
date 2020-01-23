using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.DAL
{
    public partial class ToolMachine
    {
        public int CodeAsInt {
            get {
            if(int.TryParse(this.Code, out var x))
                return x;
            return 0;
            }
        }
    }
}
