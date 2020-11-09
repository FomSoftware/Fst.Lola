using System;
using System.Diagnostics;
using System.Text;
using FomMonitoringCore.Mongo.Repository;
using FomMonitoringCore.Queue.Connection;
using FomMonitoringCore.Queue.Dto;
using FomMonitoringCore.Queue.Events;
using FomMonitoringCore.Queue.ProcessData;
using FomMonitoringCore.Service;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FomMonitoringCore.Queue.QueueConsumer
{
    public class MessageConsumer : IConsumer<Message>
    {
        private readonly IProcessor<Message> _processor;
        private readonly IQueueConnection _queueConnection;
        private readonly IGenericRepository<Mongo.Dto.Message> _messageGenericRepository;
        private EventingBasicConsumer consumer;

        public MessageConsumer(IProcessor<Message> processor,
            IQueueConnection queueConnection,
            IGenericRepository<Mongo.Dto.Message> messageGenericRepository)
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
                var data = new Mongo.Dto.Message();
                try
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var ii = JsonConvert.DeserializeObject<Message>(message);

                    data = _messageGenericRepository.Find(ii.ObjectId);
                    if (data == null)
                    {
                        Log?.Invoke(this, new LoggerEventsQueue
                        {
                            Message = $"Finita elaborazione Messages {ii.ObjectId} con errori - {DateTime.UtcNow:O} tempo trascorso {elapsedTime}",
                            Exception = new Exception($"Messaggio {ii.ObjectId} non trovato... eliminato dalla coda..."),
                            TypeLevel = LogService.TypeLevel.Error,
                            Type = TypeEvent.Messages
                        });
                        _queueConnection.ChannelMessages.BasicAck(ea.DeliveryTag, false);
                        return;
                    }

                    data.DateStartElaboration = DateTime.UtcNow;

                    if (_processor.ProcessData(ii))
                    {
                        data.DateEndElaboration = DateTime.UtcNow;
                        data.ElaborationSuccesfull = true;
                        stopWatch.Stop();
                        // Get the elapsed time as a TimeSpan value.
                        var ts = stopWatch.Elapsed;

                        // Format and display the TimeSpan value.
                        elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}.{ts.Milliseconds:00}";
                        _queueConnection.ChannelMessages.BasicAck(ea.DeliveryTag, false);

                        Log?.Invoke(this, new LoggerEventsQueue
                        {
                            Message = $"Finita elaborazione Messages {ii.ObjectId} - { DateTime.UtcNow:O} tempo trascorso { elapsedTime }",
                            Exception = null,
                            TypeLevel = LogService.TypeLevel.Info,
                            Type = TypeEvent.Messages
                        });
                    }
                    else
                    {
                        _queueConnection.ChannelMessages.BasicNack(ea.DeliveryTag, false, true);
                        throw new Exception("Errore elaborazione json senza eccezioni");
                    }
                }
                catch (Exception ex)
                {
                    data.DateEndElaboration = DateTime.UtcNow;
                    data.ElaborationSuccesfull = false;
                    _queueConnection.ChannelMessages.BasicNack(ea.DeliveryTag, false, true);
                    Log?.Invoke(this, new LoggerEventsQueue
                    {
                        Message = $"Finita elaborazione Messages {data.Id.ToString()} con errori - {DateTime.UtcNow:O} tempo trascorso {elapsedTime}",
                        Exception = ex,
                        TypeLevel = LogService.TypeLevel.Error,
                        Type = TypeEvent.Messages
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