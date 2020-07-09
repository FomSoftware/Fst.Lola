using System.Collections.Generic;

namespace FomMonitoringBLL.ViewModel
{
    public class PanelParameterViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public List<ParameterMachineValueViewModel> Parameters { get; set; }
    }
}
