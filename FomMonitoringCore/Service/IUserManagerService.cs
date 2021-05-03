using System;
using System.Collections.Generic;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringCore.Service
{
    public interface IUserManagerService
    {
        /// <summary>
        /// Ritorna il singolo utente con le sue info
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        UserModel GetUser(Guid userId);

        /// <summary>
        /// Ritorna il singolo utente con le sue info
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        UserModel GetUser(string username);

        List<UserModel> GetAllCustomers();
        void RemoveUsers(List<UserModel> users);

        /// <summary>
        /// Ritorna la lista degli utenti con le relative info
        /// </summary>
        /// <returns>Lista customer</returns>
        List<UserModel> GetUsers(string customerName);

        List<UserModel> FilterRoleUsers(enRole idRole, List<UserModel> users);

        /// <summary>
        /// Ritorna la lista dei clienti
        /// </summary>
        /// <returns>Lista customer</returns>
        List<string> GetCustomerNames();

        /// <summary>
        /// Ritorna la lista dei ruoli
        /// </summary>
        /// <returns></returns>
        List<RoleModel> GetRoles();

        /// <summary>
        /// Ritorna la lista delle macchine associate al cliente
        /// </summary>
        /// <param name="customerName"></param>
        /// <returns></returns>
        List<MachineInfoModel> GetCustomerMachines(string customerName);

        /// <summary>
        /// Restituisce la lista delle lingue
        /// </summary>
        /// <returns></returns>
        List<Languages> GetLanguages();

        /// <summary>
        /// Aggiunge un nuovo utente
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Guid CreateUser(UserModel user);

        bool SendPassword(string email, Guid id, string keySubject, string keyObject);

        /// <summary>
        /// Modifica un utente esistente
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool ModifyUser(UserModel user, string email);

        /// <summary>
        /// Modifica la password dell'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        bool ChangePassword(Guid userId, string oldPassword, string newPassword);

        /// <summary>
        /// Modifica la password dell'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool ResetPassword(Guid userId);

        /// <summary>
        /// elimina l'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool DeleteUser(Guid userId);

        void ChangeTimeZone(Guid userId, string timezone);

        void UpdateUserName(UserModel userModel);
    }
}