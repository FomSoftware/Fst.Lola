using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FomMonitoringCore.DAL;
using FomMonitoringCore.DAL;
using FomMonitoringCore.DalMongoDb;
using FomMonitoringCore.DataProcessing.Dto.Mongo;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Repository.MongoDb;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
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
            builder.RegisterType<QueueConnection>().As<IQueueConnection>().SingleInstance();
            builder.RegisterType<VariableListConsumer>().As<IVariableListConsumer>().SingleInstance();
            builder.RegisterType<VariableListProcessor>().As<IVariableListProcessor>().SingleInstance();
            builder.RegisterType<VariableListProducer>().As<IVariableListProducer>().SingleInstance();


            var container = builder.Build();

            var context = container.Resolve<IFomMonitoringEntities>();
            var variableListProducer = container.Resolve<IVariableListProducer>();



            var jsons = context.Set<JsonData>().Where(j => j.Json.Contains("variablesList")).OrderBy(i => i.Id).Take(100).ToList()
                .Select(o => JsonConvert.DeserializeObject<MachineDataModel>(o.Json)).ToList();

            foreach (var data in jsons)
            {
                data.IsCumulative = false;
                data.DateReceived = DateTime.UtcNow;
                var mongoContext = new MongoDbContext();
                var repo = new GenericRepository<MachineDataModel>(mongoContext);
                repo.Create(data);

                variableListProducer.Send(new VariablesList()
                {
                    ObjectId = data.Id.ToString(),
                    InfoMachine = data.info.FirstOrDefault(),
                    VariablesListMachine = data.variablesList
                });


                repo.Update(data);
            }

            Debugger.Break();
        }
    }
}
