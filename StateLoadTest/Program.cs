using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FomMonitoringCore.DalMongoDb;
using FomMonitoringCore.DataProcessing.Dto.Mongo;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.QueueConsumer;
using FomMonitoringCoreQueue.QueueProducer;
using Newtonsoft.Json;
using FomMonitoringCore.Repository.MongoDb;

namespace StateLoadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            builder.RegisterType<QueueConnection>().As<IQueueConnection>().SingleInstance();
            builder.RegisterType<StateConsumer>().As<IConsumer<State>>().SingleInstance();
            builder.RegisterType<StateProducer>().As<IProducer<State>>().SingleInstance();


            var container = builder.Build();

            var context = container.Resolve<FomMonitoringCore.DAL.IFomMonitoringEntities>();
            var stateProducer = container.Resolve<IProducer<State>>();



            var jsons = context.Set<FomMonitoringCore.DAL.JsonData>().Where(j => j.Json.Contains("state")).OrderByDescending(i => i.Id).Take(10).ToList()
                .Select(o => JsonConvert.DeserializeObject<MachineDataModel>(o.Json)).ToList();

            foreach (var data in jsons)
            {
                data.IsCumulative = false;
                data.DateReceived = DateTime.UtcNow;
                var mongoContext = new MongoDbContext();
                var repo = new GenericRepository<MachineDataModel>(mongoContext);
                repo.Create(data);

                stateProducer.Send(new State()
                {
                    ObjectId = data.Id.ToString(),
                    InfoMachine = data.info.FirstOrDefault(),
                    StateMachine = data.state
                });


                repo.Update(data);
            }

            Debugger.Break();
        }
    }
}
