using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;
using System.Collections.Generic;

namespace FomMonitoringBLL.ViewServices
{
    public interface IMesViewService
    {
        List<MesDataViewModel> GetVueModel(PlantModel plant, List<MachineInfoModel> allMachines, bool onlyActive);
        MesViewModel GetMes(ContextModel context);
    }
}