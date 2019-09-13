using FomMonitoringCore.Framework.Model;
using System.Collections.Generic;

namespace FomMonitoringBLL.ViewModel
{
    public class ToolParameterVueModel
    {
        public List<ParameterMachineValueModel> toolsTm { get; set; } = new List<ParameterMachineValueModel>();
        public List<ParameterMachineValueModel> toolsTf { get; set; } = new List<ParameterMachineValueModel>();

    }
}