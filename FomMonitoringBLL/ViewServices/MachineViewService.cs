using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Framework.Models;
using FomMonitoringCore.Service;
using FomMonitoringResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewServices
{
    public class MachineViewService
    {
        public static MachineViewModel GetMachine(ContextModel context)
        {
            MachineViewModel machine = new MachineViewModel();

            machine.LastUpdate = new DataUpdateModel() { DateTime = context.ActualPeriod.LastUpdate.DateTime };
            machine.Efficiency = EfficiencyViewService.GetEfficiency(context);
            machine.Productivity = ProductivityViewService.GetProductivity(context);
            //machine.Alarms = AlarmsViewService.GetAlarms(context);
            machine.Messages = MessagesViewService.GetMessages(context);
            machine.Jobs = JobsViewService.GetJobs(context);
            machine.Spindles = SpindlesViewService.GetSpindles(context);
            machine.Tools = ToolsViewService.GetTools(context);
            machine.MachineInfo = new MachineInfoViewModel
            {
                model = context.ActualMachine.Model.Name,
                mtype = context.ActualMachine.Type.Name,
                id_mtype = context.ActualMachine.Type.Id,
                machineName = context.ActualMachine.MachineName
            };
            machine.XSpindles = SpindlesViewService.GetXSpindles(context);
            machine.XTools = XToolsViewService.GetXTools(context);
            machine.Maintenance = MaintenanceViewService.GetMessages(context);
            return machine;
        }
    }
}
