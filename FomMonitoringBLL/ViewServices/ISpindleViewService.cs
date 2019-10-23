using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewServices
{
    public interface ISpindleViewService
    {
        SpindleVueModel GetSpindles(ContextModel context);
        XSpindleViewModel GetXSpindles(ContextModel context);
    }
}