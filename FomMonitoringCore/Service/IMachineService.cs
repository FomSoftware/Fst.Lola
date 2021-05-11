using System;
using System.Collections.Generic;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface IMachineService
    {
        List<MachineInfoModel> GetAllMachines();
        DateTime GetLastUpdateByMachineSerial(string machineSerial);
        MachineInfoModel GetMachineInfo(int MachineID);
        int? GetMachineModelIdByModelCode(int? modelCode);
        List<int> GetMachinePanels(ContextModel context);
        int? GetMachineTypeIdByModelCode(int? modelCode);
        int? GetShiftByStartTime(int machineId, DateTime? startTime);
        List<MachineInfoModel> GetUserMachines(ContextModel user);

        List<int> GetMachinePanels(int? MachineModelId);
        CurrentStateModel GetCurrentStateModel(int MachineID);
        List<MachineInfoModel> GetRoleMachines(enRole role);
        List<MachineInfoModel> GetPlantMachines(ContextModel ctx);
    }
}