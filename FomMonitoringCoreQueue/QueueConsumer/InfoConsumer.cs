using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FomMonitoringCore.DalMongoDb;
using FomMonitoringCore.Repository.MongoDb;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Events;
using FomMonitoringCoreQueue.ProcessData;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Info = FomMonitoringCoreQueue.Dto.Info;

namespace FomMonitoringCoreQueue.QueueConsumer
{
    public class InfoConsumer: IConsumer<Info>
    {
        private readonly IProcessor<Info> _processor;
        private readonly IQueueConnection _queueConnection;
        private readonly IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.Info> _infoGenericRepository;
        private EventingBasicConsumer consumer;

        public InfoConsumer(IProcessor<Info> processor, 
            IQueueConnection queueConnection, 
            IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.Info> infoGenericRepository)
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
                var data = new FomMonitoringCore.DataProcessing.Dto.Mongo.Info();
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

                    }

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