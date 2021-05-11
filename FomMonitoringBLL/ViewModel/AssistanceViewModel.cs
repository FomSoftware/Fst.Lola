using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewModel
{
    public class AssistanceViewModel
    {
        public List<MachineInfoViewModel> machines { get; set; }
        public List<UserViewModel> customers { get; set; }
    }

}
