
using FomMonitoringCore.Framework.Model;
using System;
using System.Collections.Generic;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringCore.Service
{
    public interface IMesService
    {
        int? GetOrSetPlantDefaultByUser(Guid userId);
        List<PlantModel> GetUserPlants(Guid UserID);
        List<MesUserMachinesModel> GetPlantData(PlantModel plant);
        List<PlantModel> GetAllPlantsMachines();
        List<PlantModel> GetAllPlantsRole(enRole role);

        void CheckOfflineMachines();
        PlantModel GetMachinePlant(int? idMachine);
    }
}