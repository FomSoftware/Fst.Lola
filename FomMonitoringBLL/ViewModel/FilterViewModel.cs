namespace FomMonitoringBLL.ViewModel
{
    public class FilterViewModel
    {
        public MachineInfoViewModel machine { get; set; }

        public PeriodViewModel period { get; set; }
        public int panelId { get; set; }
        public int cluster { get; set; }
        public string machineGroup { get; set; }
    }
}
