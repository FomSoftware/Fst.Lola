using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringResources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class MessagesViewService
    {
        public static MessageViewModel GetMessages(ContextModel context)
        {
            MessageViewModel result = new MessageViewModel();

            result.vm_messages = GetVueModel(context.ActualMachine, context.ActualPeriod);
            result.opt_historical = GetHistoricalOptions(context.ActualMachine, context.ActualPeriod);
            result.vm_details = GetMessageDetails(context.ActualMachine, context.ActualPeriod);
            return result;
        }

        private static MessageDetailsVueModel GetMessageDetails(MachineInfoModel actualMachine, PeriodModel actualPeriod)
        {
            MessageDetailsVueModel result = new MessageDetailsVueModel();

            List<MessageMachineModel> data = MessageService.GetMessageDetails(actualMachine, actualPeriod);

            if (data.Count == 0)
                return result;

            List<MessageDetailViewModel> messages = data.Select(a => new MessageDetailViewModel()
            {
                code = a.Code,
                parameters = a.Params,
                timestamp = a.Day,
                type = ((enTypeAlarm)ReadMessages.GetMessageType(a.Code, actualMachine.Id)).GetDescription(),
                //((enTypeAlarm)a.StateId).GetDescription(),
                group = ReadMessages.GetMessageGroup(a.Code, actualMachine.Id, a.Group),
                description = ReadMessages.GetMessageDescription(a.Code, actualMachine.Id, a.Params, CultureInfo.CurrentCulture.Name)
            }).ToList();

            messages = messages.OrderByDescending(o => o.timestamp).ToList();

            SortingViewModel sorting = new SortingViewModel();
            sorting.timestamp = enSorting.Descending.GetDescription();

            sorting.group = enSorting.Ascending.GetDescription();

            result.messages = messages;
            result.sorting = sorting;


            return result;
        }


        private static MessageVueModel GetVueModel(MachineInfoModel machine, PeriodModel period)
        {
            MessageVueModel result = new MessageVueModel();

            List<HistoryMessageModel> data = MessageService.GetAggregationMessages(machine, period, enDataType.Summary);

            if (data.Count == 0)
                return result;

            List<MessageDataModel> messages = data.Select(a => new MessageDataModel()
            {
                code = a.Code,
                type = ReadMessages.GetMessageType(a.Code, machine.Id) != null ? ((enTypeAlarm)ReadMessages.GetMessageType(a.Code, machine.Id)).GetDescription() : String.Empty,
                //((enTypeAlarm)a.StateId).GetDescription(),
                parameters = a.Params,
                time = CommonViewService.getTimeViewModel(a.ElapsedTime),
                quantity = a.Count == null ? 0 : a.Count.Value,
                day = a.Day == null ? "-" : a.Day.Value.ToString("t")
                //description = (a.Code != null) ? ReadMessages.GetMessageDescription(a.Code, machine.Id, a.Params, CultureInfo.CurrentCulture.Name) : ""
            }).ToList();

            messages = messages.OrderByDescending(o => o.quantity).ToList();

            SortingViewModel sorting = new SortingViewModel();
            sorting.duration = enSorting.Descending.GetDescription();

            result.messages = messages;
            result.sorting = sorting;
            

            return result;
        }


        private static ChartViewModel GetHistoricalOptions(MachineInfoModel machine, PeriodModel period)
        {
            ChartViewModel options = new ChartViewModel();

            enAggregation granularity = Common.GetAggregationType(period.StartDate, period.EndDate);
            DateTime startDateTrend = Common.GetStartDateTrend(period.EndDate, granularity);

            PeriodModel periodTrend = new PeriodModel();
            periodTrend.StartDate = startDateTrend;
            periodTrend.EndDate = period.EndDate;
            periodTrend.Aggregation = granularity;

            List<HistoryMessageModel> data = MessageService.GetAggregationMessages(machine, periodTrend, enDataType.Historical)?.OrderBy(o => o.Day).ToList() ?? new List<HistoryMessageModel>();

            if (data.Count == 0)
                return null;
           
            options.yTitle = string.Format("{0} (n)", Resource.Quantity);

            List<DateTime> days = data.Where(w => w.Day != null).Select(s => s.Day.Value).Distinct().ToList();
            options.categories = CommonViewService.GetTimeCategories(days, granularity);

            List<SerieViewModel> series = new List<SerieViewModel>();

            SerieViewModel serieOperator = new SerieViewModel();
            serieOperator.name = enTypeAlarm.Warning.ToLocalizedString();
            serieOperator.color = CommonViewService.GetColorAlarm(enTypeAlarm.Warning);
            serieOperator.data = data.Where(w => w.Type == (int)enTypeAlarm.Warning).Select(s => s.Count ?? 0).ToList();
            series.Add(serieOperator);

            SerieViewModel serieError = new SerieViewModel();
            serieError.name = enTypeAlarm.Error.ToLocalizedString();
            serieError.color = CommonViewService.GetColorAlarm(enTypeAlarm.Error);
            serieError.data = data.Where(w => w.Type == (int)enTypeAlarm.Error).Select(s => s.Count ?? 0).ToList();
            series.Add(serieError);

            options.series = series;

            return options;
        }

    }
}
