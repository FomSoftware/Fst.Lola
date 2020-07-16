using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringResources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class MessagesViewService : IMessagesViewService
    {
        private readonly IMessageService _messageService;
        private readonly IContextService _contextService;

        public MessagesViewService(IMessageService messageService, IContextService contextService)
        {
            _messageService = messageService;
            _contextService = contextService;
        }

        public MessageViewModel GetMessages(ContextModel context)
        {
            var result = new MessageViewModel();
            result.vm_messages = GetVueModel(context.ActualMachine, context.ActualPeriod, context.ActualMachineGroup);
            result.opt_historical = GetHistoricalOptions(context.ActualMachine, context.ActualPeriod, context.ActualMachineGroup);
            result.vm_details = GetMessageDetails(context.ActualMachine, context.ActualPeriod, context.ActualMachineGroup);

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

        private MessageDetailsVueModel GetMessageDetails(MachineInfoModel actualMachine, PeriodModel actualPeriod, string actualMachineGroup = null)
        {
            var result = new MessageDetailsVueModel();

            var data = _messageService.GetMessageDetails(actualMachine, actualPeriod, actualMachineGroup);

            if (data.Count == 0)
                return result;

            var messages = data.Select(a => new MessageDetailViewModel()
            {
                code = a.Code,
                parameters = a.Params,
                timestamp = DateTime.SpecifyKind(a.Day ?? DateTime.MinValue, DateTimeKind.Utc),
                utc = actualMachine.UTC ?? 0,
                type = ((enTypeAlarm)a.Type).GetDescription(),
                group = a.GroupName,
                time = CommonViewService.getTimeViewModel(a.ElapsedTime),
                description = a.Description
            }).ToList();

            messages = messages.OrderByDescending(o => o.timestamp).ToList();

            var sorting = new SortingViewModel();
            sorting.timestamp = enSorting.Descending.GetDescription();

            sorting.group = enSorting.Ascending.GetDescription();
            sorting.user = enSorting.Ascending.GetDescription();

            result.messages = messages;
            result.sorting = sorting;
            


            return result;
        }


        private MessageVueModel GetVueModel(MachineInfoModel machine, PeriodModel period, string actualMachineGroup = null)
        {
            var result = new MessageVueModel();

            var data = _messageService.GetAggregationMessages(machine, period, enDataType.Summary, actualMachineGroup);

            if (data.Count == 0)
                return result;

            var messages = data.Select(a => new MessageDataModel()
            {
                code = a.Code,
                type = a.Type != null ? ((enTypeAlarm)a.Type).GetDescription() : "",
                parameters = a.Params,
                quantity = a.Count ?? 0,
                day = a.Day == null ? "-" : a.Day.Value.ToString("t"),
                description = a.Description
            }).ToList();

            messages = messages.OrderByDescending(o => o.quantity).ToList();

            var sorting = new SortingViewModel
            {
                duration = enSorting.Descending.GetDescription()
            };

            result.messages = messages;
            result.sorting = sorting;

            return result;
        }


        private ChartViewModel GetHistoricalOptions(MachineInfoModel machine, PeriodModel period, string actualMachineGroup = null)
        {
            var options = new ChartViewModel();

            var granularity = Common.GetAggregationType(period.StartDate, period.EndDate);
            var startDateTrend = Common.GetStartDateTrend(period.EndDate, period.StartDate, granularity);

            var periodTrend = new PeriodModel();
            
            periodTrend.StartDate = startDateTrend.ToUniversalTime().Date;
            periodTrend.EndDate = period.EndDate.ToUniversalTime().Date.AddDays(1).AddTicks(-1);
            periodTrend.Aggregation = granularity;


            var data = _messageService.GetAggregationMessages(machine, periodTrend, enDataType.Historical, actualMachineGroup)?.OrderBy(o => o.Day).ToList() ?? new List<HistoryMessageModel>();

            if (data.Count == 0)
                return null;
           
            options.yTitle = $"{Resource.Quantity} (n)";

            var days = data.Where(w => w.Day != null && (w.Type == (int)enTypeAlarm.Warning || w.Type == (int)enTypeAlarm.Error)).Select(s => s.Day.Value).Distinct().ToList();
            options.categories = CommonViewService.GetTimeCategories(days, granularity);

            var series = new List<SerieViewModel>();

            var serieOperator = new SerieViewModel();
            serieOperator.name = enTypeAlarm.Warning.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieOperator.color = CommonViewService.GetColorAlarm(enTypeAlarm.Warning);
            
            var serieError = new SerieViewModel();
            serieError.name = enTypeAlarm.Error.ToLocalizedString(_contextService.GetContext().ActualLanguage.InitialsLanguage);
            serieError.color = CommonViewService.GetColorAlarm(enTypeAlarm.Error);
            
            serieOperator.data = new List<int>();
            serieError.data = new List<int>();
            foreach (string cat in options.categories)
            {
                var catVal = data.Where(w => w.Type == (int)enTypeAlarm.Warning && 
                                CommonViewService.GetTimeCategory((DateTime)w.Day, granularity) == cat).Select(s => s.Count ?? 0).FirstOrDefault();
                serieOperator.data.Add(catVal);
                var catVal2 = data.Where(w => w.Type == (int)enTypeAlarm.Error &&
                                             CommonViewService.GetTimeCategory((DateTime)w.Day, granularity) == cat).Select(s => s.Count ?? 0).FirstOrDefault();
                serieError.data.Add(catVal2);
            }

            series.Add(serieOperator);
            series.Add(serieError);
            options.series = series;

            return options;
        }

    }
}
