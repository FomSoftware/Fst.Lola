using FomMonitoringCore.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FomMonitoringBLL.ViewModel
{
    public class SpindleViewModel
    {
        public SpindleVueModel vm_spindles { get; set; }
        public MotorAxesParameterVueModel vm_motoraxes_blitz { get; set; }
        public MachineInfoViewModel vm_machine_info { get; set; }
    }

    public class XSpindleViewModel
    {
        public SpindleVueModel vm_spindles { get; set; }
    }

    public class SpindleVueModel
    {
        public List<SpindleDataModel> spindles { get; set; }
        public SortingViewModel sorting { get; set; }
    }


    public class MotorAxesParameterVueModel
    {
        public List<ParameterMachineValueModel> motors { get; set; } = new List<ParameterMachineValueModel>();
        public List<ParameterMachineValueModel> axes { get; set; } = new List<ParameterMachineValueModel>();

    }

    public class SpindleDataModel
    {
        public string code { get; set; }
        public decimal? perc { get; set; }
        public decimal velocity { get; set; }
        public WorkOverModel workover { get; set; }
        public InfoModel info { get; set; }
        public TimeToLiveModel time { get; set; }
        public ChartViewModel opt_bands { get; set; }
    }

    public class WorkOverModel
    {
        public int? power { get; set; }
        public int? vibration { get; set; }
        public int? heating { get; set; }
    }

    public class InfoModel
    {
        public string install { get; set; }
        public string maintenance { get; set; }
        //public int change { get; set; }
    }

    public class TimeToLiveModel
    {
        public TimeViewModel work { get; set; }
        public TimeViewModel residual { get; set; }
    }
}