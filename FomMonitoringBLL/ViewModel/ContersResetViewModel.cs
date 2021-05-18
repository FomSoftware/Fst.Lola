using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewModel
{
    public class CountersResetViewModel
    {
        public enPage ActualPage { get; set; }
        public List<ParameterResetValueDataModel> lista { get; set; }
    }
}
