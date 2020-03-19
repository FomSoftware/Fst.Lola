using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FomMonitoringBLL.ViewModel
{
    public class KPIViewModel
    {
        public decimal? value { get; set; }
        public ThresholdViewModel threshold { get; set; }
    }

    public class ThresholdViewModel
    {
        public double? green { get; set; }
        public double? yellow { get; set; }
    }

    public class TimeViewModel
    {
        public string days { get; set; }
        public string hours { get; set; }
        public string minutes { get; set; }
        public string seconds { get; set; }
        public long elapsed { get; set; }
    }

    public class SortingViewModel
    {
        public string code { get; set; }
        public string time { get; set; }
        public string progress { get; set; }
        public string duration { get; set; }
        public string quantity { get; set; }
        public string timestamp { get; set; }
        public string group { get; set; }
        public string user { get; set; }
    }

    public class ChartViewModel
    {
        public string xTitle { get; set; }

        public string yTitle { get; set; }

        public string yTitle2 { get; set; }

        public string valueSuffix { get; set; }

        public List<string> categories { get; set; }

        public List<SerieViewModel> series { get; set; }
    }

    public class SerieViewModel
    {
        public int type { get; set; }

        public string name { get; set; }

        public string color { get; set; }
        public decimal y { get; set; }

        public List<int> data { get; set; }
        public double stateProductivityGreenThreshold { get; set; }
        public double stateProductivityYellowThreshold { get; set; }
    }
}