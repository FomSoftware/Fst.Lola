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
    public class SpindlesViewService
    {
        public static SpindleViewModel GetSpindles(ContextModel context)
        {
            SpindleViewModel result = new SpindleViewModel();

            if (MachineService.GetMachinePanels(context).Contains((int)enPanel.BlitzMotorAxes))
            {
                result.vm_motoraxes_blitz = GetVueModelBlitz(context.ActualMachine, true);
            }
            else
            {
                if (MachineService.GetMachinePanels(context).Contains((int)enPanel.KeopeMotors))
                {
                    result.vm_motor_keope = GetVueModelKeope(context.ActualMachine);
                }
                else
                {
                    result.vm_spindles = GetVueModel(context.ActualMachine);
                }

            }
            
            result.vm_machine_info = new MachineInfoViewModel
            {
                model = context.ActualMachine.Model.Name,
                mtype = context.ActualMachine.Type.Name,
                id_mtype = context.ActualMachine.Type.Id,
                machineName = context.ActualMachine.MachineName
            };

            return result;
        }

        private static MotorKeopeParameterVueModel GetVueModelKeope(MachineInfoModel machine, bool xmodule = false)
        {

            var par = ParameterMachineService.GetParameters(machine, (int)enPanel.KeopeMotors);

            var result = new MotorKeopeParameterVueModel
            {
                fixedHead = par.Where(p => p.VarNumber == 428 || p.VarNumber == 432).OrderBy(n => n.VarNumber).ToList(),

                mobileHead = par.Where(p => p.VarNumber == 430 || p.VarNumber == 434).OrderBy(n => n.VarNumber).ToList(),
            };

            foreach (var mot in result.fixedHead)
            {
                mot.Value = double.IsNaN(double.Parse(mot.Value)) ? "" : double.Parse(mot.Value).ToString("0.000");
            }

            foreach (var ax in result.mobileHead)
            {
                ax.Value = double.IsNaN(double.Parse(ax.Value)) ? "" : double.Parse(ax.Value).ToString("0.000");
            }


            return result;
        }

        private static MotorAxesParameterVueModel GetVueModelBlitz(MachineInfoModel machine, bool xmodule = false)
        {

            var par = ParameterMachineService.GetParameters(machine, (int)enPanel.BlitzMotorAxes);

            var result = new MotorAxesParameterVueModel
            {
                motors = par.Where(p => p.VarNumber == 428 || p.VarNumber == 430 || p.VarNumber == 432 || p.VarNumber == 434).OrderBy(n => n.VarNumber).ToList(),

                axes = par.Where(p => p.VarNumber == 450 || p.VarNumber == 452 || p.VarNumber == 454).OrderBy(n => n.VarNumber).ToList(),
            };

            foreach(var mot in result.motors)
            {
                mot.Value = double.IsNaN(double.Parse(mot.Value)) ? "" : double.Parse(mot.Value).ToString("0.000");
            }

            foreach (var ax in result.axes)
            {
                ax.Value = double.IsNaN(double.Parse(ax.Value)) ? "" : double.Parse(ax.Value).ToString("0.000");
            }


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
                    //change = s.ChangeCount
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
