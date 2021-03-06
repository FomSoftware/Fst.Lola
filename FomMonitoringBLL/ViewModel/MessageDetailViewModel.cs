﻿using System;
using System.Globalization;

namespace FomMonitoringBLL.ViewModel
{
    public class MessageDetailViewModel
    {

        public string code { get; set; }
        public string parameters { get; set; }
        public DateTime? timestamp { get; set; }
        public string type { get; set; }
        public double utc { get; set; }
        public string group { get; set; }
        public string description { get; set; }

        public TimeViewModel time { get; set; }
        
        public string formatted_timestamp
        {
            get { return timestamp?.ToString(CultureInfo.CurrentCulture); }
        }

    }
}
