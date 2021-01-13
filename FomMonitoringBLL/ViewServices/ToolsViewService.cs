using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{

        public class ToolsViewService : IToolsViewService
    {
        private readonly IMachineService _machineService;
        private readonly IContextService _contextService;
        private readonly IParameterMachineService _parameterMachineService;
        private readonly IToolService _toolService;

        public ToolsViewService(IMachineService machineService, IContextService contextService,
             IToolService toolService, IParameterMachineService parameterMachineService)
        {
            _machineService = machineService;
            _contextService = contextService;
            _parameterMachineService = parameterMachineService;
            _toolService = toolService;
        }
            public ToolViewModel GetTools(ContextModel context)
            {
                var result = new ToolViewModel();
                if(_machineService.GetMachinePanels(context).Contains((int)enPanel.ToolsBlitz))
                    result.vm_tools_blitz = GetVueModelBlitz(context.ActualMachine);
                else
                    result.vm_tools = GetVueModel(context.ActualMachine, true);
                

                return result;
            }


            private ToolVueModel GetVueModel(MachineInfoModel machine, bool xmodule = false)
            {
                var result = new ToolVueModel();

                var data = _toolService.GetTools(machine, xmodule).ToList();

                var dataTools = data.Where(w => w.IsActive).ToList();
                var dataHistorical = data.Where(w => w.IsActive == false).ToList();

                if (dataTools.Count == 0)
                    return result;
                var lan = _contextService.GetContext().ActualLanguage.InitialsLanguage;
                var tools = dataTools.Select(t => new ToolDataModel()
                {
                    code = t.Code,
                    description = t.Description,
                    perc = Common.GetPercentage(t.CurrentLife, t.ExpectedLife),
                    changes = new ChangeModel()
                    {
                        breaking = 0,
                        replacement = 0,
                        historical = dataHistorical.Where(w => w.Code == t.Code).Select(h => new HistoricalModel()
                        {
                            date = h.DateReplaced.ToString(),
                            type = CommonViewService.GetTypeTool(h).ToLocalizedString(lan),
                            color_type = CommonViewService.GetTypeTool(h).GetDescription(),
                            duration = CommonViewService.getTimeViewModel(h.CurrentLife)
                        }).OrderByDescending(o => o.date).ToList()
                    },
                    time = CommonViewService.getTimeViewModel((t.ExpectedLife ?? 0) - (t.CurrentLife ?? 0))
                }).ToList();

                tools = tools.OrderByDescending(o => o.perc).ToList();

                var sorting = new SortingViewModel();
                sorting.time = enSorting.Descending.GetDescription();

                result.tools = tools;
                result.sorting = sorting;

                return result;
            }

            private ToolParameterVueModel GetVueModelBlitz(MachineInfoModel machine)
            {
                
                var par = _parameterMachineService.GetParameters(machine, (int)enPanel.ToolsBlitz);

                var result = new ToolParameterVueModel
                {
                    toolsTf = par.Where(p => p.VarNumber == 416 || p.VarNumber == 420).OrderBy(n => n.VarNumber).ToList(),

                    toolsTm = par.Where(p => p.VarNumber == 422 || p.VarNumber == 426).OrderByDescending(n => n.VarNumber).ToList(),
                };

                foreach (var t1 in result.toolsTf)
                {
                    t1.Value = double.IsNaN(double.Parse(t1.Value)) ? "" : double.Parse(t1.Value).ToString("0");
                }

                foreach (var t2 in result.toolsTm)
                {
                    t2.Value = double.IsNaN(double.Parse(t2.Value)) ? "" : double.Parse(t2.Value).ToString("0");
                }

                return result;
            }
        
    }
}
