using System.Collections.Generic;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface IParameterMachineService
    {
        List<ParameterMachineValueModel> GetParameters(MachineInfoModel machine, int idPanel, int? idCluster = null);
    }
}