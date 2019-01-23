namespace FomMonitoringCore.Framework.Model
{
    public class JsonDataModel
    {
        public int Id { get; set; }
        public bool IsCumulative { get; set; }
        public bool IsElaborated { get; set; }
        public string Json { get; set; }
    }
}
