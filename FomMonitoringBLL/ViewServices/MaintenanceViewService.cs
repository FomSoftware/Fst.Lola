using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewServices
{
    public class MaintenanceViewService : IMaintenanceViewService
    {
        private IReadMessages _readMessages;
        private IMessageService _messageService;

        public MaintenanceViewService(IReadMessages readMessages, IMessageService messageService)
        {
            _readMessages = readMessages;
            _messageService = messageService;
        }

        public MaintenanceViewModel GetMessages(ContextModel context)
        {
            MaintenanceViewModel result = new MaintenanceViewModel();

            result.vm_messages = GetVueModel(context.ActualMachine, context.ActualPeriod);
            return result;
        }


        private MaintenceVueModel GetVueModel(MachineInfoModel machine, PeriodModel period)
        {
            MaintenceVueModel result = new MaintenceVueModel();

            List<MessageMachineModel> data = _messageService.GetMaintenanceMessages(machine, period);

            if (data.Count == 0)
                return result;

            List<ManteinanceDataModel> messages = data.Select(a =>
            new ManteinanceDataModel()
            {
                id = a.Id,
                code = a.Code,
                type = ((enTypeAlarm)_readMessages.GetMessageType(a.Code, machine.Id)).GetDescription(),
                time = CommonViewService.getTimeViewModel(a.ElapsedTime),
                timestamp = DateTime.SpecifyKind(a.Day.HasValue ? a.Day.Value : DateTime.MinValue, DateTimeKind.Utc),
                utc = machine.UTC,
                expiredSpan = CommonViewService.getTimeViewModel(_messageService.GetExpiredSpan(a)),
                description = a.Description
            }).ToList();

            messages = messages.OrderByDescending(o => o.time.elapsed).ToList();

            SortingViewModel sorting = new SortingViewModel();
            sorting.duration = enSorting.Descending.GetDescription();

            result.messages = messages;
            result.sorting = sorting;


            return result;
        }

        public bool IgnoreMessage(int MessageId)
        {
            return _messageService.IgnoreMessage(MessageId);
        }


    }
}
