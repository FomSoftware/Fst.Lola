using FomMonitoringCore.Framework.Model;
using System.Collections.Generic;
using FomMonitoringCore.Framework.Common;

namespace FomMonitoringCore.Service
{
    public interface IPlantManagerService
    {
        List<PlantModel> GetPlants(string usernameCustomer);
        List<PlantModel> FilterPlantsByRole(enRole idRole, List<PlantModel> users);
        PlantModel GetPlant(int plantId);
        PlantModel GetPlantByMachine(int id);
        bool DeletePlant(int id);
        IEnumerable<MachineInfoModel> GetMachinesByPlant(int id);
        int CreatePlant(PlantModel plant);
        bool ModifyPlant(PlantModel plant);
    }
}