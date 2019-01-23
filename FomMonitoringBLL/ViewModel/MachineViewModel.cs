using FomMonitoringCore.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FomMonitoringBLL.ViewModel
{
    public class MachineViewModel
    {
        public DataUpdateModel LastUpdate { get; set; }

        public EfficiencyViewModel Efficiency { get; set; }

        public ProductivityViewModel Productivity { get; set; }

        public AlarmViewModel Alarms { get; set; }

        public SpindleViewModel Spindles { get; set; }

        public ToolViewModel Tools { get; set; }

        public JobViewModel Jobs { get; set; }
    }
}