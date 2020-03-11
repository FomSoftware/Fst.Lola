using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewModel
{
    public class MaintenanceViewModel
    {
        public MaintenceVueModel vm_messages { get; set; }

        public MaintenceVueModel ignored_messages { get; set; }

    }

    public class MaintenceVueModel
    {
        public List<ManteinanceDataModel> messages { get; set; }
        public SortingViewModel sorting { get; set; }

    }

    public class ManteinanceDataModel
    {
        public int id { get; set; }
        public string machineName { get; set; }
        public string machineModel { get; set; }
        public string machineSerial { get; set; }
        public string code { get; set; }
        public string type { get; set; }
        public TimeViewModel time { get; set; }
        public string panelName { get; set; }
        public DateTime? timestamp { get; set; }
        public TimeViewModel expiredSpan { get; set; }
        public string description { get; set; }
        public string formatted_timestamp
        {
            get { return timestamp?.ToString(CultureInfo.CurrentCulture); }
        }
        public double? utc { get; set; }
        public UserManagerViewModel user { get; set; }
    }

}
