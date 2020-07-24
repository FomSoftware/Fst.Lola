using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FomMonitoring.RequestDto
{
    public class MachineTimezoneRequestDto
    {
        public int IdMachine { get; set; }
        public string TimeZone { get; set; }
    }
}