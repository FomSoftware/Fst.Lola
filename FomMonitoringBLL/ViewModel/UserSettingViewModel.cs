using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewModel
{
    public class UserSettingViewModel
    {
        public string CurrentTimezone { get; set; }
        public List<MachineInfoModel> Machine { get; set; }
    }
}
