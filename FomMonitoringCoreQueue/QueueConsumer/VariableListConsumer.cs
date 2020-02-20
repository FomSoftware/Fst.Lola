using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FomMonitoringCore.DalMongoDb;
using FomMonitoringCore.DataProcessing.Dto.Mongo;
using FomMonitoringCore.Repository.MongoDb;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.ProcessData;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FomMonitoringCoreQueue.QueueConsumer
{
    public class VariableListConsumer : IVariableListConsumer
    {
        private readonly IVariableListProcessor _processor;
        private readonly IQueueConnection _queueConnection;
        public VariableListConsumer(IVariableListProcessor processor, IQueueConnection queueConnection)
        {
            _processor = processor;
            _queueConnection = queueConnection;
        }

        public void Init()
        {

            var consumer = new EventingBasicConsumer(_queueConnection.Channel);
                consumer.Received += (model, ea) =>
                {

                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    LogService.WriteLog("arrivato json " + DateTime.UtcNow.ToString("O"), LogService.TypeLevel.Info);
                    var data = new MachineDataModel();
                    var mongoContext = new MongoDbContext();
                    var repo = new GenericRepository<MachineDataModel>(mongoContext);
                    try
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var ii = JsonConvert.DeserializeObject<Dto.VariablesList>(message);


                    data = repo.Find(ii.ObjectId);
                    data.variablesList.AsParallel().ForAll(vl => vl.DateStartElaboration = DateTime.UtcNow);
                    foreach (var variablesList in data.variablesList)
                    {
                        variablesList.DateStartElaboration = DateTime.UtcNow;
                    }

                        if (_processor.ProcessData(ii))
                        {

                            data.variablesList.AsParallel().ForAll(vl =>
                            {
                                vl.DateEndElaboration = DateTime.UtcNow;
                                vl.ElaborationSuccesfull = true;
                            });

                        }
                    }
                    catch (Exception ex)
                    {
                        data.variablesList.AsParallel().ForAll(vl =>
                        {
                            vl.DateEndElaboration = DateTime.UtcNow;
                            vl.ElaborationSuccesfull = false;
                        });
                    }
                    finally
                    {
                        stopWatch.Stop();
                        // Get the elapsed time as a TimeSpan value.
                        var ts = stopWatch.Elapsed;

                        // Format and display the TimeSpan value.
                        var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}.{4:00}",
                            ts.Hours, ts.Minutes, ts.Seconds,
                            ts.Milliseconds / 10, ts.Milliseconds);

                        LogService.WriteLog("finita elaborazione json " + DateTime.UtcNow.ToString("O") + " tempo trascorso " + elapsedTime + " "+ DateTime.UtcNow.ToString("O"), LogService.TypeLevel.Info);
                        repo.Update(data);
                    }
                };

                _queueConnection.Channel.BasicConsume(queue: "VariableList",
                    autoAck: true,
                    consumer: consumer);
                
            
        }
    }
}
