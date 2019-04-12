﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FomMonitoringBLL.ViewModel
{
    public class AlarmViewModel
    {
        public AlarmVueModel vm_alarms { get; set; }

        public AlarmDetailsVueModel vm_details { get; set; }

    public ChartViewModel opt_historical { get; set; }
    }

    public class AlarmVueModel
    {
        public List<AlarmDataModel> alarms { get; set; }
        public SortingViewModel sorting { get; set; }
    }

    public class AlarmDataModel
    {
        public string code { get; set; }
        public string type { get; set; }
        public TimeViewModel time { get; set; }
        public int quantity { get; set; }
        public string description { get; set; }

        public string day { get; set; }
    }

    public class AlarmDetailsVueModel
    {
        public List<AlarmDetailViewModel> alarms { get; set; }
        public SortingViewModel sorting { get; set; }

    }

}