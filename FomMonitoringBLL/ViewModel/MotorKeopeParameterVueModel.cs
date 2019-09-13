using FomMonitoringCore.Framework.Model;
using System.Collections.Generic;

namespace FomMonitoringBLL.ViewModel
{
    public class MotorKeopeParameterVueModel
    {
        public List<ParameterMachineValueModel> fixedHead { get; set; } = new List<ParameterMachineValueModel>();
        public List<ParameterMachineValueModel> mobileHead { get; set; } = new List<ParameterMachineValueModel>();

    }
}