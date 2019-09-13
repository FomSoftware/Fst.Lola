namespace FomMonitoringBLL.ViewModel
{
    public class SpindleDataModel
    {
        public string code { get; set; }
        public decimal? perc { get; set; }
        public decimal velocity { get; set; }
        public WorkOverModel workover { get; set; }
        public InfoModel info { get; set; }
        public TimeToLiveModel time { get; set; }
        public ChartViewModel opt_bands { get; set; }
    }
}