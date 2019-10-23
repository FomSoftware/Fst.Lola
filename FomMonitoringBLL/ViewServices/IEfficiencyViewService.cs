using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewServices
{
    public interface IEfficiencyViewService
    {
        EfficiencyViewModel GetEfficiency(ContextModel context);
        EfficiencyVueModel GetVueModel(MachineInfoModel machine, PeriodModel period);
    }
}