using System;
using System.Collections.Generic;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface IMessageService
    {
        List<MessageMachineModel> GetMessageDetails(MachineInfoModel machine, PeriodModel period);
        void CheckMaintenance();
        void InsertMessageMachine(Machine machine, string code, DateTime day);
    }
}