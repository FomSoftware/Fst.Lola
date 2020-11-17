using FomMonitoringCore.Framework.Model.Xml;
using System.Threading.Tasks;

namespace FomMonitoringCore.Service.API
{
    public interface IXmlDataService
    {
        Task AddOrUpdateMachineParameterAsync(ParametersMachineModelXml m);

        bool CheckMachineModelCode(int ModelCodeV997);

        bool CheckVarNumber(int ModelCodeV997, string varnumber);

        bool CheckPanelId(int? panelId);

        bool CheckMachineGroup(string machineGroup);
    }


}
