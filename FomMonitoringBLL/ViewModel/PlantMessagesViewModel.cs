using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewModel
{
    public class PlantMessagesViewModel
    {
        public List<MachineMessagesDataViewModel> messages { get; set; }
        public PlantInfoViewModel plant { get; set; }

        public SortingViewModel sorting { get; set; }
    }


    public class MachineMessagesDataViewModel
    {
        public MessageDetailViewModel message { get; set; }

        public MachineInfoViewModel machine { get; set; }
    }


}
