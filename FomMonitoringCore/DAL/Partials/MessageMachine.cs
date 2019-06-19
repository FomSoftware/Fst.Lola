using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.DAL
{
    public partial class MessageMachine
    {
        public DateTime? GetInitialSpanDate(long PeriodicSpan)
        {
            DateTime? result = Machine.ActivationDate;
           
            if (IgnoreDate == null || PeriodicSpan == 0)
                return result;            

            while ( result < DateTime.Now)
            {
                DateTime? newInit = result?.AddTicks(PeriodicSpan);

                if (newInit < DateTime.Now)
                { 
                    result = result?.AddTicks(PeriodicSpan);
                    if (result > IgnoreDate)
                        break;
                }
                else
                    break;
            }
            return result;
        }
       
    }
}
