using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FomMonitoringBLL.ViewModel
{
    public class ToolViewModel
    {
        public ToolVueModel vm_tools { get; set; }
    }

    public class XToolViewModel
    {
        public ToolVueModel vm_tools { get; set; }
    }

    public class ToolVueModel
    {
        public List<ToolDataModel> tools { get; set; }
        public SortingViewModel sorting { get; set; }
    }

    public class ToolDataModel
    {
        public string code { get; set; }
        public string description { get; set; }
        public decimal? perc { get; set; }
        public ChangeModel changes { get; set; }
        public TimeViewModel time { get; set; }
    }

    public class ChangeModel
    {
        public int total { get; set; }
        public int breaking { get; set; }
        public int replacement { get; set; }
        public List<HistoricalModel> historical { get; set; }
    }

    public class HistoricalModel
    {
        public string date { get; set; }
        public string type { get; set; }
        public string color_type { get; set; }
        public TimeViewModel duration { get; set; }
    }
}