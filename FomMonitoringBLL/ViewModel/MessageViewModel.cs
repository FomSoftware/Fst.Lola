using System.Collections.Generic;

namespace FomMonitoringBLL.ViewModel
{
    public class MessageViewModel
    {
        public MessageVueModel vm_messages { get; set; }

        public MessageDetailsVueModel vm_details { get; set; }

        public ChartViewModel opt_historical { get; set; }
    }
    public class MessageVueModel
    {
        public List<MessageDataModel> messages { get; set; }
        public SortingViewModel sorting { get; set; }
    }

    public class MessageDataModel
    {
        public string code { get; set; }
        public string type { get; set; }
        public TimeViewModel time { get; set; }
        public int quantity { get; set; }
        public string parameters { get; set; }

        public string day { get; set; }
    }

    public class MessageDetailsVueModel
    {
        public List<MessageDetailViewModel> messages { get; set; }
        public SortingViewModel sorting { get; set; }

    }

}