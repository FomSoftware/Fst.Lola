using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewServices
{
    public class SharedViewService
    {
        public static HeaderViewModel GetHeader(ContextModel context)
        {
            HeaderViewModel header = new HeaderViewModel();
            header.ControllerPage = context.ActualPage.ToString();
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
                toolbar.selected_plant = toolbar.plants.Where(w => w.id == context.ActualPlant.Id).FirstOrDefault();
            }

            if (context.ActualPage == enPage.Machine)
            {
                toolbar.period = new PeriodViewModel();
                toolbar.period.start = context.ActualPeriod.StartDate;
                toolbar.period.end = context.ActualPeriod.EndDate;

                toolbar.machines = GetListMachines(context);
                toolbar.selected_machine = toolbar.machines.Where(w => w.id == context.ActualMachine.Id).FirstOrDefault();
            }

            return toolbar;
        }

        public static List<MachineInfoViewModel> GetListMachines(ContextModel context)
        {
            List<MachineInfoViewModel> machines = new List<MachineInfoViewModel>();

            machines = context.AllMachines.Select(m => new MachineInfoViewModel()
            {
                id = m.Id,
                serial = m.Serial,
                icon = m.Type.Image,
                mtype = m.Type.Name,
                model = m.Model.Name,
                description = m.Description,
                product_type = m.Product,
                product_version = m.ProductVersion,
                installation = m.InstallationDate.ToString()
            }).ToList();

            return machines;
        }

        public static List<PlantInfoViewModel> GetListPlants(ContextModel context)
        {
            List<PlantInfoViewModel> plants = new List<PlantInfoViewModel>();

            plants = context.AllPlants.Select(p => new PlantInfoViewModel()
            {
                id = p.Id,
                name = p.Name
            }).ToList();

            return plants;
        }
    }
}
