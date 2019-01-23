using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewServices
{
    public class AlarmsViewService
    {
        public static AlarmViewModel GetAlarms(ContextModel context)
        {
            AlarmViewModel result = new AlarmViewModel();

            result.vm_alarms = GetVueModel(context.ActualMachine, context.ActualPeriod);
            result.opt_historical = GetHistoricalOptions(context.ActualMachine, context.ActualPeriod);

            return result;
        }


        private static AlarmVueModel GetVueModel(MachineInfoModel machine, PeriodModel period)
        {
            AlarmVueModel result = new AlarmVueModel();

            List<HistoryAlarmModel> data = AlarmService.GetAggregationAlarms(machine, period, enDataType.Dashboard);

            if (data.Count == 0)
                return result;

            List<AlarmDataModel> alarms = data.Select(a => new AlarmDataModel()
            {
                code = a.Code,
                type = ((enTypeAlarm)a.StateId).GetDescription(),
                description = a.Description,
                time = CommonViewService.getTimeViewModel(a.ElapsedTime),
                quantity = a.Count == null ? 0 : a.Count.Value
            }).ToList();

            alarms = alarms.OrderByDescending(o => o.time.elapsed).ToList();

            SortingViewModel sorting = new SortingViewModel();
            sorting.duration = enSorting.Descending.GetDescription();

            result.alarms = alarms;
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

            List<HistoryAlarmModel> data = AlarmService.GetAggregationAlarms(machine, periodTrend, enDataType.Historical).OrderBy(o => o.Day).ToList();

            if (data.Count == 0)
                return null;
           
            options.yTitle = string.Format("{0} (n)", Resource.Quantity);

            List<DateTime> days = data.Where(w => w.Day != null).Select(s => s.Day.Value).Distinct().ToList();
            options.categories = CommonViewService.GetTimeCategories(days, granularity);

            List<SerieViewModel> series = new List<SerieViewModel>();

            SerieViewModel serieOperator = new SerieViewModel();
            serieOperator.name = Resource.Operators;
            serieOperator.color = CommonViewService.GetColorAlarm(enState.Pause);
            serieOperator.data = data.Where(w => w.enState == enState.Pause).Select(s => s.Count ?? 0).ToList();
            series.Add(serieOperator);

            SerieViewModel serieError = new SerieViewModel();
            serieError.name = enState.Error.ToLocalizedString();
            serieError.color = CommonViewService.GetColorAlarm(enState.Error);
            serieError.data = data.Where(w => w.enState == enState.Error).Select(s => s.Count ?? 0).ToList();
            series.Add(serieError);

            options.series = series;

            return options;
        }

    }
}
