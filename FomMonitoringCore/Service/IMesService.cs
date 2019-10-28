using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Model;
using System;
using System.Collections.Generic;
using UserManager.DAL;

namespace FomMonitoringCore.Service
{
    public interface IMesService
    {

        int? GetPlantDefaultByMachine(string machineSerial);
        int? GetOrSetPlantDefaultByUser(Guid userId);
        int? GetOrSetPlantIdByPlantName(string plantName, string plantAddress, string machineSerial);
        Plant AddPlant(Users user, string plantName = null, string plantAddress = null);
        List<PlantModel> GetUserPlants(Guid UserID);
        List<MesUserMachinesModel> GetPlantData(PlantModel plant);
        List<PlantModel> GetAllPlantsMachines();
    }
}