using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;
using System.Collections.Generic;

namespace FomMonitoringBLL.ViewServices
{
    public interface IMesViewService
    {
        MesViewModel GetMes(ContextModel context);
    }
}