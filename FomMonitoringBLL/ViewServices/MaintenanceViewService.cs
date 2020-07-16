using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class MaintenanceViewService : IMaintenanceViewService
    {
        private readonly IMessageService _messageService;
        private readonly IUserManagerViewService _userManagerViewService;

        public MaintenanceViewService(IMessageService messageService, IUserManagerViewService userManagerViewService)
        {
            _messageService = messageService;
            _userManagerViewService = userManagerViewService;
        }

        public MaintenanceViewModel GetMessages(ContextModel context)
        {
            var result = new MaintenanceViewModel();

            result.vm_messages = GetVueModel(context.ActualMachine, context.ActualPeriod);
            result.ignored_messages = GetIgnoredVueModel(context.ActualMachine, context.ActualPeriod);

            if (context.User.Role == enRole.Customer && context.ActualMachine.TimeZone != null)
            {
                result.timeZone = context.ActualMachine.TimeZone;
            }
            else if (context.User.TimeZone != null)
            {
               result.timeZone = context.User.TimeZone;
            }
            return result;
        }

        private MaintenceVueModel GetIgnoredVueModel(MachineInfoModel machine, PeriodModel period)
        {
            var result = new MaintenceVueModel();
            var data = _messageService.GetOldMaintenanceMessages(machine, period);

            if (data.Count == 0)
                return result;

            var messages = data.Select(a =>
                new ManteinanceDataModel
                {
                    id = a.Id,
                    code = a.Code,
                    type = ((enTypeAlarm)a.Type).GetDescription(),
                    time = CommonViewService.getTimeViewModel(a.ElapsedTime),
                    timestamp = DateTime.SpecifyKind(a.IgnoreDate ?? DateTime.MinValue, DateTimeKind.Utc),
                    utc = machine.UTC,
                    expiredSpan = CommonViewService.getTimeViewModel(_messageService.GetExpiredSpan(a)),
                    description = a.Description,
                    user = _userManagerViewService.GetUser(a.UserId)

                }).ToList();

            result.messages = messages.OrderByDescending(o => o.time.elapsed).ToList();
            var sorting = new SortingViewModel();
            sorting.duration = enSorting.Descending.GetDescription();
            sorting.user = enSorting.Descending.GetDescription();
            sorting.timestamp = enSorting.Descending.GetDescription();
            
            result.sorting = sorting;
            return result;
        }

    private MaintenceVueModel GetVueModel(MachineInfoModel machine, PeriodModel period)
        {
            var result = new MaintenceVueModel();

            var data = _messageService.GetMaintenanceMessages(machine, period);

            if (data.Count == 0)
                return result;
            
            var messages = data.Select(a =>
            new ManteinanceDataModel()
            {
                id = a.Id,
                code = a.Code,
                type = ((enTypeAlarm)a.Type).GetDescription(),
                time = CommonViewService.getTimeViewModel(a.ElapsedTime),
                timestamp = DateTime.SpecifyKind(a.Day ?? DateTime.MinValue, DateTimeKind.Utc),
                utc = machine.UTC,
                expiredSpan = CommonViewService.getTimeViewModel(_messageService.GetExpiredSpan(a)),
                description = a.Description

            }).ToList();

            messages = messages.OrderByDescending(o => o.time.elapsed).ToList();

            var sorting = new SortingViewModel();
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
