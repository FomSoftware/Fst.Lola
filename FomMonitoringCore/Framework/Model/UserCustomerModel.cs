using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.Framework.Model
{
    public class UserCustomerModel
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }       
        public string Username { get; set; }

    }
}
