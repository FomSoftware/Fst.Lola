using System;
using System.Configuration;
using System.IO;
using System.Linq;
using FomMonitoringCore.DataProcessing.Dto.Mongo;
using FomMonitoringCore.Repository.MongoDb;
using FomMonitoringCoreQueue.QueueProducer;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using NJsonSchema;
using HistoryJobPieceBar = FomMonitoringCoreQueue.Dto.HistoryJobPieceBar;
using Info = FomMonitoringCoreQueue.Dto.Info;
using Message = FomMonitoringCoreQueue.Dto.Message;
using State = FomMonitoringCoreQueue.Dto.State;
using Tool = FomMonitoringCoreQueue.Dto.Tool;
using VariablesList = FomMonitoringCoreQueue.Dto.VariablesList;

namespace FomMonitoringCoreQueue.Forwarder
{
    public interface IQueueForwarder
    {
        void Forward(string json);
    }

    public class QueueForwarder : IQueueForwarder
    {
        private readonly IProducer<VariablesList> _variablesProducer;
        private readonly IProducer<HistoryJobPieceBar> _historyJobPieceBarProducer;
        private readonly IProducer<Info> _infoProducer;
        private readonly IProducer<Message> _messageProducer;
        private readonly IProducer<State> _stateProducer;
        private readonly IProducer<Tool> _toolProducer;

        private readonly IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.HistoryJobPieceBar>
            _historyJobPieceBarGenericRepository;

        private readonly IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.Message>
            _messageGenericRepository;

        private readonly IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.State>
            _stateGenericRepository;

        private readonly IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.Tool>
            _toolGenericRepository;

        private readonly IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.VariablesList>
            _variablesGenericRepository;

        private readonly IGenericRepository<Unknown> _unknownGenericRepository;
        private readonly IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.Info> _infoGenericRepository;

        public QueueForwarder(IProducer<VariablesList> variablesProducer,
            IProducer<HistoryJobPieceBar> historyJobPieceBarProducer,
            IProducer<Info> infoProducer,
            IProducer<Message> messageProducer,
            IProducer<State> stateProducer,
            IProducer<Tool> toolProducer,
            IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.VariablesList> variablesGenericRepository,
            IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.HistoryJobPieceBar>
                historyJobPieceBarGenericRepository,
            IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.Message> messageGenericRepository,
            IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.State> stateGenericRepository,
            IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.Tool> toolGenericRepository,
            IGenericRepository<FomMonitoringCore.DataProcessing.Dto.Mongo.Info> infoGenericRepository,
            IGenericRepository<Unknown> unknownGenericRepository
        )
        {
            _variablesProducer = variablesProducer;
            _historyJobPieceBarProducer = historyJobPieceBarProducer;
            _infoProducer = infoProducer;
            _messageProducer = messageProducer;
            _stateProducer = stateProducer;
            _toolProducer = toolProducer;

            _historyJobPieceBarGenericRepository = historyJobPieceBarGenericRepository;
            _messageGenericRepository = messageGenericRepository;
            _stateGenericRepository = stateGenericRepository;
            _toolGenericRepository = toolGenericRepository;
            _variablesGenericRepository = variablesGenericRepository;
            _unknownGenericRepository = unknownGenericRepository;
            _infoGenericRepository = infoGenericRepository;
        }

