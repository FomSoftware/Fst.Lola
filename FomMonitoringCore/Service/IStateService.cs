using System;
using System.Collections.Generic;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface IStateService
    {
        List<HistoryStateModel> GetAggregationStates(MachineInfoModel machine, PeriodModel period, enDataType dataType);
        List<EfficiencyStateMachineModel> GetOperatorsActivity(MachineInfoModel machineId, DateTime dateFrom, DateTime dateTo);
        List<EfficiencyStateMachineModel> GetDayActivity(MachineInfoModel machine, List<DateTime> days);
        int? GetOrderViewState(string stateCode);
    }
}