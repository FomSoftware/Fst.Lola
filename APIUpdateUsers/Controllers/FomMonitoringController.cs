using FomMonitoringCore.Framework.Model;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace APIUpdateUsers.Controllers
{
    [Authorize]
    public class FomMonitoringController : ApiController
    {
        // GET fommonitoring/getcustomers
        [Route("GetCustomers")]
        public JsonCustomersModel GetCustomers()
        {
            JsonCustomersModel result = new JsonCustomersModel();
            JsonMachine machine = new JsonMachine()
            {
                serial = "319C1000141",
                expirationDate = new DateTime(2019, 12, 31)
            };
            result.customers = new List<JsonCustomer>();
            result.customers.Add(new JsonCustomer()
            {
                username = "mservillo",
                machines = new List<JsonMachine>() { machine }
            });
            return result;
        }

        // GET fommonitoring/getlogin
        [Route("GetAuthentication")]
        public JsonLoginModel GetAuthentication()
        {
            return new JsonLoginModel()
            {
                result = "NOT EXISTS"
            };
        }
    }
}
