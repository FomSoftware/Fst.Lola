using System;
using System.Collections.Generic;

namespace UserManager.Service
{
    public interface ILanguageServives
    {
        List<FomMonitoringCore.SqlServer.Languages> GetLanguages();

        void ModifyLanguage(FomMonitoringCore.SqlServer.Languages language);

        void DeleteLanguage(Guid languageId);

        FomMonitoringCore.SqlServer.Languages GetLanguage(Guid languageId);

        List<FomMonitoringCore.SqlServer.Languages> GetListOfLanguage(FomMonitoringCore.SqlServer.FomMonitoringEntities modelloEntity);


    }
}
