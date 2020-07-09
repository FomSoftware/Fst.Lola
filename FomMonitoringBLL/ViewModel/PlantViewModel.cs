using System.Collections.Generic;

namespace FomMonitoringBLL.ViewModel
{
    public class PlantViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string CustomerName { get; set; }
        public List<UserMachineViewModel> Machines { get; set; }
        public List<string> MachineSerials { get; set; }
    }
}
