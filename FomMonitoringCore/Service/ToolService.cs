using CommonCore.Service;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.Service
{
    public class ToolService
    {
        public static List<ToolMachineModel> GetTools(MachineInfoModel machine)
        {
            List<ToolMachineModel> result = new List<ToolMachineModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<ToolMachine> query = ent.ToolMachine.Where(w => w.MachineId == machine.Id).ToList();
                    result = query.Adapt<List<ToolMachineModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format("{0}(machineId = '{1}')", Common.GetStringLog(), machine.Id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }
    }
}
