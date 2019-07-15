using FomMonitoringBLL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewModel
{
    public class PanelParameterViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public List<ParameterMachineValueViewModel> Parameters { get; set; }
    }
}
