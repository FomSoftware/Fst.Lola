using System;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface IContextService
    {
        void CheckLastUpdate();
        bool CheckSecurityParameterApi(int id, enCheckParam check);
        ContextModel GetContext();
        bool InitializeAdminLevel();
        bool InitializeContext();
        bool InitializeMachineLevel(int? machineId = null);
        bool InitializeMesLevel();
        bool InitializeUserSettingLevel();
        bool InitializePlantManagerLevel();
        void SetActualLanguage(string languageNameIso);
        void SetActualTimeZone(string timezone);
        void SetActualMachine(int id);
        void SetActualPeriod(DateTime start, DateTime end);
        void SetActualPlant(int id);
        void SetContext(ContextModel context);
        void SetActualMachineGroup(string group);
        bool InitializeUploadConfigurationLevel();
        bool InitializeAssistanceLevel();
    }
}