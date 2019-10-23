using System.Collections.Generic;
using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewServices
{
    public interface IPlantMessagesViewService
    {
        PlantMessagesViewModel GetPlantMessages(ContextModel context);
        List<MachineMessagesDataViewModel> GetVueModel(PlantModel plant, List<MachineInfoModel> allMachines, PeriodModel period);
    }
}