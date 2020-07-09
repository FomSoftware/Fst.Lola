using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FomMonitoringCore.Queue.Dto;
using FomMonitoringCore.SqlServer;
using Mapster;
using ToolMachine = FomMonitoringCore.SqlServer.ToolMachine;

namespace FomMonitoringCore.Queue.ProcessData
{
    public class ToolProcessor : IProcessor<Tool>
    {
        private readonly ILifetimeScope _parentScope;


        public ToolProcessor(ILifetimeScope parentScope)
        {
            _parentScope = parentScope;
        }
        public bool ProcessData(Tool data)
        {
            try
            {
                using (var threadLifetime = _parentScope.BeginLifetimeScope())
                using (var _context = threadLifetime.Resolve<IFomMonitoringEntities>())
                {
                    
                    var serial = data.InfoMachine.First().MachineSerial;
                    var mac = _context.Set<Machine>().AsNoTracking().FirstOrDefault(m => m.Serial == serial);
                    if (mac == null)
                        return false;

                    foreach (var tool in data.ToolMachine)
                    {
                        tool.DateLoaded = tool.DateLoaded.HasValue && tool.DateLoaded.Value.Year < 1900 ? null : tool.DateLoaded;
                        tool.DateReplaced = tool.DateReplaced.HasValue && tool.DateReplaced.Value.Year < 1900 ? null : tool.DateReplaced;
                    }
                    var tools = data.ToolMachine.BuildAdapter().AddParameters("machineId", mac.Id).AdaptToType<List<ToolMachine>>();

                    //richiesta di Romina: non svuotare mai i tools se non arrivano tenere quelli vecchi
                    if (tools != null && tools.Any())
                    {
                        var removeTools = data.ToolMachine.Join(_context.Set<ToolMachine>(), to => to.Code, from => from.Code, (to, from) => new { From = from, To = to })
                            .Where(w => w.From.MachineId == mac.Id && w.From.Code == w.To.Code && w.From.DateLoaded == w.To.DateLoaded).Select(s => s.From).ToList();
                        _context.Set<ToolMachine>().RemoveRange(removeTools);
                        _context.SaveChanges();
                        var modifyTools = _context.Set<ToolMachine>().Where(w => w.MachineId == mac.Id && w.IsActive).ToList();
                        foreach (var modifyTool in modifyTools)
                        {
                            modifyTool.IsActive = false;
                        }
                        _context.SaveChanges();

                        _context.Set<ToolMachine>().AddRange(tools);
                    }
                    _context.SaveChanges();

                    return true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            _parentScope?.Dispose();
        }
    }
}