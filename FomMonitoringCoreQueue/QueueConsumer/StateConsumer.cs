using FomMonitoringCore.DalMongoDb;
using FomMonitoringCore.DataProcessing.Dto.Mongo;
using FomMonitoringCore.Repository.MongoDb;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.ProcessData;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FomMonitoringCoreQueue.Events;
using State = FomMonitoringCoreQueue.Dto.State;

namespace FomMonitoringCoreQueue.QueueConsumer
{
    public class StateConsumer : IConsumer<State>
    {
        private readonly IProcessor<State> _processor;
        private readonly IQueueConnection _queueConnection;
        public StateConsumer(IProcessor<State> processor, IQueueConnection queueConnection)
        {
            _processor = processor;
            _queueConnection = queueConnection;
        }

        public event EventHandler<LoggerEventsQueue> Log;

        public void Init()
        {
            var consumer = new EventingBasicConsumer(_queueConnection.Channel);
            consumer.Received += (model, ea) =>
            {
                var elapsedTime = string.Empty;
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                var data = new FomMonitoringCore.DataProcessing.Dto.Mongo.State();
                var mongoContext = new MongoDbContext();
                var repo = new GenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.State>(mongoContext);
                try
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var ii = JsonConvert.DeserializeObject<State>(message);

                    data = repo.Find(ii.ObjectId);
                    data.state.AsParallel().ForAll(vl => vl.DateStartElaboration = DateTime.UtcNow);

                    if (_processor.ProcessData(ii))
                    {

                        data.state.AsParallel().ForAll(vl =>
                        {
                            vl.DateEndElaboration = DateTime.UtcNow;
                            vl.ElaborationSuccesfull = true;
                        });

                    }

                    stopWatch.Stop();
                    // Get the elapsed time as a TimeSpan value.
                    var ts = stopWatch.Elapsed;

                    // Format and display the TimeSpan value.
                    elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}.{ts.Milliseconds:00}";
                    _queueConnection.Channel.BasicAck(ea.DeliveryTag, false);
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
                    data.state?.AsParallel().ForAll(vl =>
                    {
                        vl.DateEndElaboration = DateTime.UtcNow;
                        vl.ElaborationSuccesfull = false;
                    });

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
                    LogService.WriteLog(
                        $"Finita elaborazione json {DateTime.UtcNow:O} tempo trascorso {elapsedTime} ", LogService.TypeLevel.Info);
                    repo.Update(data);
                }
            };

            _queueConnection.Channel.BasicConsume("State", false, consumer);
        }
    }
}