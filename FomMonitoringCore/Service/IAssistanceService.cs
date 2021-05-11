using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface IAssistanceService
    {
        List<UserModel> GetCustomers();
        List<PlantModel> GetCustomerPlants(Guid customerId);
        UserModel GetMachineCustomer(int idMachine);
        UserModel GetUser(string idUser);
    }
}
