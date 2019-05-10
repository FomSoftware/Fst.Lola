using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FomMonitoringCore.Service
{
    public class ToolService
    {
        public static List<ToolMachineModel> GetTools(MachineInfoModel machine, bool xmodule = false)
        {
            List<ToolMachineModel> result = new List<ToolMachineModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<ToolMachine> query = null;                   
                    if (machine.Type.Id == (int)enMachineType.LineaTaglioLavoro)
                    {
                        Regex regex = new Regex(@"^[1-2]\d{2}$");
                        if (xmodule)
                        {
                            query = ent.ToolMachine.Where(w => w.MachineId == machine.Id).ToList().Where(w => regex.IsMatch(w.Code)).ToList();
                        }
                        else
                        {
                            query = ent.ToolMachine.Where(w => w.MachineId == machine.Id).ToList().Where(w => !regex.IsMatch(w.Code)).ToList();
                        }
                    }
                    else
                        query = ent.ToolMachine.Where(w => w.MachineId == machine.Id).ToList();

                    result = query.Adapt<List<ToolMachineModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machine.Id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }
    }
}
