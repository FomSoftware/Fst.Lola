using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System;

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
            _jobsViewService = jobsViewService;
        }

        public MachineViewModel GetMachine(ContextModel context)
        {
            MachineViewModel machine = new MachineViewModel();
            machine.MachinePanels = _messagesService.GetMachinePanels(context);

            var timeZone = context.User.TimeZone;
            if (context.User.Role == enRole.Customer && context.ActualMachine.TimeZone != null)
            {
                timeZone = context.ActualMachine.TimeZone;
            }

            //prendo come data di lastupdate quella del currentstate
            /*var lastState = _messagesService.GetCurrentStateModel(context.ActualMachine.Id);
            if (lastState != null)
            {
                context.ActualMachine.LastUpdate = lastState.LastUpdated;
            }*/
            
            machine.LastUpdate = new DataUpdateModel()
            {
                DateTime = (DateTime)context.ActualMachine.LastUpdate,
                TimeZone = timeZone
            };
            context.ActualPeriod.LastUpdate.DateTime = machine.LastUpdate.DateTime;
            context.ActualPeriod.LastUpdate.TimeZone = machine.LastUpdate.TimeZone;
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
                machineName = context.ActualMachine.MachineName,
                timeZone = context.ActualMachine.TimeZone,
                modelCode = context.ActualMachine.Model.ModelCodev997
            };
            
            machine.XTools = _xToolsViewService.GetXTools(context);
            machine.Maintenance = _maintenanceViewService.GetMessages(context);

            return machine;
        }
    }
}
