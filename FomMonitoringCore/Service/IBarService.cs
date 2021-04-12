using System;
using FomMonitoringCore.Framework.Model;
using System.Collections.Generic;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringCore.Service
{
    public interface IBarService : IDisposable
    {
        List<HistoryBarModel> GetAggregationBar(MachineInfoModel machine, PeriodModel period);
        //int? GetBarIdByBarIdOldAndMachineId(int? barIdOld, int machineId, DateTime start, DateTime end, string JCode);
        int? GetBarId(int? barId, int machineId, List<Bar> listaBarre, string JCode);
    }
}