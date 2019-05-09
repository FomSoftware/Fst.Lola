using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringResources;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class SpindlesViewService
    {
        public static SpindleViewModel GetSpindles(ContextModel context)
        {
            SpindleViewModel result = new SpindleViewModel();
            result.vm_spindles = GetVueModel(context.ActualMachine);

            return result;
        }

        public static XSpindleViewModel GetXSpindles(ContextModel context)
        {
            XSpindleViewModel result = new XSpindleViewModel();
            result.vm_spindles = GetVueModel(context.ActualMachine, true);

            return result;
        }

        private static SpindleVueModel GetVueModel(MachineInfoModel machine, bool xmodule = false)
        {
            SpindleVueModel result = new SpindleVueModel();

            List<SpindleModel> data = SpindleService.GetSpindles(machine, xmodule);

            if (data.Count == 0)
                return result;

            List<SpindleDataModel> spindles = data.Select(s => new SpindleDataModel()
            {
                code = s.Code,
                perc = Common.GetPercentage(s.ElapsedTimeWorkTotal, s.ExpectedWorkTime),
                velocity = s.AverageElapsedTimeWork.MathRound(0),
                workover = new WorkOverModel()
                {
                    power = s.WorkOverPowerCount,
                    vibration = s.WorkOverVibratingCount,
                    heating = s.WorkOverheatingCount
                },
                info = new InfoModel()
                {
                    install = s.InstallDate == null ? "-" : s.InstallDate.Value.ToString("d"),
                    maintenance = machine.NextMaintenanceService == null ? "-" : machine.NextMaintenanceService.Value.ToString("d"),
                    change = s.ChangeCount
                },
                time = new TimeToLiveModel()
                {
                    work = CommonViewService.getTimeViewModel(s.ElapsedTimeWorkTotal),
                    residual = CommonViewService.getTimeViewModel(s.ExpectedWorkTime ?? 0 - s.ElapsedTimeWorkTotal ?? 0),
                },
                opt_bands = GetBandsOptions(s)
            }).ToList();

            spindles = spindles.OrderByDescending(o => o.perc).ToList();

            SortingViewModel sorting = new SortingViewModel();
            sorting.time = enSorting.Descending.GetDescription();

            result.spindles = spindles;
            result.sorting = sorting;

            return result;
        }


        private static ChartViewModel GetBandsOptions(SpindleModel spindle)
        {
            ChartViewModel options = new ChartViewModel();

            options.categories = new List<string>() { "3K", "6K", "9K", "12K", "15K", "18K" };
            options.xTitle = string.Format("{0} (K)", Resource.SpeedRanges);

            // calcolo dell'unità di misura delle serie del grafico sul valore medio 
            long avgData = (long)(spindle.ElapsedTimeWork3K ?? 0 + spindle.ElapsedTimeWork6K ?? 0 + spindle.ElapsedTimeWork9K ?? 0
                                    + spindle.ElapsedTimeWork12K ?? 0 + spindle.ElapsedTimeWork15K ?? 0 + spindle.ElapsedTimeWork18K ?? 0) / 6;
            enMeasurement measurement = Common.GetTimeMeasurement(avgData);

            options.yTitle = string.Format("{0} ({1})", Resource.Duration, measurement.GetDescription());
            options.valueSuffix = string.Format(" {0}", measurement.GetDescription());

            List<long> elapsedBands = new List<long>()
            {
                spindle.ElapsedTimeWork3K ?? 0,
                spindle.ElapsedTimeWork6K ?? 0,
                spindle.ElapsedTimeWork9K ?? 0,
                spindle.ElapsedTimeWork12K ?? 0,
                spindle.ElapsedTimeWork15K ?? 0,
                spindle.ElapsedTimeWork18K ?? 0
            };

            List<SerieViewModel> series = new List<SerieViewModel>();

            SerieViewModel serieBands = new SerieViewModel();
            serieBands.name = Resource.UsageTime;
            serieBands.data = Common.ConvertElapsedByMeasurement(elapsedBands, measurement);
            series.Add(serieBands);

            options.series = series;

            return options;
        }
    }
}
