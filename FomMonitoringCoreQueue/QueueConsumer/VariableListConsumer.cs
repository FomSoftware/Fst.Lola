using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FomMonitoringCore.DalMongoDb;
using FomMonitoringCore.DataProcessing.Dto.Mongo;
using FomMonitoringCore.Repository.MongoDb;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.Events;
using FomMonitoringCoreQueue.ProcessData;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VariablesList = FomMonitoringCoreQueue.Dto.VariablesList;

namespace FomMonitoringCoreQueue.QueueConsumer
{
    public class VariableListConsumer : IConsumer<VariablesList>
    {
        private readonly IProcessor<VariablesList> _processor;
        private readonly IQueueConnection _queueConnection;
        private readonly IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.VariablesList> _variableGenericRepository;
        private EventingBasicConsumer consumer;

        public VariableListConsumer(IProcessor<VariablesList> processor, IQueueConnection queueConnection,
            IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.VariablesList> variableGenericRepository)
        {
            _processor = processor;
            _queueConnection = queueConnection;
            _variableGenericRepository = variableGenericRepository;
        }

        public event EventHandler<LoggerEventsQueue> Log;

        public void Init()
        {

            consumer = new EventingBasicConsumer(_queueConnection.ChannelVariableList);
            consumer.Received += ConsumerOnReceived();

            _queueConnection.ChannelVariableList.BasicConsume("VariableList",false, consumer);
                
        }

        private EventHandler<BasicDeliverEventArgs> ConsumerOnReceived()
        {
            return (model, ea) =>
            {
                var elapsedTime = string.Empty;
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                var data = new FomMonitoringCore.DataProcessing.Dto.Mongo.VariablesList();
                try
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var ii = JsonConvert.DeserializeObject<VariablesList>(message);

                    data = _variableGenericRepository.Find(ii.ObjectId);
                    data.DateStartElaboration = DateTime.UtcNow;

                    if (_processor.ProcessData(ii))
                    {
                        data.DateEndElaboration = DateTime.UtcNow;
                        data.ElaborationSuccesfull = true;

                    }

                    stopWatch.Stop();
                    // Get the elapsed time as a TimeSpan value.
                    var ts = stopWatch.Elapsed;

                    // Format and display the TimeSpan value.
                    elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}.{ts.Milliseconds:00}";
                    _queueConnection.ChannelVariableList.BasicAck(ea.DeliveryTag, false);
                    Log?.Invoke(this, new LoggerEventsQueue
                    {
                        Message = $"Finita elaborazione VariableLists {data.Id.ToString()} - { DateTime.UtcNow:O} tempo trascorso { elapsedTime }",
                        Exception = null,
                        TypeLevel = LogService.TypeLevel.Info,
                        Type = TypeEvent.Variable
                    });
                }
                catch (Exception ex)
                {

                    data.DateEndElaboration = DateTime.UtcNow;
                    data.ElaborationSuccesfull = false;


                    Log?.Invoke(this, new LoggerEventsQueue
                    {
                        Message = $"Finita elaborazione con errori json {DateTime.UtcNow:O} tempo trascorso {elapsedTime}",
                        Exception = ex,
                        TypeLevel = LogService.TypeLevel.Error,
                        Type = TypeEvent.Info
                    });
                }
                finally
                {
                    _variableGenericRepository.Update(data);
                }
            };
        }

        public void Dispose()
        {
            consumer.Received -= ConsumerOnReceived();
            _processor?.Dispose();
        }
    }


}
