using System.Collections.Generic;

namespace FomMonitoringBLL.ViewModel
{
    public class ToolVueModel
    {
        public List<ToolDataModel> tools { get; set; } = new List<ToolDataModel>();
        public SortingViewModel sorting { get; set; }
    }
}