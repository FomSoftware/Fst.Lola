using System;
using System.Collections.Generic;
namespace UserManager.Service
{
    public interface ILoggedUserServices
    {
        Guid GetLoggedUserID();
        string GetLoggedUserName();

        FomMonitoringCore.SqlServer.Users GetLoggedUser();

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
            bool CheckUserRole(FomMonitoringCore.SqlServer.Roles role);

            /// <summary>
            /// Recupera i ruoli associati all'utente loggato
            /// </summary>
            /// <returns></returns>
            List<FomMonitoringCore.SqlServer.Roles> GetLoggedUserRoles();

            string GetLoggedUserRolesString();
        #endregion

        #region User Groups

        #endregion


    }
}
