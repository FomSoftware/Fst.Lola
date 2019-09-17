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

        public MessageViewModel Messages { get; set; }

        public PanelParametersViewModel PanelParameter { get; set; }

        public ToolViewModel Tools { get; set; }

        public JobViewModel Jobs { get; set; }

        public XSpindleViewModel XSpindles { get; set; }

        public XToolViewModel XTools { get; set; }

        public MachineInfoViewModel MachineInfo { get; set; }

        public MaintenanceViewModel Maintenance { get; set; }

        public List<int> MachinePanels { get; set; }
    }
}