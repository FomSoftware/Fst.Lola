using System.Collections.Generic;

namespace UserManager.Service
{
    public interface IAdministrativeServices
    {
        /// <summary>
        /// Restituisce una lista di UserInfo
        /// </summary>
        /// <returns>Lista di UserInfo</returns>
        List<DAL.vw_UserInfo> GetListOfUserInfo();

        /// <summary>
        /// Resituisce la lista di tutti i languages definiti nel database
        /// </summary>
        /// <returns></returns>
        List<DAL.Languages> GetLanguages();
    }
}
