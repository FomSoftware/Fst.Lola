using System;
using System.Diagnostics;
using System.Text;
using FomMonitoringCore.Mongo.Repository;
using FomMonitoringCore.Queue.Connection;
using FomMonitoringCore.Queue.Events;
using FomMonitoringCore.Queue.Notifier;
using FomMonitoringCore.Queue.ProcessData;
using FomMonitoringCore.Service;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using State = FomMonitoringCore.Queue.Dto.State;

namespace FomMonitoringCore.Queue.QueueConsumer
{
    public class StateConsumer : IConsumer<State>
    {
        private readonly IProcessor<State> _processor;
        private readonly IQueueConnection _queueConnection;
        private readonly IGenericRepository<Mongo.Dto.State> _stateGenericRepository;
        private EventingBasicConsumer _consumer;

        public StateConsumer(IProcessor<State> processor, IQueueConnection queueConnection,
            IGenericRepository<Mongo.Dto.State> stateGenericRepository)
        {
            _processor = processor;
            _queueConnection = queueConnection;
            _stateGenericRepository = stateGenericRepository;
        }

        public event EventHandler<LoggerEventsQueue> Log;

        public void Init()
        {
            _consumer = new EventingBasicConsumer(_queueConnection.ChannelState);
            _consumer.Received += ConsumerOnReceived();

            _queueConnection.ChannelState.BasicConsume("State", false, _consumer);
        }

        public void Dispose()
        {
            _consumer.Received -= ConsumerOnReceived();
            _processor?.Dispose();
        }

        private EventHandler<BasicDeliverEventArgs> ConsumerOnReceived()
        {
            return (model, ea) =>
            {
                var elapsedTime = string.Empty;
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                var data = new Mongo.Dto.State();
                try
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var ii = JsonConvert.DeserializeObject<State>(message);

                    data = _stateGenericRepository.Find(ii.ObjectId);
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
                        _queueConnection.ChannelState.BasicAck(ea.DeliveryTag, false);
                        Log?.Invoke(this, new LoggerEventsQueue
                        {
                            Message = $"Finita elaborazione State {data.Id.ToString()} - { DateTime.UtcNow:O} tempo trascorso { elapsedTime }",
                            Exception = null,
                            TypeLevel = LogService.TypeLevel.Info,
                            Type = TypeEvent.State
                        });
                    }
                    else
                    {
                        if (data.DateEndElaboration == null)
                        {
                            FailedJsonProcessorNotifier.Notify(data.Id, "State");
                        }
                        data.DateEndElaboration = DateTime.UtcNow;
                        data.ElaborationSuccesfull = false;
                        _queueConnection.ChannelHistoryJobPieceBar.BasicAck(ea.DeliveryTag, false);
                        throw new Exception("Errore elaborazione json senza eccezioni");
                    }
                }
                catch (Exception ex)
                {
                    if (data.DateEndElaboration == null)
                    {
                        FailedJsonProcessorNotifier.Notify(data.Id, "State");
                    }
                    data.DateEndElaboration = DateTime.UtcNow;
                    data.ElaborationSuccesfull = false;
                    _queueConnection.ChannelHistoryJobPieceBar.BasicAck(ea.DeliveryTag, false);

                    Log?.Invoke(this, new LoggerEventsQueue
                    {
                        Message = $"Finita elaborazione  State {data.Id} con errori - {DateTime.UtcNow:O} tempo trascorso {elapsedTime}",
                        Exception = ex,
                        TypeLevel = LogService.TypeLevel.Error,
                        Type = TypeEvent.Info
                    });
                }
                finally
                {
                    LogService.WriteLog(
                        $"Finita elaborazione json {DateTime.UtcNow:O} tempo trascorso {elapsedTime} ", LogService.TypeLevel.Info);
                    _stateGenericRepository.Update(data);
                }
            };
        }
    }
}