using System;
using System.Collections.Generic;
using FomMonitoringCore.Framework.Common;
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
        int? GetMachineModelIdByModelCode(int? modelCode);
        List<int> GetMachinePanels(ContextModel context);
        int? GetMachineTypeIdByTypeName(string typeName);
        int? GetMachineTypeIdByModelCode(int? modelCode);
        int? GetShiftByStartTime(int machineId, DateTime? startTime);
        List<MachineInfoModel> GetUserMachines(Guid UserID);

        List<int> GetMachinePanels(int? MachineModelId);
        ParameterMachineValueModel GetProductionValueModel(MachineInfoModel context, enPanel pp);
    }
}