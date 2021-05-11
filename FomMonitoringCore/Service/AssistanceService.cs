using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.SqlServer;
using Mapster;
using UserManager.Service;

namespace FomMonitoringCore.Service
{
    class AssistanceService : IAssistanceService
    {
        private readonly IFomMonitoringEntities _fomMonitoringEntities;
        private readonly IUserServices _userServices;

        public AssistanceService(IFomMonitoringEntities fomMonitoringEntities, IUserServices userServices)
        {
            _fomMonitoringEntities = fomMonitoringEntities;
            _userServices = userServices;
        }

        public List<UserModel> GetCustomers()
        {
            List<UserModel> result = null;
            try
            {
                List<string> plants = _fomMonitoringEntities.Set<Plant>().Select(p => p.UserId.ToString()).ToList();

                var userQuery = _fomMonitoringEntities.Set<Users>()
                    .Include("Roles_Users")
                    .Include("Roles_Users.Roles")
                    .Include("Languages").AsQueryable()
                    .Where(w => w.Roles_Users.Any(a => a.Roles.IdRole == (int)enRole.Customer) && w.CompanyName != null && plants.Contains(w.ID.ToString())).OrderBy(o => o.CompanyName);


                var users = userQuery.ToList();
                result = users.Adapt<List<Users>, List<UserModel>>();
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public List<PlantModel> GetCustomerPlants(Guid customerId)
        {
            var result = new List<PlantModel>();

            try
            {
                var query = _fomMonitoringEntities.Set<Plant>().Where(w => w.UserId == customerId).ToList();
                result = query.Adapt<List<PlantModel>>();
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), customerId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public UserModel GetMachineCustomer(int idMachine)
        {
            var plantId = _fomMonitoringEntities.Set<Machine>().Find(idMachine)?.PlantId;
            if (plantId == null) return null;
            var userId = _fomMonitoringEntities.Set<Plant>().Find(plantId)?.UserId;
            if (userId == null) return null;
            return _userServices.GetUserById(userId.ToString())?.Adapt<UserModel>();

        }

        public UserModel GetUser(string idUser)
        {
            return _userServices.GetUserById(idUser)?.Adapt<UserModel>();
        }


    }
}
