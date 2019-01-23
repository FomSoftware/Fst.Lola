using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewServices
{
    public class JobsViewService
    {
        public static JobViewModel GetJobs(ContextModel context)
        {
            JobViewModel result = new JobViewModel();

            List<HistoryJobModel> jobsData = JobService.GetAllHistoryJobs(context.ActualMachine, context.ActualPeriod);
            result.vm_jobs = GetVueModel(context.ActualMachine, context.ActualPeriod);

            return result;
        }

        private static JobVueModel GetVueModel(MachineInfoModel machine, PeriodModel period)
        {
            JobVueModel result = new JobVueModel();

            List<HistoryJobModel> data = JobService.GetAggregationJobs(machine, period);

            if (data.Count == 0)
                return result;

            List<JobDataModel> jobs = data.Where(w => w.PiecesProduced <= w.TotalPieces).Select(j => new JobDataModel()
            {
                code = j.Code,
                perc = Common.GetPercentage(j.PiecesProduced ?? 0, j.TotalPieces ?? 0).RoundToInt(),
                time = CommonViewService.getTimeViewModel(j.ElapsedTime),
                quantity_day = j.PiecesProducedDay ?? 0,
                quantity = j.PiecesProduced ?? 0,
                pieces = j.TotalPieces ?? 0
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
