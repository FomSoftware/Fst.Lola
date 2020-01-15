﻿using FomMonitoringBLL.ViewModel;
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
    public class MachineViewService : IMachineViewService
    {
        private readonly IMessagesViewService _messagesViewService;
        private readonly IMaintenanceViewService _maintenanceViewService;
        private readonly IEfficiencyViewService _efficiencyViewService;
        private readonly IProductivityViewService _productivityViewService;
        private readonly IMachineService _messagesService;
        private readonly IToolsViewService _toolsViewService;
        private readonly IPanelParametersViewService _panelParametersViewService;
        private readonly IXToolsViewService _xToolsViewService;
        private readonly ISpindleViewService _spindleViewService;
        private readonly IJobsViewService _jobsViewService;

        public MachineViewService(
            IMessagesViewService messagesViewService,
            IMachineService messagesService,
            IMaintenanceViewService maintenanceViewService,
            IEfficiencyViewService efficiencyViewService,
            IProductivityViewService productivityViewService,
            IPanelParametersViewService panelParametersViewService,
            IXToolsViewService xToolsViewService,
            IToolsViewService toolsViewService,
            ISpindleViewService spindleViewService,
            IJobsViewService jobsViewService)
        {
            _messagesViewService = messagesViewService;
            _maintenanceViewService = maintenanceViewService;
            _efficiencyViewService = efficiencyViewService;
            _productivityViewService = productivityViewService;
            _messagesService = messagesService;
            _toolsViewService = toolsViewService;
            _panelParametersViewService = panelParametersViewService;
            _xToolsViewService = xToolsViewService;
            _spindleViewService = spindleViewService;
            _jobsViewService = jobsViewService;
        }

        public MachineViewModel GetMachine(ContextModel context)
        {
            MachineViewModel machine = new MachineViewModel();
            machine.MachinePanels = _messagesService.GetMachinePanels(context);

            machine.LastUpdate = new DataUpdateModel() { DateTime = (DateTime)context.ActualMachine.LastUpdate };
            context.ActualPeriod.LastUpdate.DateTime = machine.LastUpdate.DateTime;
            machine.Efficiency = _efficiencyViewService.GetEfficiency(context);
            machine.Productivity = _productivityViewService.GetProductivity(context);
            machine.Messages = _messagesViewService.GetMessages(context);
            machine.Jobs = _jobsViewService.GetJobs(context);
            machine.PanelParameter = _panelParametersViewService.GetParameters(context);
            machine.Tools = _toolsViewService.GetTools(context);           
            machine.MachineInfo = new MachineInfoViewModel
            {
                model = context.ActualMachine.Model.Name,
                mtype = context.ActualMachine.Type.Name,
                id_mtype = context.ActualMachine.Type.Id,
                machineName = context.ActualMachine.MachineName
            };
            if (!machine.MachinePanels.Contains((int) enPanel.XSpindles))
            {
                machine.XSpindles = _spindleViewService.GetXSpindles(context);
            }
            machine.XTools = _xToolsViewService.GetXTools(context);
            machine.Maintenance = _maintenanceViewService.GetMessages(context);           
            return machine;
        }
    }
}
