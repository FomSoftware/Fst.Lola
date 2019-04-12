using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FomMonitoringBLL.ViewModel
{
    public class EfficiencyViewModel
    {
        public EfficiencyVueModel vm_efficiency { get; set; }

        public ChartViewModel opt_historical { get; set; }

        public ChartViewModel opt_kpis { get; set; }

        public ChartViewModel opt_operators { get; set; }

        public ChartViewModel opt_shifts { get; set; }

        public ChartViewModel opt_states { get; set; }

        public MachineInfoViewModel vm_machine_info { get; set; }
    }

    public class EfficiencyVueModel
    {
        public KPIViewModel kpi { get; set; }
        public TotalTimeModel total { get; set; }
        public KPIViewModel overfeed { get; set; }
        public List<StateViewModel> states { get; set; }
    }

    public class StateViewModel
    {
        public string code { get; set; }
        public string text { get; set; }
        public decimal? perc { get; set; }
        public TimeViewModel time { get; set; }
        public bool active { get; set; }
    }

    public class TotalTimeModel
    {
        public TimeViewModel on { get; set; }
        public TimeViewModel off { get; set; }
    }
}