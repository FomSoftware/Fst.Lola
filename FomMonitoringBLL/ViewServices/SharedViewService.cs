using System;
using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Framework.Common;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using FomMonitoringCore.Service;

namespace FomMonitoringBLL.ViewServices
{
    public class SharedViewService
    {
        public static HeaderViewModel GetHeader(ContextModel context)
        {
            HeaderViewModel header = new HeaderViewModel();
            header.ControllerPage = context.ActualPage.GetController();                         
            header.ActionPage = context.ActualPage.GetDescription();
            header.ActualPeriod = context.ActualPeriod;
            header.User = context.User;
            header.AllLanguages = context.AllLanguages;
            header.ActualLanguage = context.ActualLanguage;

            return header;
        }

        public static ToolbarViewModel GetToolbar(ContextModel context)
        {
            ToolbarViewModel toolbar = new ToolbarViewModel();
            toolbar.page = context.ActualPage;
            toolbar.role = context.User.Role;
            toolbar.language = new LanguageViewModel();
            toolbar.language.initial = context.ActualLanguage.InitialsLanguage;
            toolbar.language.labels = new CalendarI18nModel();

            if (context.ActualPage == enPage.Mes)
            {
                toolbar.plants = GetListPlants(context);
                toolbar.selected_plant = toolbar.plants.FirstOrDefault(w => w.id == context.ActualPlant.Id);
                toolbar.period = new PeriodViewModel();
                toolbar.period.start = context.ActualPeriod.StartDate;
                toolbar.period.end = context.ActualPeriod.EndDate;
            }

            if (context.ActualPage == enPage.PlantMessages)
            {
                toolbar.plants = GetListPlants(context);
                toolbar.selected_plant = toolbar.plants.FirstOrDefault(w => w.id == context.ActualPlant.Id);
                toolbar.period = new PeriodViewModel();
                if(context.ActualPeriod.StartDate.Year == 1)
                {
                    context.ActualPeriod.StartDate = DateTime.UtcNow.AddDays(-30);
                    context.ActualPeriod.EndDate = DateTime.UtcNow;
                }

                toolbar.period.start = context.ActualPeriod.StartDate;
                toolbar.period.end = context.ActualPeriod.EndDate;
            }

            if (context.ActualPage == enPage.Machine)
            {
                toolbar.period = new PeriodViewModel();
                toolbar.period.start = context.ActualPeriod.StartDate;
                toolbar.period.end = context.ActualPeriod.EndDate;

                toolbar.machines = GetListMachines(context);

                toolbar.selected_machine = toolbar.machines.FirstOrDefault(w => w.id == context.ActualMachine.Id);

            }

            return toolbar;
        }

        public static List<MachineInfoViewModel> GetListMachines(ContextModel context)
        {

                var machines = context.AllMachines.Where(w => w.Id == context.ActualMachine.Id || 
                                                              (w.PlantId == context.ActualPlant?.Id &&
                                                               (w.ExpirationDate == null || w.ExpirationDate >= DateTime.UtcNow))).Select(m => new MachineInfoViewModel()
                {
                    id = m.Id,
                    serial = m.Serial,
                    icon = m.Type?.Image,
                    mtype = LocalizationService.GetResource(m.Type?.Name?.ToLower().Replace(' ', '_')),
                    model = m.Model?.Name,
                    description = m.Description,
                    product_type = m.Product,
                    product_version = m.ProductVersion,
                    machineName = m.MachineName,
                    modelCode = m.Model != null ? m.Model.ModelCodev997 : 0,
                    activation = m.ActivationDate?.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern) ?? string.Empty
                }).ToList();

                return machines;
            
        }

        public static List<PlantInfoViewModel> GetListPlants(ContextModel context)
        {
            var plants = context.AllPlants.Select(p => new PlantInfoViewModel
            {
                id = p.Id,
                name = p.Name
            }).ToList();

            return plants;
        }
    }
}
