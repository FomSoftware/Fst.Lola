using FomMonitoringCore.Framework.Model;
using System.Collections.Generic;

namespace FomMonitoringBLL.ViewModel
{
    public class MotorAxesParameterVueModel
    {
        public List<ParameterMachineValueModel> motors { get; set; } = new List<ParameterMachineValueModel>();
        public List<ParameterMachineValueModel> axes { get; set; } = new List<ParameterMachineValueModel>();

    }
}