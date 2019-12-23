using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewModel
{
    public class FilterViewModel
    {
        public MachineInfoViewModel machine { get; set; }

        public PeriodViewModel period { get; set; }
        public int panelId { get; set; }
        public int cluster { get; set; }
    }
}
