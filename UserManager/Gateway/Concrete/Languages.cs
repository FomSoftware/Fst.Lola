using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.SqlServer;

namespace UserManager.Gateway.Concrete
{
    public static class Languages
    {
        public static List<FomMonitoringCore.SqlServer.Languages> GetListOflanguage(FomMonitoringEntities userManagerEntities)
        {
            try
            {

                return (from lng in userManagerEntities.Languages
                        select lng).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting audit login: {ex.Message}");
            }

        }

        public static void DeleteLanguage(Guid languageId)
        {
            try
            {
                using (var userManagerEntities = new FomMonitoringEntities())
                {
                    // Verifico che la lingua esista nel db
                    if (!userManagerEntities.Languages.Any(l => l.ID == languageId))
                    {
                        // Errore - lingua non trovato
                        throw new Exception("Error lingua  not found: {0}");
                    }

                    // Recupero la lingua dal db
                    var languageToDelete = userManagerEntities.Languages.FirstOrDefault(l => l.ID == languageId);

                    if (languageToDelete != null) userManagerEntities.Languages.Remove(languageToDelete);


                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the delete of the Language: {ex.Message}");
            }
        }

        public static bool CheckIfLanguageAlreadyExist(string name)
        {
            try
            {
                using (var userManagerEntities = new FomMonitoringEntities())
                {
                    return userManagerEntities.Languages.Any(l => l.Name.ToLower() == name.ToLower());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Modifica alcune proprietà dell'language
        /// </summary>
        /// <param name="language"></param>
        public static void ModifyLanguage(FomMonitoringCore.SqlServer.Languages language)
        {
            try
            {
                using (var userManagerEntities = new FomMonitoringEntities())
                {
                    // Verifico che la lingua esista nel db
                    if (!userManagerEntities.Languages.Any(l => string.Equals(l.Name, language.Name, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        // Errore - lingua non trovata
                        throw new Exception("Error language not found: {0}");
                    }

                    // Recupero la lingua dal db
                    var linguaToModify = userManagerEntities.Languages.FirstOrDefault(l => string.Equals(l.Name, language.Name, StringComparison.CurrentCultureIgnoreCase));

                    // Aggiorno  con le nuove informazioni (solo alcuni campi vengono aggiornati)
                    linguaToModify.Name = language.Name;
                    linguaToModify.InitialsLanguage = language.InitialsLanguage;
                    linguaToModify.DotNetCulture = language.DotNetCulture;
                    
                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the update of the language: {ex.Message}");
            }
        }
       
        public static FomMonitoringCore.SqlServer.Languages GetLanguages(Guid languagesId)
        {
            try
            {
                using (var userManagerEntities = new FomMonitoringEntities())
                {
                    return (from language in userManagerEntities.Languages
                            where language.ID == languagesId
                            select language).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore : {ex.Message} (Error #111009)");
            }
        }


          public static List<FomMonitoringCore.SqlServer.Languages> GetLanguages()
       
          {
            using (var userManagerEntities = new FomMonitoringEntities())
            {
                return userManagerEntities.Languages.Select(s => s).ToList();
            }
        }
    }
}
