using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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

                            var tools = context.Set<ToolMachine>().Where(t =>
                                t.MachineId == mac.Id && tool.Code == t.Code).OrderByDescending(t => t.DateReplaced);

                                
                            if (tools.Count() > 1)
                            {
                                var toolToRemove = tools.Skip(1).ToList();
                                context.Set<ToolMachine>().RemoveRange(toolToRemove);

                                context.SaveChanges();
                            }

                            var toolToUpdate = tools.FirstOrDefault() ?? new ToolMachine();
                            toolToUpdate.DateReplaced = tool.DateReplaced;
                            toolToUpdate.DateLoaded = tool.DateLoaded;
                            toolToUpdate.Code = tool.Code;
                            toolToUpdate.MachineId = mac.Id;
                            toolToUpdate.ExpectedLife = tool.ExpectedLife;
                            toolToUpdate.CurrentLife = tool.CurrentLife;
                            toolToUpdate.Description = tool.Description;
                            toolToUpdate.IsActive = true;
                            context.Set<ToolMachine>().AddOrUpdate(toolToUpdate);
                            context.SaveChanges();
                        }

                    }
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