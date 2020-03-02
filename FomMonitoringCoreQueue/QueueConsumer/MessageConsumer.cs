using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FomMonitoringCore.Repository.MongoDb;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.Events;
using FomMonitoringCoreQueue.ProcessData;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FomMonitoringCoreQueue.QueueConsumer
{
    public class MessageConsumer : IConsumer<Message>
    {
        private readonly IProcessor<Message> _processor;
        private readonly IQueueConnection _queueConnection;
        private readonly IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.Message> _messageGenericRepository;
        private EventingBasicConsumer consumer;

        public MessageConsumer(IProcessor<Message> processor,
            IQueueConnection queueConnection,
            IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.Message> messageGenericRepository)
        {
            _processor = processor;
            _queueConnection = queueConnection;
            _messageGenericRepository = messageGenericRepository;
        }

        public event EventHandler<LoggerEventsQueue> Log;

        public void Init()
        {
            consumer = new EventingBasicConsumer(_queueConnection.ChannelMessages);
            consumer.Received += ConsumerOnReceived();

            _queueConnection.ChannelMessages.BasicConsume("Messages", false, consumer);
        }

        private EventHandler<BasicDeliverEventArgs> ConsumerOnReceived()
        {
            return (model, ea) =>
            {
                var elapsedTime = string.Empty;
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var data = new FomMonitoringCore.DataProcessing.Dto.Mongo.Message();
                try
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var ii = JsonConvert.DeserializeObject<Message>(message);

                    data = _messageGenericRepository.Find(ii.ObjectId);
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
                    _queueConnection.ChannelMessages.BasicAck(ea.DeliveryTag, false);

                    Log?.Invoke(this, new LoggerEventsQueue
                    {
                        Message = $"Finita elaborazione json { DateTime.UtcNow:O} tempo trascorso { elapsedTime }",
                        Exception = null,
                        TypeLevel = LogService.TypeLevel.Info,
                        Type = TypeEvent.Info
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
                    _messageGenericRepository.Update(data);
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