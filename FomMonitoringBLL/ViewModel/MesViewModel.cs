using System.Collections.Generic;

namespace FomMonitoringBLL.ViewModel
{
    public class MesViewModel
    {
        public List<MesDataViewModel> machines { get; set; }
    }


    public class MesDataViewModel
    {
        public MachineInfoViewModel info { get; set; }

        public StateViewModel state { get; set; }

        public string error { get; set; }

        public JobDataModel job { get; set; }

        public string @operator { get; set; }

        public EfficiencyVueModel efficiency { get; set; }

        public ProductivityVueModel productivity { get; set; }

        public ErrorViewModel errors { get; set; }

    }
}
