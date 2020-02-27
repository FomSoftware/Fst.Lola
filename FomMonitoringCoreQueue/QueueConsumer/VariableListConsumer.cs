using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FomMonitoringCore.DalMongoDb;
using FomMonitoringCore.DataProcessing.Dto.Mongo;
using FomMonitoringCore.Repository.MongoDb;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.Events;
using FomMonitoringCoreQueue.ProcessData;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VariablesList = FomMonitoringCoreQueue.Dto.VariablesList;

namespace FomMonitoringCoreQueue.QueueConsumer
{
    public class VariableListConsumer : IConsumer<VariablesList>
    {
        private readonly IProcessor<VariablesList> _processor;
        private readonly IQueueConnection _queueConnection;
        private IMongoDbContext _mongoContext;
        private readonly IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.VariablesList> _variableGenericRepository;

        public VariableListConsumer(IProcessor<VariablesList> processor, IQueueConnection queueConnection, IMongoDbContext mongoContext,
            IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.VariablesList> variableGenericRepository)
        {
            _processor = processor;
            _queueConnection = queueConnection;
            _mongoContext = mongoContext;
            _variableGenericRepository = variableGenericRepository;
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
                    var data = new FomMonitoringCore.DataProcessing.Dto.Mongo.VariablesList();
                    try
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var ii = JsonConvert.DeserializeObject<VariablesList>(message);
                        
                        data = _variableGenericRepository.Find(ii.ObjectId);
                        data.variablesList.AsParallel().ForAll(vl => vl.DateStartElaboration = DateTime.UtcNow);

                        if (_processor.ProcessData(ii))
                        {

                            data.variablesList.AsParallel().ForAll(vl =>
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
                            Type = TypeEvent.Variable
                        });
                    }
                    catch (Exception ex)
                    {
                        data.variablesList.AsParallel().ForAll(vl =>
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
                        _variableGenericRepository.Update(data);
                    }
                };

                _queueConnection.Channel.BasicConsume("VariableList",false, consumer);
                
        }
    }


}
