using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Framework.Model.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.Service.API
{
    public interface IXmlDataService
    {
        void AddOrUpdateMachineParameter(ParametersMachineModelXml m);
    }
}
