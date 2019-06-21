using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCheckMaintenance
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MessageService.CheckMaintenance();
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), string.Join(", ", args));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

        }
    }
}
