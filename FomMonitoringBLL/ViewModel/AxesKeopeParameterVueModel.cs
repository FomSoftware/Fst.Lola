using FomMonitoringCore.Framework.Model;
using System.Collections.Generic;

namespace FomMonitoringBLL.ViewModel
{
    public class AxesKeopeParameterVueModel
    {
        public List<ParameterMachineValueModel> axes { get; set; } = new List<ParameterMachineValueModel>();        
    }
}