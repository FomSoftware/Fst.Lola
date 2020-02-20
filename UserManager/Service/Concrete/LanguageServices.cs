using System;
using System.Collections.Generic;

namespace UserManager.Service.Concrete
{
    public class LanguageServices : ILanguageServives
    {
        public void ModifyLanguage(DAL.Languages Language)
        {
            DAL.Gateway.Concrete.Languages.ModifyLanguage(Language);
        }

        public void DeleteLanguage(Guid LanguageID)
        {
            DAL.Gateway.Concrete.Languages.DeleteLanguage(LanguageID);
        }

        public List<UserManager.DAL.Languages> GetListOfLanguage(DAL.UserManagerEntities ModelloEntity)
        {
            return DAL.Gateway.Concrete.Languages.GetListOflanguage(ModelloEntity);

        }
        public DAL.Languages GetLanguage(Guid LanguagesID)
        {
            return DAL.Gateway.Concrete.Languages.GetLanguages(LanguagesID);
        }

        public List<DAL.Languages> GetLanguages()
        {
            return DAL.Gateway.Concrete.Languages.GetLanguages();
        }
    }
}
