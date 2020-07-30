using System;
using System.Collections.Generic;
using System.Globalization;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewModel
{
    public class JobViewModel
    {
        public JobVueModel vm_jobs { get; set; }
    }

    public class JobVueModel
    {
        public List<JobDataModel> jobs { get; set; }
        public SortingViewModel sorting { get; set; }
        public CurrentStateModel currentState { get; set; }
    }

    public class JobDataModel
    {
        public string code { get; set; }
        public int perc { get; set; }
        public TimeViewModel time { get; set; }
        public int quantity { get; set; }
        public int pieces { get; set; }
        public DateTime? day { get; set; }

        public string formatted_day
        {
            get { return day?.ToString("d", CultureInfo.CurrentCulture); }
        }
    }
}