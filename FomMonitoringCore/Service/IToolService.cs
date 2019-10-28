using FomMonitoringCore.Framework.Model;
using System.Collections.Generic;

namespace FomMonitoringCore.Service
{
    public interface IToolService
    {
        List<ToolMachineModel> GetTools(MachineInfoModel machine, bool xmodule = false);
    }
}