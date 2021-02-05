using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

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
            var data = _messageService.GetOldMaintenanceMessages(context.ActualMachine, context.ActualPeriod);
            if (data.Count == 0)
            {
                result.ignored_messages = new MaintenceVueModel();
                result.kpi_messages = new MaintenceVueModel();
            }
            else
            {
                result.ignored_messages = GetIgnoredVueModel(data, context.ActualMachine);
                result.kpi_messages = GetKpiVueModel(data, context.ActualMachine);
            }

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

        private MaintenceVueModel GetKpiVueModel(List<MessageMachineModel> data, MachineInfoModel machine)
        {
            var result = new MaintenceVueModel();
            var messages = data.Select(a =>
                new ManteinanceDataModel
                {
                    id = a.Id,
                    day = (DateTime)a.Day?.Date,
                    ignoreDate = (DateTime)a.IgnoreDate?.Date,
                    dateDiff = ((TimeSpan)(a.IgnoreDate?.Date - a.Day?.Date)).TotalDays,
                    utc = machine.UTC,
                    description = a.Description,
                    user = _userManagerViewService.GetUser(a.UserId)

                }).ToList();

            result.messages = messages.OrderByDescending(o => o.day).ToList();
            var sorting = new SortingViewModel();
            sorting.user = enSorting.Descending.GetDescription();
            sorting.dateDiff = enSorting.Descending.GetDescription();
            result.sorting = sorting;
            return result;
        }

        private MaintenceVueModel GetIgnoredVueModel(List<MessageMachineModel> data, MachineInfoModel machine)
        {
            var result = new MaintenceVueModel();
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
                description = a.Description,
                icon = GetIcon(a) 

            }).ToList();

            messages = messages.OrderByDescending(o => o.time.elapsed).ToList();

            var sorting = new SortingViewModel();
            sorting.duration = enSorting.Descending.GetDescription();

            result.messages = messages;
            result.sorting = sorting;


            return result;
        }

    private string GetIcon(MessageMachineModel m)
    {
        if (m.Type == 14 && m.PeriodicSpan != null && m.IsPeriodicMsg == true)
        {
            return "periodica";
        }
        else if (m.Type == 14 && m.PeriodicSpan == null && m.IsPeriodicMsg == true)
        {
            return "ordinaria";
        }
        else if (m.Type != 14 && m.PeriodicSpan == null && m.IsPeriodicMsg == true)
        {
            return "predittiva";
        }

        return "";
    }

        public bool IgnoreMessage(int messageId)
        {
            return _messageService.IgnoreMessage(messageId);
        }


    }
}
