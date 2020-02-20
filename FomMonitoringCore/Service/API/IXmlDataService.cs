using FomMonitoringCore.Framework.Model.Xml;
using System.Threading.Tasks;

namespace FomMonitoringCore.Service.API
{
    public interface IXmlDataService
    {
        Task AddOrUpdateMachineParameterAsync(ParametersMachineModelXml m);
    }
}
