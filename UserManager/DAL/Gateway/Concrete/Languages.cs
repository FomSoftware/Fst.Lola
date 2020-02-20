using System;
using System.Collections.Generic;
using System.Linq;

namespace UserManager.DAL.Gateway.Concrete
{
    public static class Languages
    {
        public static List<DAL.Languages> GetListOflanguage(UserManagerEntities userManagerEntities)
        {
            try
            {

                return (from lng in userManagerEntities.Languages
                        select lng).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error while getting audit login: {0}", ex.Message));
            }

        }

        public static void DeleteLanguage(Guid LanguageID)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    // Verifico che la lingua esista nel db
                    if (!userManagerEntities.Languages.Where(l => l.ID == LanguageID).Any())
                    {
                        // Errore - lingua non trovato
                        throw new Exception("Error lingua  not found: {0}");
                    }

                    // Recupero la lingua dal db
                    DAL.Languages languageToDelete = userManagerEntities.Languages.Where(l => l.ID == LanguageID).FirstOrDefault();

                    userManagerEntities.Languages.Remove(languageToDelete);


                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the delete of the Language: {0}", ex.Message));
            }
        }

        public static bool CheckIfLanguageAlreadyExist(string Name)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return userManagerEntities.Languages.Where(l => l.Name.ToLower() == Name.ToLower()).Any();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Modifica alcune proprietà dell'language
        /// </summary>
        /// <param name="User"></param>
        public static void ModifyLanguage(DAL.Languages Language)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    // Verifico che la lingua esista nel db
                    if (!userManagerEntities.Languages.Where(l => l.Name.ToLower() == Language.Name.ToLower()).Any())
                    {
                        // Errore - lingua non trovata
                        throw new Exception("Error language not found: {0}");
                    }

                    // Recupero la lingua dal db
                    DAL.Languages linguaToModify = userManagerEntities.Languages.Where(l => l.Name.ToLower() == Language.Name.ToLower()).FirstOrDefault();

                    // Aggiorno  con le nuove informazioni (solo alcuni campi vengono aggiornati)
                    linguaToModify.Name = Language.Name;
                    linguaToModify.InitialsLanguage = Language.InitialsLanguage;
                    linguaToModify.DotNetCulture = Language.DotNetCulture;
                    
                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the update of the language: {0}", ex.Message));
            }
        }
       
        public static DAL.Languages GetLanguages(Guid LanguagesId)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return (from language in userManagerEntities.Languages
                            where language.ID == LanguagesId
                            select language).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Errore : {0} (Error #111009)", ex.Message));
            }
        }


          public static List<DAL.Languages> GetLanguages()
       
          {
            using (UserManagerEntities userManagerEntities = new UserManagerEntities())
            {
                return userManagerEntities.Languages.Select(s => s).ToList();
            }
        }
    }
}
