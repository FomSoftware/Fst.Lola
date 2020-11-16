using System;
using System.Collections.Generic;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface IPieceService
    {
        List<HistoryPieceModel> GetAggregationPieces(MachineInfoModel machine, PeriodModel period, enDataType dataType);
    }
}