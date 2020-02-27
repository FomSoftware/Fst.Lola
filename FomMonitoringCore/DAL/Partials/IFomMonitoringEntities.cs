using System;
using System.Data.Entity.Core.Objects;
using FomMonitoringCore.DAL;

namespace FomMonitoringCore.DAL
{
    public interface IFomMonitoringEntities : IDbContext
    {
        ObjectResult<Nullable<int>> usp_CleanMachineData(Nullable<int> machineId);
        ObjectResult<usp_AggregationState_Result> usp_AggregationState(Nullable<int> machineId, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> aggregation, Nullable<int> dataType);
        ObjectResult<usp_AggregationPiece_Result> usp_AggregationPiece(Nullable<int> machineId, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> aggregation, Nullable<int> dataType);
        ObjectResult<usp_AggregationBar_Result> usp_AggregationBar(Nullable<int> machineId, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> aggregation);
        ObjectResult<usp_AggregationJob_Result> usp_AggregationJob(Nullable<int> machineId, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> aggregation);
        int usp_HistoricizingAll(Nullable<int> machineId);
        int usp_HistoricizingMessages(Nullable<int> machineId);
        int usp_HistoricizingBars(Nullable<int> machineId);
        int usp_HistoricizingPieces(Nullable<int> machineId);
    }
}
