﻿using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class XToolsViewService : IXToolsViewService
    {
        private readonly IContextService _contextService;
        private readonly IToolService _toolService;

        public XToolsViewService(IContextService contextService, IToolService toolService)
        {
            _contextService = contextService;
            _toolService = toolService;
        }
        public XToolViewModel GetXTools(ContextModel context)
        {
            XToolViewModel result = new XToolViewModel();
            result.vm_tools = GetVueModel(context.ActualMachine, true);

            return result;
        }


        private ToolVueModel GetVueModel(MachineInfoModel machine, bool xmodule = false)
        {
            ToolVueModel result = new ToolVueModel();

            List<ToolMachineModel> data = _toolService.GetTools(machine, xmodule);

            List<ToolMachineModel> dataTools = data.Where(w => w.IsActive).ToList();
            //List<ToolMachineModel> dataHistorical = data.Where(w => w.IsActive == false).ToList();

            if (dataTools.Count == 0)
                return result;
            var lan = _contextService.GetContext().ActualLanguage.InitialsLanguage;
            List<ToolDataModel> tools = dataTools.Select(t => new ToolDataModel()
            {
                code = t.Code,
                description = t.Description,
                perc = Common.GetPercentage(t.CurrentLife, t.ExpectedLife),
                changes = new ChangeModel()
                {
                    //total = (t.BrokenEventsCount ?? 0) + (t.RevisedEventsCount ?? 0),
                    breaking = 0,
                    replacement = 0,
                    //historical = dataHistorical.Where(w => w.Code == t.Code).Select(h => new HistoricalModel()
                    //{
                    //    date = h.DateReplaced.ToString(),
                    //    type = CommonViewService.GetTypeTool(h).ToLocalizedString(lan),
                    //    color_type = CommonViewService.GetTypeTool(h).GetDescription(),
                    //    duration = CommonViewService.getTimeViewModel(h.CurrentLife)
                    //}).OrderByDescending(o => o.date).ToList()
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
