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
        public List<JsonMachine> machines { get; set; }
    }

    public class JsonMachine
    {
        public string serial { get; set; }
        public DateTime expirationDate { get; set; }
    }
}
