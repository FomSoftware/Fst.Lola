using System;
using System.Collections.Generic;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service
{
    public interface IMessageService
    {
        List<MessageMachineModel> GetMessageDetails(MachineInfoModel machine, PeriodModel period);
        void CheckMaintenance();
        void InsertMessageMachine(Machine machine, string code, DateTime day);
        List<HistoryMessageModel> GetAggregationMessages_SP(MachineInfoModel machine, PeriodModel period, enDataType dataType);
        List<HistoryMessageModel> GetAggregationMessages(MachineInfoModel machine, PeriodModel period, enDataType dataType);
        List<HistoryMessageModel> GetAllHistoryMessages(MachineInfoModel machine, PeriodModel period);
        long? GetExpiredSpan(MessageMachineModel mm);
        List<MessageMachineModel> GetMaintenanceMessages(MachineInfoModel machine, PeriodModel period);
        List<MessageMachineModel> GetMaintenanceNotifications(MachineInfoModel machine, PeriodModel period, string userId);
        List<MessageMachineModel> GetAllCurrentMessages(MachineInfoModel machine, PeriodModel period);
        bool IgnoreMessage(int messageId);
        void SetNotificationAsRead(int id, string userId);
    }
}