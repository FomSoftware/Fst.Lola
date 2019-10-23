using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewServices
{
    public interface IToolsViewService
    {
        ToolViewModel GetTools(ContextModel context);
    }
}