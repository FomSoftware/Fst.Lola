using System.Collections.Generic;

namespace FomMonitoringBLL.ViewModel
{
    public class SpindleVueModel
    {
        public List<SpindleDataModel> spindles { get; set; }
        public SortingViewModel sorting { get; set; }
    }
}