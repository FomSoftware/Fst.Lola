using System;
using System.Diagnostics;
using System.Text;
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
        private EventingBasicConsumer _consumer;

        public InfoConsumer(IProcessor<Info> processor, 
            IQueueConnection queueConnection)
        {
            _processor = processor;
            _queueConnection = queueConnection;
        }
        

        public event EventHandler<LoggerEventsQueue> Log;

        public void Init()
        {

            _consumer = new EventingBasicConsumer(_queueConnection.ChannelInfo);
            _consumer.Received += ConsumerOnReceived();

            _queueConnection.ChannelInfo.BasicConsume("Info", false, _consumer);

        }

        private EventHandler<BasicDeliverEventArgs> ConsumerOnReceived()
        {
            return (model, ea) =>
            {
                var elapsedTime = string.Empty;
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                try
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var ii = JsonConvert.DeserializeObject<Info>(message);


                    if (_processor.ProcessData(ii))
                    {

                        stopWatch.Stop();
                        var ts = stopWatch.Elapsed;
                        
                        elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}.{ts.Milliseconds:00}";
                        _queueConnection.ChannelInfo.BasicAck(ea.DeliveryTag, false);

                        Log?.Invoke(this, new LoggerEventsQueue
                        {
                            Message = $"Finita elaborazione Info  - { DateTime.UtcNow:O} tempo trascorso { elapsedTime }",
                            Exception = null,
                            TypeLevel = LogService.TypeLevel.Info,
                            Type = TypeEvent.Info
                        });
                    }
                    else
                    {
                        _queueConnection.ChannelInfo.BasicNack(ea.DeliveryTag, false, true);
                        throw new Exception("Errore elaborazione json senza eccezioni");
                    }
                }
                catch (Exception ex)
                {
                    _queueConnection.ChannelInfo.BasicNack(ea.DeliveryTag, false, true);
                    Log?.Invoke(this, new LoggerEventsQueue
                    {
                        Message = $"Finita elaborazione Info con errori - {DateTime.UtcNow:O} tempo trascorso {elapsedTime}",
                        Exception = ex,
                        TypeLevel = LogService.TypeLevel.Error,
                        Type = TypeEvent.Info
                    });


                }
            };
        }

        public void Dispose()
        {
            _consumer.Received -= ConsumerOnReceived();
            _processor?.Dispose();
        }
    }
}