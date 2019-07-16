using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewServices
{

        public class ToolsViewService
        {

            public static ToolViewModel GetTools(ContextModel context)
            {
                ToolViewModel result = new ToolViewModel();
                result.vm_tools = GetVueModel(context.ActualMachine, true);

                return result;
            }


            private static ToolParameterVueModel GetVueModel(MachineInfoModel machine, bool xmodule = false)
            {
                var par = ParameterMachineService.GetParameters(machine, (int)enPanel.Tools);

                var result = new ToolParameterVueModel
                {
                    toolsTf = par.Where(p => p.VarNumber == 416 || p.VarNumber == 418 || p.VarNumber == 420).OrderBy(n => n.VarNumber).ToList(),

                    toolsTm = par.Where(p => p.VarNumber == 422 || p.VarNumber == 424 || p.VarNumber == 426).OrderBy(n => n.VarNumber).ToList(),
                };

                return result;
            }
        
    }
}
