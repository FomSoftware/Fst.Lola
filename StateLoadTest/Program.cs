using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FomMonitoringCore.DAL;
using FomMonitoringCore.DalMongoDb;
using FomMonitoringCore.DataProcessing.Dto.Mongo;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.QueueConsumer;
using FomMonitoringCoreQueue.QueueProducer;
using Newtonsoft.Json;
using FomMonitoringCore.Repository.MongoDb;
using FomMonitoringCoreQueue.Forwarder;

namespace StateLoadTest
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





            var jsons = context.Set<JsonData>().Where(j => j.Json.Contains("state")).OrderByDescending(i => i.Id).Take(10).ToList()
                .Select(o => o.Json).ToList();

            foreach (var data in jsons)
            {
                forwarder.Forward(data);
            }

            Debugger.Break();
            


        }
    }
}
