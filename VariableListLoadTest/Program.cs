using System;
using System.Diagnostics;
using System.Linq;
using Autofac;
using FomMonitoringCore.DAL;
using FomMonitoringCore.DalMongoDb;
using FomMonitoringCore.DataProcessing.Dto.Mongo;
using FomMonitoringCore.Repository.MongoDb;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.Forwarder;
using FomMonitoringCoreQueue.ProcessData;
using FomMonitoringCoreQueue.QueueConsumer;
using FomMonitoringCoreQueue.QueueProducer;
using Newtonsoft.Json;

namespace VariableListLoadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            FomMonitoringCoreQueue.Ioc.IocContainerBuilder.BuildCore(builder, false);
            
            var container = builder.Build();

            var context = container.Resolve<IFomMonitoringEntities>();
            var forwarder = container.Resolve<IQueueForwarder>();

            
            var jsons = context.Set<JsonData>().Where(h => h.Json.Contains("variable") && h.ElaborationDate > new DateTime(2020, 1, 1)).OrderByDescending(i => i.Id).ToList().AsParallel()
                .Select(o => new {o.Id, o.Json}).OrderByDescending(n => n.Id).ToList();
            

            foreach (var data in jsons)
            {
                forwarder.Forward(data.Json);
                break;
            }

            Debugger.Break();
        }
    }
}
