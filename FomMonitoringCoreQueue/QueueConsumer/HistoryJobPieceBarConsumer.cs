using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FomMonitoringCore.Repository.MongoDb;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.ProcessData;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FomMonitoringCoreQueue.QueueConsumer
{
    public class HistoryJobPieceBarConsumer : IConsumer<HistoryJobPieceBar>
    {
        private readonly IProcessor<HistoryJobPieceBar> _processor;
        private readonly IQueueConnection _queueConnection;
        private readonly IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.HistoryJobPieceBar> _messageGenericRepository;

        public HistoryJobPieceBarConsumer(IProcessor<HistoryJobPieceBar> processor,
            IQueueConnection queueConnection,
            IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.HistoryJobPieceBar> messageGenericRepository)
        {
            _processor = processor;
            _queueConnection = queueConnection;
            _messageGenericRepository = messageGenericRepository;
        }

        public void Init()
        {
            var consumer = new EventingBasicConsumer(_queueConnection.Channel);
            consumer.Received += (model, ea) =>
            {
                var elapsedTime = string.Empty;
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var data = new FomMonitoringCore.DataProcessing.Dto.Mongo.HistoryJobPieceBar();
                try
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var ii = JsonConvert.DeserializeObject<HistoryJobPieceBar>(message);

                    data = _messageGenericRepository.Find(ii.ObjectId);
                    data.historyjob.AsParallel().ForAll(vl => vl.DateStartElaboration = DateTime.UtcNow);
                    data.bar.AsParallel().ForAll(vl => vl.DateStartElaboration = DateTime.UtcNow);
                    data.piece.AsParallel().ForAll(vl => vl.DateStartElaboration = DateTime.UtcNow);

                    if (_processor.ProcessData(ii))
                    {

                        data.bar.AsParallel().ForAll(vl =>
                        {
                            vl.DateEndElaboration = DateTime.UtcNow;
                            vl.ElaborationSuccesfull = true;
                        });
                        data.piece.AsParallel().ForAll(vl =>
                        {
                            vl.DateEndElaboration = DateTime.UtcNow;
                            vl.ElaborationSuccesfull = true;
                        });
                        data.historyjob.AsParallel().ForAll(vl =>
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
                }
                catch (Exception ex)
                {
                    data.info.AsParallel().ForAll(vl =>
                    {
                        vl.DateEndElaboration = DateTime.UtcNow;
                        vl.ElaborationSuccesfull = false;
                    });
                    LogService.WriteLog(
                        $"Finita elaborazione con errori json {DateTime.UtcNow:O} tempo trascorso {elapsedTime} ", LogService.TypeLevel.Error, ex);

                }
                finally
                {
                    LogService.WriteLog(
                        $"Finita elaborazione json {DateTime.UtcNow:O} tempo trascorso {elapsedTime} ", LogService.TypeLevel.Info);
                    _messageGenericRepository.Update(data);
                }
            };

            _queueConnection.Channel.BasicConsume("Messages", false, consumer);
        }

    }
}