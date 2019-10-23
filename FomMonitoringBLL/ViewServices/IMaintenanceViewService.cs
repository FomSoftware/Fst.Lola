using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewServices
{
    public interface IMaintenanceViewService
    {
        MaintenanceViewModel GetMessages(ContextModel context);
        bool IgnoreMessage(int MessageId);
    }
}