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

        //valori presi dal current state
        public long? ResidueWorkingTimeJob { get; set; }
        public virtual string ResidueWorkingTimeJobMin => ResTimeMin(this.ResidueWorkingTimeJob);
        public virtual string ResidueWorkingTimeJobSec => ResTimeSec(this.ResidueWorkingTimeJob);

        public string ResTimeMin(long? _ResidueWorkingTime)
        {
            string result = "0";
            if (_ResidueWorkingTime != null)
            {
                //ResidueWorkingTime arriva in secondi e non in ticks
                TimeSpan interval = new TimeSpan((long)_ResidueWorkingTime * 10000000);
                int res = interval.Days * 24 * 60 +
                          interval.Hours * 60 +
                          interval.Minutes;
                result = res.ToString("D");
            }

            return result;
        }

        public string ResTimeSec(long? _ResidueWorkingTime)
        {
            string result = "0";
            if (_ResidueWorkingTime != null)
            {
                TimeSpan interval = new TimeSpan((long)_ResidueWorkingTime * 10000000);
                int res = interval.Seconds;
                result = res.ToString("D");
            }
            return result;
        }
        public string formatted_day
        {
            get { return day?.ToString("d", CultureInfo.CurrentCulture); }
        }
    }
}