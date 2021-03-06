﻿using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringResources;

namespace FomMonitoringBLL.ViewServices
{
    public class NotificationViewService: INotificationViewService
    {
        private readonly IMessageService _messageService;

        public NotificationViewService(IMessageService messageService)
        {
            _messageService = messageService;

        }

        public void SetNotificationAsRead(int id, ContextModel context)
        {
            _messageService.SetNotificationAsRead(id, context.User.ID.ToString());
        }

        public List<ManteinanceDataModel> GetNotification(ContextModel context)
        {
            var period = new PeriodModel
            {
                StartDate = DateTime.UtcNow.Date.AddTicks(1),
                EndDate = DateTime.UtcNow.AddDays(1).Date.AddTicks(-1),
            };

            var messages = new List<ManteinanceDataModel>();
            if (context.AllMachines != null)
            {
                foreach (var machine in context.AllMachines)
                {
                    var userId = context.User.ID.ToString();
                    var data = _messageService.GetMaintenanceNotifications(machine, period, userId);

                    data = data.Where(m => m.Day.HasValue && m.Day.Value.Date == DateTime.UtcNow.Date).ToList();

                    var mes = data.Select(a =>
                        new ManteinanceDataModel
                        {
                            id = a.Id,
                            code = a.Code,
                            type = ((enTypeAlarm)a.Type).GetDescription(),
                            time = CommonViewService.getTimeViewModel(a.ElapsedTime),
                            timestamp = DateTime.SpecifyKind(a.Day ?? DateTime.MinValue, DateTimeKind.Utc),
                            utc = machine.UTC,
                            expiredSpan = CommonViewService.getTimeViewModel(_messageService.GetExpiredSpan(a)),
                            description = a.Description,
                            machineName = machine.MachineName,
                            machineModel = machine.Model.Name,
                            machineSerial = machine.Serial,
                            panelName = Resource.PeriodicMaintenance
                        }).ToList();

                    messages.AddRange(mes);
                }
            }
            
            
            messages = messages.OrderByDescending(o => o.expiredSpan.elapsed).ToList();


            return messages;
        }
    }
}
