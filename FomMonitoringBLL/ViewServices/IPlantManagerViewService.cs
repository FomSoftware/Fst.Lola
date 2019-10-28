using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;
using System.Collections.Generic;

namespace FomMonitoringBLL.ViewServices
{
    public interface IPlantManagerViewService
    {
        PlantManagerViewModel GetPlants(ContextModel context);
        PlantManagerViewModel GetPlantsByCustomer(string idCustomer);
        PlantManagerViewModel GetPlant(int id);
        PlantManagerViewModel GetPlantByMachine(int id);
        bool EditPlant(PlantViewModel plantModel);
        IEnumerable<UserMachineViewModel> GetMachinesByPlant(int id);
        bool CreatePlant(PlantViewModel plantModel, ContextModel context);
        bool DeletePlant(int id);
    }
}