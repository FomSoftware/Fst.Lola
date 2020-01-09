using System;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface IContextService
    {
        void CheckLastUpdate();
        bool CheckSecurityParameterApi(int ID, enCheckParam check);
        ContextModel GetContext();
        bool InitializeAdminLevel();
        bool InitializeContext();
        bool InitializeMachineLevel(int? MachineID = null);
        bool InitializeMesLevel();
        bool InitializeMessagesMachineLevel();
        bool InitializePlantManagerLevel();
        void SetActualLanguage(string LanguageNameISO);
        void SetActualMachine(int id);
        void SetActualPage(enPage Page);
        void SetActualPeriod(DateTime start, DateTime end);
        void SetActualPlant(int id);
        void SetContext(ContextModel context);
        void SetActualMachineGroup(string group);
    }
}