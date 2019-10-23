using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewServices
{
    public interface IMessagesViewService
    {
        MessageViewModel GetMessages(ContextModel context);
    }
}