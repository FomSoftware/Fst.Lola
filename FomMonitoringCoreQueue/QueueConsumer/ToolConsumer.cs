using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FomMonitoringCore.DalMongoDb;
using FomMonitoringCore.Repository.MongoDb;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.ProcessData;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Tool = FomMonitoringCoreQueue.Dto.Tool;
using FomMonitoringCoreQueue.Events;

namespace FomMonitoringCoreQueue.QueueConsumer
{
    public class ToolConsumer : IConsumer<Tool>
    {
        private readonly IProcessor<Tool> _processor;
        private readonly IQueueConnection _queueConnection;
        private IMongoDbContext _mongoContext;
        private readonly IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.Tool> _toolGenericRepository;
        private EventingBasicConsumer consumer;

        public ToolConsumer(IProcessor<Tool> processor, IQueueConnection queueConnection,
            IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.Tool> toolGenericRepository)
        {
            _processor = processor;
            _queueConnection = queueConnection;
            _toolGenericRepository = toolGenericRepository;
        }

        public event EventHandler<LoggerEventsQueue> Log;

        public void Init()
        {
            consumer = new EventingBasicConsumer(_queueConnection.ChannelState);
            consumer.Received += ConsumerOnReceived();

            _queueConnection.ChannelTool.BasicConsume("Tool", false, consumer);
        }

        private EventHandler<BasicDeliverEventArgs> ConsumerOnReceived()
        {
            return (model, ea) =>
            {
                var elapsedTime = string.Empty;
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                var data = new FomMonitoringCore.DataProcessing.Dto.Mongo.Tool();
                try
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var ii = JsonConvert.DeserializeObject<Tool>(message);

                    data = _toolGenericRepository.Find(ii.ObjectId);
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
                    _queueConnection.ChannelTool.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    data.DateEndElaboration = DateTime.UtcNow;
                    data.ElaborationSuccesfull = false;

                    LogService.WriteLog(
                        $"Finita elaborazione con errori json tool {DateTime.UtcNow:O} tempo trascorso {elapsedTime} ", LogService.TypeLevel.Error, ex);

                }
                finally
                {
                    LogService.WriteLog(
                        $"Finita elaborazione json tool {DateTime.UtcNow:O} tempo trascorso {elapsedTime} ", LogService.TypeLevel.Info);
                    _toolGenericRepository.Update(data);
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