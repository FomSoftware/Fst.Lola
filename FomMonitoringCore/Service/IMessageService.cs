﻿using System;
using System.Collections.Generic;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringCore.Service
{
    public interface IMessageService
    {
        List<MessageMachineModel> GetMessageDetails(MachineInfoModel machine, PeriodModel period, string actualMachineGroup = null);
        void CheckMaintenance();
        void InsertMessageMachine(Machine machine, MessagesIndex msgIndex, DateTime day);
        List<HistoryMessageModel> GetAggregationMessages(MachineInfoModel machine, PeriodModel period, enDataType dataType, string actualMachineGroup = null);
        long? GetExpiredSpan(MessageMachineModel mm);
        List<MessageMachineModel> GetMaintenanceMessages(MachineInfoModel machine, PeriodModel period);
        List<MessageMachineModel> GetOldMaintenanceMessages(MachineInfoModel machine, PeriodModel period);
        List<MessageMachineModel> GetMaintenanceNotifications(MachineInfoModel machine, PeriodModel period, string userId);
        bool IgnoreMessage(int messageId);
        void SetNotificationAsRead(int id, string userId);
    }
}