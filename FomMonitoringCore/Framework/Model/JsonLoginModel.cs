using FomMonitoringCore.Framework.Common;
using System.ComponentModel;

namespace FomMonitoringCore.Framework.Model
{
    public class JsonLoginModel
    {
        public string result { get; set; }
        public enLoginResult enResult
        {
            get
            {
                return enResult;
            }
            private set
            {
                enResult = result.GetValueFromAttribute<enLoginResult, DescriptionAttribute>(a => a.Description);
            }
        }
    }
}
