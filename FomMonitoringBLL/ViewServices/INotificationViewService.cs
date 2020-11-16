using System.Collections.Generic;
using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewServices
{
    public interface INotificationViewService
    {
        List<ManteinanceDataModel> GetNotification(ContextModel context);
        void SetNotificationAsRead(int id, ContextModel context);
    }
}