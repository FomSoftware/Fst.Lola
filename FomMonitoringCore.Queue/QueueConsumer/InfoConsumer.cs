using System;
using System.Diagnostics;
using System.Text;
using FomMonitoringCore.Mongo.Repository;
using FomMonitoringCore.Queue.Connection;
using FomMonitoringCore.Queue.Events;
using FomMonitoringCore.Queue.ProcessData;
using FomMonitoringCore.Service;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Info = FomMonitoringCore.Queue.Dto.Info;

namespace FomMonitoringCore.Queue.QueueConsumer
{
    public class InfoConsumer: IConsumer<Dto.Info>
    {
        private readonly IProcessor<Info> _processor;
        private readonly IQueueConnection _queueConnection;
        private readonly IGenericRepository<Mongo.Dto.Info> _infoGenericRepository;
        private EventingBasicConsumer consumer;

        public InfoConsumer(IProcessor<Info> processor, 
            IQueueConnection queueConnection, 
            IGenericRepository<Mongo.Dto.Info> infoGenericRepository)
        {
            _processor = processor;
            _queueConnection = queueConnection;
            _infoGenericRepository = infoGenericRepository;
        }
        

        public event EventHandler<LoggerEventsQueue> Log;

        public void Init()
        {

            consumer = new EventingBasicConsumer(_queueConnection.ChannelInfo);
            consumer.Received += ConsumerOnReceived();

            _queueConnection.ChannelInfo.BasicConsume("Info", false, consumer);

        }

        private EventHandler<BasicDeliverEventArgs> ConsumerOnReceived()
        {
            return (model, ea) =>
            {
                var elapsedTime = string.Empty;
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var data = new Mongo.Dto.Info();
                try
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var ii = JsonConvert.DeserializeObject<Info>(message);

                    data = _infoGenericRepository.Find(ii.ObjectId);
                    data.DateStartElaboration = DateTime.UtcNow;

                    if (_processor.ProcessData(ii))
                    {
                        data.DateEndElaboration = DateTime.UtcNow;
                        data.ElaborationSuccesfull = true;

                        

                        stopWatch.Stop();
                        var ts = stopWatch.Elapsed;
                        
                        elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}.{ts.Milliseconds:00}";
                        _queueConnection.ChannelInfo.BasicAck(ea.DeliveryTag, false);

                        Log?.Invoke(this, new LoggerEventsQueue
                        {
                            Message = $"Finita elaborazione Info {data.Id.ToString()} - { DateTime.UtcNow:O} tempo trascorso { elapsedTime }",
                            Exception = null,
                            TypeLevel = LogService.TypeLevel.Info,
                            Type = TypeEvent.Info
                        });
                    }
                    else
                    {
                        throw new Exception("Errore elaborazione json senza eccezioni");
                    }
            }
                catch (Exception ex)
                {
                    data.DateEndElaboration = DateTime.UtcNow;
                    data.ElaborationSuccesfull = false;

                    Log?.Invoke(this, new LoggerEventsQueue
                    {
                        Message = $"Finita elaborazione Info {data.Id.ToString()} con errori - {DateTime.UtcNow:O} tempo trascorso {elapsedTime}",
                        Exception = ex,
                        TypeLevel = LogService.TypeLevel.Error,
                        Type = TypeEvent.Info
                    });


                }
                finally
                {
                    _infoGenericRepository.Update(data);
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