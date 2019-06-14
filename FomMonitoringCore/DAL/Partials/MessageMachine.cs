using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.DAL
{
    public partial class MessageMachine
    {
        public DateTime? GetInitialSpanDate(int PeriodicSpan)
        {
            DateTime? result = this.Machine.ActivationDate;
           
            if (IgnoreDate == null || PeriodicSpan == 0)
                return result;

            while( result < DateTime.Now)
            {
                if (result?.AddDays(PeriodicSpan) < DateTime.Now)
                    result = result?.AddDays(PeriodicSpan);
                else
                    break;
            }
            return result;
        }
       
    }
}
