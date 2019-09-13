namespace FomMonitoringBLL.ViewModel
{
    public class ToolDataModel
    {
        public string code { get; set; }
        public string description { get; set; }
        public decimal? perc { get; set; }
        public ChangeModel changes { get; set; }
        public TimeViewModel time { get; set; }
    }
}