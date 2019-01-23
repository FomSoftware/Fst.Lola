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
            result.vm_tools = GetVueModel(context.ActualMachine);

            return result;
        }

        private static ToolVueModel GetVueModel(MachineInfoModel machine)
        {
            ToolVueModel result = new ToolVueModel();

            List<ToolMachineModel> data = ToolService.GetTools(machine);

            List<ToolMachineModel> dataTools = data.Where(w => w.IsActive == true).ToList();
            List<ToolMachineModel> dataHistorical = data.Where(w => w.IsActive == false).ToList();

            if (dataTools.Count == 0)
                return result;

            List<ToolDataModel> tools = dataTools.Select(t => new ToolDataModel()
            {
                code = t.Code,
                description = t.Description,
                perc = Common.GetPercentage(t.CurrentLife, t.ExpectedLife),
                changes = new ChangeModel()
                {
                    total = (t.BrokenEventsCount ?? 0) + (t.RevisedEventsCount ?? 0),
                    breaking = t.BrokenEventsCount ?? 0,
                    replacement = t.RevisedEventsCount ?? 0,
                    historical = dataHistorical.Where(w => w.Code == t.Code).Select(h => new HistoricalModel()
                    {
                        date = h.DateReplaced.ToString(),
                        type = CommonViewService.GetTypeTool(h).ToLocalizedString(),
                        color_type = CommonViewService.GetTypeTool(h).GetDescription(),
                        duration = CommonViewService.getTimeViewModel(h.CurrentLife)
                    }).OrderByDescending(o => o.date).ToList()
                },
                time = CommonViewService.getTimeViewModel((t.ExpectedLife ?? 0) - (t.CurrentLife ?? 0))
            }).ToList();

            tools = tools.OrderByDescending(o => o.perc).ToList();

            SortingViewModel sorting = new SortingViewModel();
            sorting.time = enSorting.Descending.GetDescription();

            result.tools = tools;
            result.sorting = sorting;

            return result;
        }
    }
}
