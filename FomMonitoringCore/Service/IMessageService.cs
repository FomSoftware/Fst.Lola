using System.Collections.Generic;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface IMessageService
    {
        List<MessageMachineModel> GetMessageDetails(MachineInfoModel machine, PeriodModel period);
    }
}