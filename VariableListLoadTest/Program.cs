using System;
using System.Collections.ObjectModel;
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

            //ReadOnlyCollection<TimeZoneInfo> zones = TimeZoneInfo.GetSystemTimeZones();
            //foreach (TimeZoneInfo zone in zones)
            //{
            //        Console.WriteLine($"{zone.Id} - {zone.DisplayName} - {zone.StandardName}");
            //}
            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            FomMonitoringCore.Queue.Ioc.IocContainerBuilder.BuildCore(builder, false);

            var container = builder.Build();

            var repository = container.Resolve<IGenericRepository<State>>();
            var repositoryInfo = container.Resolve<IGenericRepository<Info>>();
            var repositoryHistoryJobPieceBar = container.Resolve<IGenericRepository<Tool>>();
            var repositoryMessage = container.Resolve<IGenericRepository<Message>>();
            var repositoryVariablesList = container.Resolve<IGenericRepository<VariablesList>>();
            var forwarder = container.Resolve<IQueueForwarder>();
            //var reforwarder = container.Resolve<IUnknownForwarder>();
            //reforwarder.ReForward();

            var jsonsMessage = repositoryMessage.Query().ToList().OrderByDescending(n => n.DateReceived).Take(1000).ToList();
            var jsonsMessagevar = repositoryVariablesList.Query().ToList().OrderByDescending(n => n.DateReceived).Take(1000).ToList();


            foreach (var data in jsonsMessage)
            {
                forwarder.Forward(JsonConvert.SerializeObject(data));
            }

            foreach (var data in jsonsMessagevar)
            {
                forwarder.Forward(JsonConvert.SerializeObject(data));
            }
            //var x = new TimeSpan(13400000000);
            Console.ReadKey();
            
        }
    }
}
