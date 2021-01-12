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
using Tool = FomMonitoringCore.Queue.Dto.Tool;

namespace FomMonitoringCore.Queue.QueueConsumer
{
    public class ToolConsumer : IConsumer<Tool>
    {
        private readonly IProcessor<Tool> _processor;
        private readonly IQueueConnection _queueConnection;
        private readonly IGenericRepository<Mongo.Dto.Tool> _toolGenericRepository;
        private EventingBasicConsumer _consumer;

        public ToolConsumer(IProcessor<Tool> processor, IQueueConnection queueConnection,
            IGenericRepository<Mongo.Dto.Tool> toolGenericRepository)
        {
            _processor = processor;
            _queueConnection = queueConnection;
            _toolGenericRepository = toolGenericRepository;
        }

        public event EventHandler<LoggerEventsQueue> Log;

        public void Init()
        {
            _consumer = new EventingBasicConsumer(_queueConnection.ChannelState);
            _consumer.Received += ConsumerOnReceived();

            _queueConnection.ChannelTool.BasicConsume("Tool", false, _consumer);
        }

        private EventHandler<BasicDeliverEventArgs> ConsumerOnReceived()
        {
            return (model, ea) =>
            {
                var elapsedTime = string.Empty;
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                var data = new Mongo.Dto.Tool();
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

                        stopWatch.Stop();
                        // Get the elapsed time as a TimeSpan value.
                        var ts = stopWatch.Elapsed;

                        // Format and display the TimeSpan value.
                        elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}.{ts.Milliseconds:00}";
                        _queueConnection.ChannelTool.BasicAck(ea.DeliveryTag, false);
                        Log?.Invoke(this, new LoggerEventsQueue
                        {
                            Message = $"Finita elaborazione Tool {data.Id.ToString()} - { DateTime.UtcNow:O} tempo trascorso { elapsedTime }",
                            Exception = null,
                            TypeLevel = LogService.TypeLevel.Info,
                            Type = TypeEvent.Tool
                        });
                    }
                    else
                    {
                        if (data.DateEndElaboration == null)
                        {
                            FailedJsonProcessorNotifier.Notify(data.Id, "Tool");
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
                        FailedJsonProcessorNotifier.Notify(data.Id, "Tool");
                    }
                    data.DateEndElaboration = DateTime.UtcNow;
                    data.ElaborationSuccesfull = false;
                    _queueConnection.ChannelHistoryJobPieceBar.BasicAck(ea.DeliveryTag, false);
                    LogService.WriteLog(
                        $"Finita elaborazione con errori json tool {DateTime.UtcNow:O} tempo trascorso {elapsedTime} ", LogService.TypeLevel.Error, ex);

                }
                finally
                {
                    _toolGenericRepository.Update(data);
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