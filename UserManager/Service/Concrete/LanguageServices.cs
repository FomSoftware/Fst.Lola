using System;
using System.Collections.Generic;
using FomMonitoringCore.SqlServer;

namespace UserManager.Service.Concrete
{
    public class LanguageServices : ILanguageServives
    {
        public void ModifyLanguage(Languages language)
        {
            Gateway.Concrete.Languages.ModifyLanguage(language);
        }

        public void DeleteLanguage(Guid languageId)
        {
            Gateway.Concrete.Languages.DeleteLanguage(languageId);
        }

        public List<Languages> GetListOfLanguage(FomMonitoringEntities modelloEntity)
        {
            return Gateway.Concrete.Languages.GetListOflanguage(modelloEntity);
        }
        
        public Languages GetLanguage(Guid languagesId)
        {
            return Gateway.Concrete.Languages.GetLanguages(languagesId);
        }

        public List<Languages> GetLanguages()
        {
            return Gateway.Concrete.Languages.GetLanguages();
        }
    }
}
