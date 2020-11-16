using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FomMonitoringCore.Queue.Dto;
using FomMonitoringCore.SqlServer;
using FomMonitoringCore.Uow;
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
            using (var scope = _parentScope.BeginLifetimeScope())
            using (var context = scope.Resolve<IFomMonitoringEntities>())
            using (var unitOfWork = new UnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction(context);
                    var serial = data.InfoMachine.First().MachineSerial;
                    var mac = context.Set<Machine>().AsNoTracking().FirstOrDefault(m => m.Serial == serial);
                    if (mac == null)
                        return false;
                    //richiesta di Romina: non svuotare mai i tools se non arrivano tenere quelli vecchi
                    if (data.ToolMachine != null && data.ToolMachine.Any())
                    {
                        foreach (var tool in data.ToolMachine)
                        {
                            tool.DateLoaded = tool.DateLoaded.HasValue && tool.DateLoaded.Value.Year < 1900 ? null : tool.DateLoaded;
                            tool.DateReplaced = tool.DateReplaced.HasValue && tool.DateReplaced.Value.Year < 1900 ? null : tool.DateReplaced;

                            var toolToRemove = context.Set<ToolMachine>().FirstOrDefault(t =>
                                t.MachineId == mac.Id && tool.Code == t.Code && tool.DateLoaded == t.DateLoaded);
                            //var removeTools = data.ToolMachine.Join(_context.Set<ToolMachine>(), to => to.Code, from => from.Code, (to, from) => new { From = from, To = to })
                            //    .Where(w => w.From.MachineId == mac.Id && w.From.Code == w.To.Code && w.From.DateLoaded == w.To.DateLoaded).Select(s => s.From).ToList();
                            if (toolToRemove == null)
                                continue;
                            context.Set<ToolMachine>().Remove(toolToRemove);
                            context.SaveChanges();
                        }

                        var tools = data.ToolMachine.BuildAdapter().AddParameters("machineId", mac.Id).AdaptToType<List<ToolMachine>>();

                        var modifyTools = context.Set<ToolMachine>().Where(w => w.MachineId == mac.Id && w.IsActive).ToList();
                        foreach (var modifyTool in modifyTools)
                        {
                            modifyTool.IsActive = false;
                        }
                        context.SaveChanges();

                        context.Set<ToolMachine>().AddRange(tools);
                    }
                    context.SaveChanges();
                    unitOfWork.CommitTransaction();
                    return true;
                }
                catch (Exception ex)
                {
                    unitOfWork.RollbackTransaction();
                    throw ex;
                }
            }

        }

        public void Dispose()
        {
            _parentScope?.Dispose();
        }
    }
}