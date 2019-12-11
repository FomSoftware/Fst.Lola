using System.Collections.Generic;
using System.Linq;
using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringResources;

namespace FomMonitoringBLL.ViewServices
{
    public class SpindleViewService : ISpindleViewService
    {
        private ISpindleService _spindleService;

        public SpindleViewService(ISpindleService spindleService)
        {
            _spindleService = spindleService;
        }

        public SpindleVueModel GetSpindles(ContextModel context)
        {
            return GetVueModel(context.ActualMachine, true);
        }

        public XSpindleViewModel GetXSpindles(ContextModel context)
        {
            var result = new XSpindleViewModel();
            result.vm_spindles = GetVueModel(context.ActualMachine, true);

            return result;
        }

        private SpindleVueModel GetVueModel(MachineInfoModel machine, bool xmodule = false)
        {
            var result = new SpindleVueModel();

            var data = _spindleService.GetSpindles(machine, xmodule);

            if (data.Count == 0)
                return result;

            var spindles = data.Select(s => new SpindleDataModel
            {
                code = s.Code,
                perc = Common.GetPercentage(s.ElapsedTimeWorkTotal, s.ExpectedWorkTime),
                velocity = s.AverageElapsedTimeWork.MathRound(0),
                workover = new WorkOverModel
                {
                    power = s.WorkOverPowerCount,
                    vibration = s.WorkOverVibratingCount,
                    heating = s.WorkOverheatingCount
                },
                info = new InfoModel
                {
                    install = s.InstallDate == null ? "-" : s.InstallDate.Value.ToString("d"),
                    maintenance = machine.NextMaintenanceService == null ? "-" : machine.NextMaintenanceService.Value.ToString("d")
                    //change = s.ChangeCount
                },
                time = new TimeToLiveModel
                {
                    work = CommonViewService.getTimeViewModel(s.ElapsedTimeWorkTotal),
                    residual = CommonViewService.getTimeViewModel(s.ExpectedWorkTime ?? 0 - s.ElapsedTimeWorkTotal ?? 0)
                },
                opt_bands = GetBandsOptions(s)
            }).ToList();

            spindles = spindles.OrderByDescending(o => o.perc).ToList();

            var sorting = new SortingViewModel();
            sorting.time = enSorting.Descending.GetDescription();

            result.spindles = spindles;
            result.sorting = sorting;

            return result;
        }


        private ChartViewModel GetBandsOptions(SpindleModel spindle)
        {
            var options = new ChartViewModel();

            options.categories = new List<string> { "3K", "6K", "9K", "12K", "15K", "18K" };
            options.xTitle = $"{Resource.SpeedRanges} (K)";

            // calcolo dell'unità di misura delle serie del grafico sul valore medio 
            var avgData = (spindle.ElapsedTimeWork3K ?? 0 + spindle.ElapsedTimeWork6K ?? 0 + spindle.ElapsedTimeWork9K ?? 0
                           + spindle.ElapsedTimeWork12K ?? 0 + spindle.ElapsedTimeWork15K ?? 0 + spindle.ElapsedTimeWork18K ?? 0) / 6;
            var measurement = Common.GetTimeMeasurement(avgData);

            options.yTitle = $"{Resource.Duration} ({measurement.GetDescription()})";
            options.valueSuffix = $" {measurement.GetDescription()}";

            var elapsedBands = new List<long>
            {
                spindle.ElapsedTimeWork3K ?? 0,
                spindle.ElapsedTimeWork6K ?? 0,
                spindle.ElapsedTimeWork9K ?? 0,
                spindle.ElapsedTimeWork12K ?? 0,
                spindle.ElapsedTimeWork15K ?? 0,
                spindle.ElapsedTimeWork18K ?? 0
            };

            var series = new List<SerieViewModel>();

            var serieBands = new SerieViewModel();
            serieBands.name = Resource.UsageTime;
            serieBands.data = Common.ConvertElapsedByMeasurement(elapsedBands, measurement);
            series.Add(serieBands);

            options.series = series;

            return options;
        }

    }
}
