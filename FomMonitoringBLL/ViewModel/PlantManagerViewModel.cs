using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewModel
{
    public class PlantManagerViewModel
    {
        public List<PlantViewModel> Plants { get; set; }
        public PlantViewModel Plant { get; set; }
        public List<string> Customers { get; set; }
        public List<UserMachineViewModel> Machines { get; set; }
    }
}
