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
using VariablesList = FomMonitoringCore.DataProcessing.Dto.Mongo.VariablesList;

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

            var repository = container.Resolve<IGenericRepository<VariablesList>>();
            var forwarder = container.Resolve<IQueueForwarder>();


            var jsons = repository.Query(vl => vl.DateStartElaboration > new DateTime(2020, 6, 7)).ToList();


            foreach (var data in jsons)
            {
                
                forwarder.Forward(JsonConvert.SerializeObject(data));
            }

            Debugger.Break();
        }
    }
}
