using FomMonitoringCore.Framework.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewModel
{
    public class AlarmDetailViewModel
    {

        public string code { get; set; }
        public string description { get; set; }
        public DateTime? timestamp { get; set; }
        public string type { get; set; }

        public string formatted_timestamp {
            get { return timestamp?.ToString(CultureInfo.CurrentCulture); }
        }

    }
}
