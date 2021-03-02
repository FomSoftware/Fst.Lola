using System;
using System.Collections.Generic;

namespace FomMonitoringCore.Framework.Model
{
    public class JsonCustomersModel
    {
        public List<JsonCustomer> customers { get; set; }
    }

    public class JsonCustomer
    {
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string companyName { get; set; }
        public List<JsonMachine> machines { get; set; }
    }

    public class JsonMachine
    {
        public string serial { get; set; }
        public DateTime expirationDate { get; set; }

        public DateTime activationDate { get; set; }
        
        public string machineName { get; set; }
    }
}
