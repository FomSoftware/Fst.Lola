using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewServices
{
    public interface IMachineViewService
    {
        MachineViewModel GetMachine(ContextModel context);
    }
}