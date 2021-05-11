using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;
using System.Collections.Generic;

namespace FomMonitoringBLL.ViewServices
{
    public interface IAssistanceViewService
    {
        AssistanceViewModel GetAssistance(ContextModel context);

        void SetCompanyName(ContextModel context);
    }
}