        public void Forward(string json)
        {
            var pathSchemas = ConfigurationManager.AppSettings["PathSchemaLOLA"];


            var schemaDataVariablesList =
                JsonSchema.FromFileAsync(Path.Combine(pathSchemas, "variablesList.json")).Result;
            var errorDataVariablesList = schemaDataVariablesList.Validate(json);


            if (!errorDataVariablesList.Any())
            {
                var data = JsonConvert
                    .DeserializeObject<FomMonitoringCore.DataProcessing.Dto.Mongo.VariablesList>(json);

                _variablesGenericRepository.Create(data);

                _infoGenericRepository.Create(new FomMonitoringCore.DataProcessing.Dto.Mongo.Info
                {
                    info = data.info,
                    DateReceived = data.DateReceived,
                    IsCumulative = data.IsCumulative
                });

                _infoProducer.Send(new Info
                {
                    ObjectId = data.Id.ToString(),
                    InfoMachine = data.info
                });


                _variablesProducer.Send(new VariablesList
                {
                    ObjectId = data.Id.ToString(),
                    InfoMachine = data.info,
                    VariablesListMachine = data.variablesList
                });


                _variablesGenericRepository.Update(data);
                return;
            }


            var schemaDataMessages = JsonSchema.FromFileAsync(Path.Combine(pathSchemas, "messages.json")).Result;
            var errorMessages = schemaDataMessages.Validate(json);


            if (!errorMessages.Any())
            {
                var data = JsonConvert.DeserializeObject<FomMonitoringCore.DataProcessing.Dto.Mongo.Message>(json);
                _messageGenericRepository.Create(data);

                _infoGenericRepository.Create(new FomMonitoringCore.DataProcessing.Dto.Mongo.Info
                {
                    info = data.info,
                    DateReceived = data.DateReceived,
                    IsCumulative = data.IsCumulative
                });


                _infoProducer.Send(new Info
                {
                    ObjectId = data.Id.ToString(),
                    InfoMachine = data.info
                });


                _messageProducer.Send(new Message
                {
                    ObjectId = data.Id.ToString(),
                    InfoMachine = data.info,
                    MessageMachine = data.messages
                });


                _messageGenericRepository.Update(data);
                return;
            }


            var schemaDataState = JsonSchema.FromFileAsync(Path.Combine(pathSchemas, "state.json")).Result;
            var errorStates = schemaDataState.Validate(json);


            if (!errorStates.Any())
            {
                var data = JsonConvert.DeserializeObject<FomMonitoringCore.DataProcessing.Dto.Mongo.State>(json);
                _stateGenericRepository.Create(data);
                data.DateReceived = DateTime.UtcNow;

                _infoGenericRepository.Create(new FomMonitoringCore.DataProcessing.Dto.Mongo.Info
                {
                    info = data.info,
                    DateReceived = data.DateReceived,
                    IsCumulative = data.IsCumulative
                });

                _infoProducer.Send(new Info
                {
                    ObjectId = data.Id.ToString(),
                    InfoMachine = data.info
                });

                _stateProducer.Send(new State
                {
                    ObjectId = data.Id.ToString(),
                    InfoMachine = data.info,
                    StateMachine = data.state
                });


                _stateGenericRepository.Update(data);
                return;
            }


            var schemaDataTool = JsonSchema.FromFileAsync(Path.Combine(pathSchemas, "tool.json")).Result;
            var errorTool = schemaDataTool.Validate(json);


            if (!errorTool.Any())
            {
                var data = JsonConvert.DeserializeObject<FomMonitoringCore.DataProcessing.Dto.Mongo.Tool>(json);
                _toolGenericRepository.Create(data);
                data.DateReceived = DateTime.UtcNow;

                _infoGenericRepository.Create(new FomMonitoringCore.DataProcessing.Dto.Mongo.Info
                {
                    info = data.info,
                    DateReceived = data.DateReceived,
                    IsCumulative = data.IsCumulative
                });

                _infoProducer.Send(new Info
                {
                    ObjectId = data.Id.ToString(),
                    InfoMachine = data.info
                });

                _toolProducer.Send(new Tool
                {
                    ObjectId = data.Id.ToString(),
                    InfoMachine = data.info,
                    ToolMachine = data.tool
                });


                _toolGenericRepository.Update(data);
                return;
            }


            var schemaDataHistoryJobPieceBar =
                JsonSchema.FromFileAsync(Path.Combine(pathSchemas, "historyJobPieceBar.json")).Result;
            var errorHistoryJobPieceBar = schemaDataHistoryJobPieceBar.Validate(json);


            if (!errorHistoryJobPieceBar.Any())
            {
                var data = JsonConvert
                    .DeserializeObject<FomMonitoringCore.DataProcessing.Dto.Mongo.HistoryJobPieceBar>(json);
                _historyJobPieceBarGenericRepository.Create(data);

                _infoGenericRepository.Create(new FomMonitoringCore.DataProcessing.Dto.Mongo.Info
                {
                    info = data.info,
                    DateReceived = data.DateReceived,
                    IsCumulative = data.IsCumulative
                });

                _infoProducer.Send(new Info
                {
                    ObjectId = data.Id.ToString(),
                    InfoMachine = data.info
                });

                _historyJobPieceBarProducer.Send(new HistoryJobPieceBar
                {
                    ObjectId = data.Id.ToString(),
                    InfoMachine = data.info,
                    HistoryJobMachine = data.historyjob
                });


                _historyJobPieceBarGenericRepository.Update(data);
                return;
            }

            var dataUnknown = JsonConvert.DeserializeObject<BaseModel>(json);
            var en = new Unknown(dataUnknown)
            {
                EntityUnknown = BsonSerializer.Deserialize<BsonDocument>(json),
                ErrorDataVariablesList = errorDataVariablesList.ToDictionary(mc => mc.Path,
                    mc => mc.Kind.ToString(),
                    StringComparer.OrdinalIgnoreCase),
                ErrorMessages = errorMessages.ToDictionary(mc => mc.Path,
                    mc => mc.Kind.ToString(),
                    StringComparer.OrdinalIgnoreCase),
                ErrorHistoryJobPieceBar = errorHistoryJobPieceBar.ToDictionary(mc => mc.Path,
                    mc => mc.Kind.ToString(),
                    StringComparer.OrdinalIgnoreCase),
                ErrorStates = errorStates.ToDictionary(mc => mc.Path,
                    mc => mc.Kind.ToString(),
                    StringComparer.OrdinalIgnoreCase),
                ErrorTool = errorTool.ToDictionary(mc => mc.Path,
                    mc => mc.Kind.ToString(),
                    StringComparer.OrdinalIgnoreCase)
            };
            _unknownGenericRepository.Create(en);
        }
    }
}