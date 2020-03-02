using System;
using FomMonitoringCore.Framework.Model;
using System.Collections.Generic;

namespace FomMonitoringCore.Service
{
    public interface IBarService : IDisposable
    {
        List<HistoryBarModel> GetAggregationBarSP(MachineInfoModel machine, PeriodModel period);
        List<HistoryBarModel> GetAggregationBar(MachineInfoModel machine, PeriodModel period);
        int? GetBarIdByBarIdOldAndMachineId(int? barIdOld, int machineId);
    }
}