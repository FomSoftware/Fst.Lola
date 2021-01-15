using FomMonitoringCore.SqlServer;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ToolMachine = FomMonitoringCore.SqlServer.ToolMachine;

namespace FomMonitoringCore.Service
{
    public class ToolService : IToolService
    {
        private readonly IFomMonitoringEntities _context;

        public ToolService(IFomMonitoringEntities context)
        {
            _context = context;
        }

        public List<ToolMachineModel> GetTools(MachineInfoModel machine, bool xmodule = false)
        {
            List<ToolMachineModel> result = new List<ToolMachineModel>();

            try
            {
                
                List<ToolMachine> query = null;                   
                if (machine.Type.Id == (int)enMachineType.LineaTaglioLavoro)
                {
                    Regex regex = new Regex(@"^[1-2]\d{3}$");
                    if (xmodule)
                    {
                        query = _context.Set<ToolMachine>().Where(w => w.IsActive && w.MachineId == machine.Id).ToList().Where(w => regex.IsMatch(w.Code)).OrderBy(w => w.CodeAsInt).ThenByDescending(w => w.CurrentLife).ToList();
                    }
                    else
                    {
                        query = _context.Set<ToolMachine>().Where(w => w.IsActive && w.MachineId == machine.Id).ToList().Where(w => !regex.IsMatch(w.Code)).OrderBy(w => w.CodeAsInt).ThenByDescending(w => w.CurrentLife).ToList();
                    }
                }
                else
                    query = _context.Set<ToolMachine>().Where(w => w.IsActive && w.MachineId == machine.Id).ToList().OrderBy(w => w.CodeAsInt).ThenByDescending(w => w.CurrentLife).ToList();

                result = query.Adapt<List<ToolMachineModel>>();
                
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
