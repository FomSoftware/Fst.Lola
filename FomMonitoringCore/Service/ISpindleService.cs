using System.Collections.Generic;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface ISpindleService
    {
        List<SpindleModel> GetSpindles(MachineInfoModel machine, bool xmodule = false);
    }
}