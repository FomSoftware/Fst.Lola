using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class PlantMessagesViewService
    {
        public static PlantMessagesViewModel GetPlantMessages(ContextModel context)
        {
            PlantMessagesViewModel result = new PlantMessagesViewModel();           
            result.messages = GetVueModel(context.ActualPlant, context.AllMachines, context.ActualPeriod);
            result.plant = new PlantInfoViewModel()
            {
                id = context.ActualPlant.Id,
                name = context.ActualPlant.Name
            };
            SortingViewModel sorting = new SortingViewModel();
            sorting.timestamp = enSorting.Descending.GetDescription();

            //sorting.group = enSorting.Ascending.GetDescription();
            result.sorting = sorting;

            return result;
        }


        public static List<MachineMessagesDataViewModel> GetVueModel(PlantModel plant, List<MachineInfoModel> allMachines, PeriodModel period)
        {
            List<MachineMessagesDataViewModel> result = new List<MachineMessagesDataViewModel>();

            List<MesUserMachinesModel> dataAllMachines = MesService.GetPlantData(plant);

            foreach (MesUserMachinesModel dataMachine in dataAllMachines)
            {
                MachineMessagesDataViewModel machineMsgs = new MachineMessagesDataViewModel();

                MachineInfoModel machine = allMachines.Where(w => w.Id == dataMachine.MachineId).FirstOrDefault();

                if (machine == null)
                {
                    continue;
                }

                MachineInfoViewModel machineInfo = new MachineInfoViewModel()
                {
                    id = machine.Id,
                    description = machine.Description == string.Empty ? null : machine.Description,
                    model = machine.Model.Name,
                    icon = machine.Type.Image,
                    serial = machine.Serial
                };

               
                List<MessageMachineModel> data = MessageService.GetMessageDetails(machine, period);

                if (data.Count == 0)
                    continue;

                List<MessageDetailViewModel> msgDet = data.Select(a => new MessageDetailViewModel()
                {
                    code = a.Code,
                    parameters = a.Params,
                    timestamp = a.Day,
                    type = ((enTypeAlarm)a.StateId).GetDescription(),
                    group = a.Group,
                    time = CommonViewService.getTimeViewModel(a.ElapsedTime),
                    description = ReadMessages.GetMessageDescription(a.Code, a.Params, CultureInfo.CurrentCulture.EnglishName)

                }).ToList();

                foreach(MessageDetailViewModel det in msgDet)
                {
                    MachineMessagesDataViewModel msg = new MachineMessagesDataViewModel();
                    msg.message = det;
                    msg.machine = machineInfo;
                    result.Add(msg);
                }

            }
            if (result.Count == 0) result = null;

            return result;
        }

    }
}
