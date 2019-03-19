using System;
using System.Collections.Generic;
namespace UserManager.Service
{
    public interface ILoggedUserServices
    {
        Guid GetLoggedUserID();
        string GetLoggedUserName();

        UserManager.DAL.Users GetLoggedUser();

        string GetLoggedUserDefualtHomePage();

        #region User Role
            /// <summary>
            /// Verifica se l'utente ha tra i suoi ruoli quello passato come parametro
            /// </summary>
            /// <param name="idRole">Id Ruolo</param>
            /// <returns></returns>
            bool CheckUserRole(int idRole);

            /// <summary>
            /// Verifica se l'utente ha tra i suoi ruoli quello passato come parametro
            /// </summary>
            /// <param name="role">Role object</param>
            /// <returns></returns>
            bool CheckUserRole(DAL.Roles role);

            /// <summary>
            /// Recupera i ruoli associati all'utente loggato
            /// </summary>
            /// <returns></returns>
            List<DAL.Roles> GetLoggedUserRoles();

            string GetLoggedUserRolesString();
        #endregion

        #region User Groups

        /// <summary>
        /// Recupera i gruppi associati all'utente loggato
        /// </summary>
        /// <returns></returns>
        List<DAL.Groups> GetLoggedUserGroups();

        #endregion

        /// <summary>
        /// Salva nella tabella RedirectAccessRequests il Guid della richiesta e il Guid dell'utente per potere
        /// effettuare l'accesso in differita
        /// </summary>
        /// <returns>il Guid della richiesta da poter passare alla pagina di dettaglio che dovrebbe passarlo alla pagina che effettua il 
        /// login in differita</returns>
        Guid SetUserRedirectAccessRequestsAndGetGuidRequest();
    }
}
