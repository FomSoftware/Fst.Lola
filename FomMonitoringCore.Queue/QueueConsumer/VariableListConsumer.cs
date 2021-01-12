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
using VariablesList = FomMonitoringCore.Queue.Dto.VariablesList;

namespace FomMonitoringCore.Queue.QueueConsumer
{
    public class VariableListConsumer : IConsumer<Dto.VariablesList>
    {
        private readonly IProcessor<VariablesList> _processor;
        private readonly IQueueConnection _queueConnection;
        private readonly IGenericRepository<Mongo.Dto.VariablesList> _variableGenericRepository;
        private EventingBasicConsumer consumer;

        public VariableListConsumer(IProcessor<VariablesList> processor, IQueueConnection queueConnection,
            IGenericRepository<Mongo.Dto.VariablesList> variableGenericRepository)
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
                var data = new Mongo.Dto.VariablesList();
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

                        stopWatch.Stop();
                        // Get the elapsed time as a TimeSpan value.
                        var ts = stopWatch.Elapsed;

                        _queueConnection.ChannelVariableList.BasicAck(ea.DeliveryTag, false);
                        // Format and display the TimeSpan value.
                        elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}.{ts.Milliseconds:00}";
                        Log?.Invoke(this, new LoggerEventsQueue
                        {
                            Message = $"Finita elaborazione VariableLists {data.Id} - { DateTime.UtcNow:O} tempo trascorso { elapsedTime }",
                            Exception = null,
                            TypeLevel = LogService.TypeLevel.Info,
                            Type = TypeEvent.Variable
                        });
                    }
                    else
                    {
                        if (data.DateEndElaboration == null)
                        {
                            FailedJsonProcessorNotifier.Notify(data.Id, "VariableList");
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
                        FailedJsonProcessorNotifier.Notify(data.Id, "VariableList");
                    }
                    data.DateEndElaboration = DateTime.UtcNow;
                    data.ElaborationSuccesfull = false;
                    _queueConnection.ChannelHistoryJobPieceBar.BasicAck(ea.DeliveryTag, false);

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
