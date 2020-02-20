using System;
using System.Collections.Generic;

namespace UserManager.Service
{
    public interface ILanguageServives
    {
        List<DAL.Languages> GetLanguages();

        void ModifyLanguage(UserManager.DAL.Languages language);

        void DeleteLanguage(Guid LanguageID);

        DAL.Languages GetLanguage(Guid LanguageID);

        List<UserManager.DAL.Languages> GetListOfLanguage(DAL.UserManagerEntities ModelloEntity);


    }
}
