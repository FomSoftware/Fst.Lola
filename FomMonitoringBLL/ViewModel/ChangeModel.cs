using System.Collections.Generic;

namespace FomMonitoringBLL.ViewModel
{
    public class ChangeModel
    {
        //public int total { get; set; }
        public int breaking { get; set; }
        public int replacement { get; set; }
        public List<HistoricalModel> historical { get; set; }
    }
}