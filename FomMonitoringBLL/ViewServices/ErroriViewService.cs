using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class ErroriViewService
    {
        public static ListaErroriViewModel GetErrori(ContextModel context)
        {
            ListaErroriViewModel result = new ListaErroriViewModel();

            List<AlarmMachineModel> erroriData = AlarmService.GetAllCurrentAlarms(context.ActualMachine, context.ActualPeriod);



            result.vm_errori = erroriData.Select(n => new ErroriViewModel
            {
                Code = n.Code,
                Messaggio = n.Description,
                Timestamp = n.Day
            }).ToList();

            return result;
        }


    }
}