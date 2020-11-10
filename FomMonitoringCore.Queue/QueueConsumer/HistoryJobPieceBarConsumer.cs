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
    public class HistoryJobPieceBarConsumer : IConsumer<HistoryJobPieceBar>
    {
        private readonly IProcessor<HistoryJobPieceBar> _processor;
        private readonly IQueueConnection _queueConnection;
        private readonly IGenericRepository<Mongo.Dto.HistoryJobPieceBar> _messageGenericRepository;
        private EventingBasicConsumer consumer;

        public HistoryJobPieceBarConsumer(
            IProcessor<HistoryJobPieceBar> processor,
            IQueueConnection queueConnection,
            IGenericRepository<Mongo.Dto.HistoryJobPieceBar> messageGenericRepository)
        {
            _processor = processor;
            _queueConnection = queueConnection;
            _messageGenericRepository = messageGenericRepository;
        }
        

        public event EventHandler<LoggerEventsQueue> Log;

        public void Init()
        {
            consumer = new EventingBasicConsumer(_queueConnection.ChannelHistoryJobPieceBar);
            consumer.Received += ConsumerOnReceived();

            _queueConnection.ChannelHistoryJobPieceBar.BasicConsume("HistoryJobPieceBar", false, consumer);
        }

        private EventHandler<BasicDeliverEventArgs> ConsumerOnReceived()
        {
            return (model, ea) =>
            {
                var elapsedTime = string.Empty;
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var data = new Mongo.Dto.HistoryJobPieceBar();
                try
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var ii = JsonConvert.DeserializeObject<HistoryJobPieceBar>(message);

                    data = _messageGenericRepository.Find(ii.ObjectId);
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
                        _queueConnection.ChannelHistoryJobPieceBar.BasicAck(ea.DeliveryTag, false);
                        Log?.Invoke(this, new LoggerEventsQueue
                        {
                            Message = $"Finita elaborazione HistoryBarJobPiece {data.Id.ToString()} - { DateTime.UtcNow:O} tempo trascorso { elapsedTime }",
                            Exception = null,
                            TypeLevel = LogService.TypeLevel.Info,
                            Type = TypeEvent.HistoryBarJobPiece
                        });
                    }
                    else
                    {
                        _queueConnection.ChannelHistoryJobPieceBar.BasicNack(ea.DeliveryTag, false, true);
                        throw new Exception("Errore elaborazione json senza eccezioni");
                    }
                }
                catch (Exception ex)
                {
                    data.DateEndElaboration = DateTime.UtcNow;
                    data.ElaborationSuccesfull = false;

                    _queueConnection.ChannelHistoryJobPieceBar.BasicNack(ea.DeliveryTag, false, true);
                    Log?.Invoke(this, new LoggerEventsQueue
                    {
                        Message = $"Finita elaborazione HistoryBarJobPiece {data.Id} con errori - {DateTime.UtcNow:O} tempo trascorso {elapsedTime}",
                        Exception = ex,
                        TypeLevel = LogService.TypeLevel.Error,
                        Type = TypeEvent.HistoryBarJobPiece
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