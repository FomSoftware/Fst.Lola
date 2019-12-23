using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewServices
{
    public interface IPanelParametersViewService
    {
        PanelParametersViewModel GetParameters(ContextModel context);
        MultiSpindleParameterVueModel GetMultiSpindleVueModel(MachineInfoModel machine, int? position);
    }
}