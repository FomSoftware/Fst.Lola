using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewServices
{
    public interface IProductivityViewService
    {
        ProductivityViewModel GetProductivity(ContextModel context);
    }
}