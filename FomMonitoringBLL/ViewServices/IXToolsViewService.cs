using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewServices
{
    public interface IXToolsViewService
    {
        XToolViewModel GetXTools(ContextModel context);
    }
}