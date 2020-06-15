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
using HistoryJobPieceBar = FomMonitoringCore.DataProcessing.Dto.Mongo.HistoryJobPieceBar;
using Info = FomMonitoringCore.DataProcessing.Dto.Mongo.Info;
using Message = FomMonitoringCore.DataProcessing.Dto.Mongo.Message;
using State = FomMonitoringCore.DataProcessing.Dto.Mongo.State;
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

            var repository = container.Resolve<IGenericRepository<State>>();
            var repositoryHistoryJobPieceBar = container.Resolve<IGenericRepository<HistoryJobPieceBar>>();
            var repositoryMessage = container.Resolve<IGenericRepository<Message>>();
            var repositoryVariablesList = container.Resolve<IGenericRepository<VariablesList>>();
            var forwarder = container.Resolve<IQueueForwarder>();


            var jsonsState = repository.Query(vl => vl.DateSendedQueue > new DateTime(2020, 6, 12)).ToList();
            var jsonshistory = repositoryHistoryJobPieceBar.Query(vl => vl.DateSendedQueue > new DateTime(2020, 6, 12)).ToList();
            var jsonsMessage = repositoryMessage.Query(vl => vl.DateSendedQueue > new DateTime(2020, 6, 12)).ToList();
            var jsonsVariablesList = repositoryVariablesList.Query(vl => vl.DateSendedQueue > new DateTime(2020, 6, 12)).ToList();


            foreach (var data in jsonsState)
            {
                
                forwarder.Forward(JsonConvert.SerializeObject(data));
            }

            foreach (var data in jsonshistory)
            {

                forwarder.Forward(JsonConvert.SerializeObject(data));
            }

            foreach (var data in jsonsMessage)
            {

                forwarder.Forward(JsonConvert.SerializeObject(data));
            }

            foreach (var data in jsonsVariablesList)
            {

                forwarder.Forward(JsonConvert.SerializeObject(data));
            }

            Debugger.Break();
        }
    }
}
