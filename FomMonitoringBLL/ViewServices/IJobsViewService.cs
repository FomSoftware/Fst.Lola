﻿using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewServices
{
    public interface IJobsViewService
    {
        JobViewModel GetJobs(ContextModel context);
    }
}