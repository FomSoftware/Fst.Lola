using System;

namespace FomMonitoringCore.Framework.Model
{
    public class LanguagesModel
    {
        public System.Guid ID { get; set; }
        public string Name { get; set; }
        public string InitialsLanguage { get; set; }
        public Nullable<int> IdLanguage { get; set; }
        public string DotNetCulture { get; set; }
    }
}
