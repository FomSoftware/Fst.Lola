using System.Collections.Generic;

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
