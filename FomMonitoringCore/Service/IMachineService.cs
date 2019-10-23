using System;
using System.Collections.Generic;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface IMachineService
    {
        bool AddMachineModel(string modelName, int modelCode);
        List<MachineInfoModel> GetAllMachines();
        List<MachineInfoModel> GetAllMachinesByPlantID(int PlantID);
        DateTime GetLastUpdateByMachineSerial(string machineSerial);
        MachineInfoModel GetMachineInfo(int MachineID);
        int? GetMachineModelIdByModelCodeOrName(int? modelCode, string modelName);
        List<int> GetMachinePanels(ContextModel context);
        int? GetMachineTypeIdByTypeName(string typeName);
        int? GetShiftByStartTime(int machineId, DateTime? startTime);
        List<MachineInfoModel> GetUserMachines(Guid UserID);
    }
}