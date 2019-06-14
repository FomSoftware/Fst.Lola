using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewModel
{
    public class MaintenanceViewModel
    {
        public MaintenceVueModel vm_messages { get; set; }

    }

    public class MaintenceVueModel
    {
        public List<ManteinanceDataModel> messages { get; set; }
        public SortingViewModel sorting { get; set; }

    }

    public class ManteinanceDataModel
    {
        public string code { get; set; }
        public string type { get; set; }
        public TimeViewModel time { get; set; }

        public Nullable<double> expiredSpan { get; set; }
        public string description { get; set; }
    }

}
