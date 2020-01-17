using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class PlantMessagesViewService : IPlantMessagesViewService
    {
        private readonly IMessageService _messageService;
        private readonly IReadMessages _readMessages;
        private readonly IMesService _mesService;

        public PlantMessagesViewService(IMessageService messageService, IReadMessages readMessages, IMesService mesService)
        {
            _messageService = messageService;
            _readMessages = readMessages;
            _mesService = mesService;
        }

        public PlantMessagesViewModel GetPlantMessages(ContextModel context)
        {
            PlantMessagesViewModel result = new PlantMessagesViewModel();           
            result.messages = GetVueModel(context.ActualPlant, context.AllMachines, context.ActualPeriod);
            result.plant = new PlantInfoViewModel()
            {
                id = context.ActualPlant.Id,
                name = context.ActualPlant.Name
            };
            SortingViewModel sorting = new SortingViewModel
            {
                timestamp = enSorting.Descending.GetDescription()
            };
            
            result.sorting = sorting;

            result.UtcOffset = context.AllMachines.FirstOrDefault(w => w.Id == (result.messages?.FirstOrDefault()?.machine.id ?? 0))?.UTC ?? 0; 
            return result;
        }


        public List<MachineMessagesDataViewModel> GetVueModel(PlantModel plant, List<MachineInfoModel> allMachines, PeriodModel period)
        {
            List<MachineMessagesDataViewModel> result = new List<MachineMessagesDataViewModel>();

            List<MesUserMachinesModel> dataAllMachines = _mesService.GetPlantData(plant);

            foreach (MesUserMachinesModel dataMachine in dataAllMachines)
            {

                MachineInfoModel machine = allMachines.FirstOrDefault(w => w.Id == dataMachine.MachineId);

                if (machine == null)
                {
                    continue;
                }

                MachineInfoViewModel machineInfo = new MachineInfoViewModel()
                {
                    id = machine.Id,
                    description = machine.Description == string.Empty ? null : machine.Description,
                    model = machine.Model.Name,
                    machineName = machine.MachineName,
                    icon = machine.Type.Image,
                    serial = machine.Serial
                };

               
                List<MessageMachineModel> data = _messageService.GetMessageDetails(machine, period);

                if (data.Count == 0)
                    continue;

                List<MessageDetailViewModel> msgDet = data.Select(a => new MessageDetailViewModel()
                {
                    code = a.Code,
                    parameters = a.Params,
                    timestamp = DateTime.SpecifyKind(a.Day ?? DateTime.MinValue, DateTimeKind.Utc),
                    utc = machine.UTC ?? 0,
                    type = ((enTypeAlarm)a.Type).GetDescription(),
                    //((enTypeAlarm)a.StateId).GetDescription(),
                    group = a.GroupName,
                    time = CommonViewService.getTimeViewModel(a.ElapsedTime),
                    description = a.Description

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
