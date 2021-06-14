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
            var repositoryHistoryJobPieceBar = container.Resolve<IGenericRepository<HistoryJobPieceBar>>();
            var repositoryMessage = container.Resolve<IGenericRepository<Message>>();
            var repositoryVariablesList = container.Resolve<IGenericRepository<VariablesList>>();
            var forwarder = container.Resolve<IQueueForwarder>();
            //var reforwarder = container.Resolve<IUnknownForwarder>();
            //reforwarder.ReForward();

            //var jsonsMessage = repositoryHistoryJobPieceBar.Query(n => n.info[0].MachineSerial == "C0800418").ToList().OrderByDescending(n => n.DateReceived).Take(1).ToList();
            //var jsonsMessagevar = repositoryVariablesList.Query(n => n.info[0].MachineSerial == "C0800418").OrderBy(n => n.variablesList[0].UtcDateTime).ToList().OrderByDescending(n => n.DateReceived).Take(1000).ToList();
            var jsonsMessage = repositoryMessage.Query(n => n.info[0].MachineSerial == "A1400013" && n.message[0].Code == "212").ToList().OrderByDescending(n => n.DateReceived).Take(1).ToList();


            foreach (var data in jsonsMessage)
            {
                string aa =
                    "{\"tool\": [{\"Id\": 1,\"Code\": 1,\"Description\": \"Mill D.5 Pos.11\",\"DateLoaded\": \"2021-01-01T00:00:00Z\",\"DateReplaced\": \"\",\"CurrentLife\": 476.0,\"ExpectedLife\": 360000.0},{\"Id\": 2,\"Code\": 2,\"Description\": \"Mill D.8 Pos.12\",\"DateLoaded\": \"2021-01-01T00:00:00Z\",\"DateReplaced\": \"\",\"CurrentLife\": 911.0,\"ExpectedLife\": 360000.0},{\"Id\": 3,\"Code\": 3, \"Description\": \"Mill D.3 Pos.15\", \"DateLoaded\": \"2021-01-01T00:00:00Z\", \"DateReplaced\": \"\", \"CurrentLife\": 1155.0, \"ExpectedLife\": 360000.0}],  \"info\": [{ \"KeyId\": \"2021563044\", \"MachineSerial\": \"B0900006\", \"MachineCode\": 211, \"Product\": \"FSTLine4\", \"ProductVersion\": \"4.2.0.198\", \"FirmwareVersion\": \"1.7.2 - 8/11/2019\", \"PlcVersion\": \"1.6.3.0\", \"LoginDate\": \"2021-06-14T08:09:12.7193924Z\", \"UTC\": 0 }]}";

                //aa = JsonConvert.SerializeObject(data);
                forwarder.Forward(aa);
            }

            //foreach (var data in jsonsMessagevar)
            //{
            //    forwarder.Forward(JsonConvert.SerializeObject(data));
            //}
            //var x = new TimeSpan(13400000000);
            Console.ReadKey();
            
        }
    }
}
