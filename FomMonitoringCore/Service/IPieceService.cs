using System;
using System.Collections.Generic;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface IPieceService
    {
        List<HistoryPieceModel> GetAggregationPieces(MachineInfoModel machine, PeriodModel period, enDataType dataType);
        List<HistoryPieceModel> GetAllHistoryPiecesByMachineId(int machineId, DateTime dateFrom, DateTime dateTo, enAggregation typePeriod);
        List<HistoryPieceModel> GetAllHistoryPiecesByMachineIdSystem(int machineId, string system, DateTime dateFrom, DateTime dateTo, enAggregation typePeriod);
    }
}