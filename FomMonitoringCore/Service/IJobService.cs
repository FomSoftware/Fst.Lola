using System;
using System.Collections.Generic;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface IJobService : IDisposable
    {
        List<HistoryJobModel> GetAggregationJobs(MachineInfoModel machine, PeriodModel period);
        List<HistoryJobModel> GetAllHistoryJobs(MachineInfoModel machine, PeriodModel period);
        int? GetJobIdByJobCode(string jobCode, int machineId, DateTime? endDateTime);
        void CleanHistoryJobs();
    }
}