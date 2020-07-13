using System;
using System.Diagnostics;
using System.Linq;
using Autofac;
using FomMonitoringCore.Mongo.Dto;
using FomMonitoringCore.Mongo.Repository;
using FomMonitoringCore.Queue.Forwarder;
using Newtonsoft.Json;

namespace VariableListLoadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            FomMonitoringCore.Queue.Ioc.IocContainerBuilder.BuildCore(builder, false);
            
            var container = builder.Build();

            var repository = container.Resolve<IGenericRepository<State>>();
            var repositoryInfo = container.Resolve<IGenericRepository<Info>>();
            var repositoryHistoryJobPieceBar = container.Resolve<IGenericRepository<HistoryJobPieceBar>>();
            var repositoryMessage = container.Resolve<IGenericRepository<Message>>();
            var repositoryVariablesList = container.Resolve<IGenericRepository<VariablesList>>();
            var forwarder = container.Resolve<IQueueForwarder>();

            
            var jsonsMessage = repositoryVariablesList.Query(var => var.variablesList.Any(vl => vl.Values.Any(v => v.VariableResetDate > new DateTime(2020, 05,15)))).ToList();

            
            foreach (var data in jsonsMessage)
            {
                forwarder.Forward(JsonConvert.SerializeObject(data));
            }


            Debugger.Break();
        }
    }
}
