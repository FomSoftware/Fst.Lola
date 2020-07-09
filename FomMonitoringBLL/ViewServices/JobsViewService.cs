using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class JobsViewService : IJobsViewService
    {
        private readonly IJobService _jobService;

        public JobsViewService(IJobService jobService)
        {
            _jobService = jobService;
        }

        public JobViewModel GetJobs(ContextModel context)
        {
            JobViewModel result = new JobViewModel();

            List<HistoryJobModel> jobsData = _jobService.GetAllHistoryJobs(context.ActualMachine, context.ActualPeriod);
            result.vm_jobs = GetVueModel(context.ActualMachine, context.ActualPeriod);

            return result;
        }

        private JobVueModel GetVueModel(MachineInfoModel machine, PeriodModel period)
        {
            JobVueModel result = new JobVueModel();

            List<HistoryJobModel> data = _jobService.GetAggregationJobs(machine, period);

            if (data.Count == 0)
                return result;

            List<JobDataModel> jobs = data.Select(j => new JobDataModel()
            {
                code = j.Code,
                perc = Common.GetPercentage(j.PiecesProduced ?? 0, j.TotalPieces ?? 0).RoundToInt() > 100 ? 100 : Common.GetPercentage(j.PiecesProduced ?? 0, j.TotalPieces ?? 0).RoundToInt(),
                time = CommonViewService.getTimeViewModel(j.ElapsedTime),
                quantity = j.PiecesProduced ?? 0,
                pieces = j.TotalPieces ?? 0,
                day = j.Day.GetValueOrDefault()
            }).ToList();

            jobs = jobs.OrderBy(o => o.perc).ToList();

            SortingViewModel sorting = new SortingViewModel();
            sorting.progress = enSorting.Ascending.GetDescription();

            result.jobs = jobs;
            result.sorting = sorting;

            return result;
        }
    }
}
