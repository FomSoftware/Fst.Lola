using System.Collections.Generic;

namespace FomMonitoringBLL.ViewModel
{
    public class PlantMessagesViewModel
    {
        public List<MachineMessagesDataViewModel> messages { get; set; }
        public PlantInfoViewModel plant { get; set; }

        public SortingViewModel sorting { get; set; }
        public double UtcOffset { get; set; }
        public string timeZone { get; set; }
    }


    public class MachineMessagesDataViewModel
    {
        public MessageDetailViewModel message { get; set; }

        public MachineInfoViewModel machine { get; set; }
    }


}
