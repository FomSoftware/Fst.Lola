using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewServices
{
    public class ParameterMachineViewService
    {
        public IEnumerable<ParameterMachineViewModel> GetPanelParameter(enPanel panel)
        {
            var r = new List<ParameterMachineViewModel>();
            switch (panel)
            {
                case enPanel.Efficiency:
                    break;
                case enPanel.Messages:
                    break;
                case enPanel.Productivity:
                    break;
                case enPanel.Orders:
                    break;
                case enPanel.Maintance:
                    break;
                case enPanel.Multispindle:
                    break;
                case enPanel.Tools:
                    break;
                default:
                    break;
            }
            return r;
        }
    }
}
