using System;
using System.Configuration;
using System.IO;
using System.Linq;
using FomMonitoringCore.Mongo.Dto;
using FomMonitoringCore.Mongo.Repository;
using FomMonitoringCore.Queue.QueueProducer;
using Newtonsoft.Json;
using NJsonSchema;
using HistoryJobPieceBar = FomMonitoringCore.Queue.Dto.HistoryJobPieceBar;
using Info = FomMonitoringCore.Queue.Dto.Info;
using Message = FomMonitoringCore.Queue.Dto.Message;
using State = FomMonitoringCore.Queue.Dto.State;
using Tool = FomMonitoringCore.Queue.Dto.Tool;
using VariablesList = FomMonitoringCore.Queue.Dto.VariablesList;

namespace FomMonitoringCore.Queue.Forwarder
{
    public interface IQueueForwarder
    {
        bool Forward(string json);
    }

    public class QueueForwarder : IQueueForwarder
    {
        private readonly IProducer<VariablesList> _variablesProducer;
        private readonly IProducer<HistoryJobPieceBar> _historyJobPieceBarProducer;
        private readonly IProducer<Info> _infoProducer;
        private readonly IProducer<Message> _messageProducer;
        private readonly IProducer<State> _stateProducer;
        private readonly IProducer<Tool> _toolProducer;

        private readonly IGenericRepository<Mongo.Dto.HistoryJobPieceBar>
            _historyJobPieceBarGenericRepository;

        private readonly IGenericRepository<Mongo.Dto.Message>
            _messageGenericRepository;

        private readonly IGenericRepository<Mongo.Dto.State>
            _stateGenericRepository;

        private readonly IGenericRepository<Mongo.Dto.Tool>
            _toolGenericRepository;

        private readonly IGenericRepository<Mongo.Dto.VariablesList>
            _variablesGenericRepository;

        private readonly IGenericRepository<Unknown> _unknownGenericRepository;
        private readonly IGenericRepository<Mongo.Dto.Info> _infoGenericRepository;

        public QueueForwarder(IProducer<VariablesList> variablesProducer,
            IProducer<HistoryJobPieceBar> historyJobPieceBarProducer,
            IProducer<Info> infoProducer,
            IProducer<Message> messageProducer,
            IProducer<State> stateProducer,
            IProducer<Tool> toolProducer,
            IGenericRepository<Mongo.Dto.VariablesList> variablesGenericRepository,
            IGenericRepository<Mongo.Dto.HistoryJobPieceBar>
                historyJobPieceBarGenericRepository,
            IGenericRepository<Mongo.Dto.Message> messageGenericRepository,
            IGenericRepository<Mongo.Dto.State> stateGenericRepository,
            IGenericRepository<Mongo.Dto.Tool> toolGenericRepository,
            IGenericRepository<Mongo.Dto.Info> infoGenericRepository,
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

        public bool Forward(string json)
        {
            var pathSchemas = ConfigurationManager.AppSettings["PathSchemaLOLA"];
            var schemaDataVariablesList =
                JsonSchema.FromFileAsync(Path.Combine(pathSchemas, "variablesList.json")).Result;
            var errorDataVariablesList = schemaDataVariablesList.Validate(json);

            if (!errorDataVariablesList.Any())
            {
                var data = JsonConvert
                    .DeserializeObject<Mongo.Dto.VariablesList>(json);

                //controllo delle date obbligatorie
                if (data.info[0].LoginDate <= DateTime.MinValue)
                {
                    return false;
                }
                if (data.variablesList!= null && data.variablesList.Any(v => v.UtcDateTime <= DateTime.MinValue))
                {
                    return false;
                }
                data.DateSendedQueue = DateTime.UtcNow;
                data.DateReceived = DateTime.UtcNow;
                var infoMongo = new Mongo.Dto.Info
                {
                    info = data.info,
                    DateReceived = data.DateReceived,
                    DateSendedQueue = data.DateSendedQueue,
                    IsCumulative = data.IsCumulative
                };

                _infoGenericRepository.Create(infoMongo);

                _infoProducer.Send(new Info
                {
                    ObjectId = infoMongo.Id,
                    InfoMachine = data.info
                });

                _variablesGenericRepository.Create(data);

                
                _variablesProducer.Send(new VariablesList
                {
                    ObjectId = data.Id,
                    InfoMachine = data.info,
                    VariablesListMachine = data.variablesList
                });

                return true;
            }


            var schemaDataMessages = JsonSchema.FromFileAsync(Path.Combine(pathSchemas, "messages.json")).Result;
            var errorMessages = schemaDataMessages.Validate(json);


            if (!errorMessages.Any())
            {
                var data = JsonConvert.DeserializeObject<Mongo.Dto.Message>(json);
                //controllo delle date obbligatorie
                if (data.info[0].LoginDate <= DateTime.MinValue)
                {
                    return false;
                }

                if(data.message != null && data.message.Any(m => m.Time <= DateTime.MinValue))
                {
                    return false;
                }
                data.DateSendedQueue = DateTime.UtcNow;
                data.DateReceived = DateTime.UtcNow;
                var infoMongo = new Mongo.Dto.Info
                {
                    info = data.info,
                    DateReceived = data.DateReceived,
                    DateSendedQueue = data.DateSendedQueue,
                    IsCumulative = data.IsCumulative
                };

                _infoGenericRepository.Create(infoMongo);

                _infoProducer.Send(new Info
                {
                    ObjectId = infoMongo.Id,
                    InfoMachine = data.info
                });

                _messageGenericRepository.Create(data);

                _messageProducer.Send(new Message
                {
                    ObjectId = data.Id,
                    InfoMachine = data.info,
                    MessageMachine = data.message
                });

                return true;
            }


            var schemaDataState = JsonSchema.FromFileAsync(Path.Combine(pathSchemas, "state.json")).Result;
            var errorStates = schemaDataState.Validate(json);


            if (!errorStates.Any())
            {
                var data = JsonConvert.DeserializeObject<Mongo.Dto.State>(json);
                //controllo delle date obbligatorie
                if (data.info[0].LoginDate <= DateTime.MinValue)
                {
                    return false;
                }

                if (data.state != null && data.state.Any(m => m.StartTime <= DateTime.MinValue ||
                                                              m.EndTime <= DateTime.MinValue))
                {
                    return false;
                }
                data.DateSendedQueue = DateTime.UtcNow;
                data.DateReceived = DateTime.UtcNow;
                var infoMongo = new Mongo.Dto.Info
                {
                    info = data.info,
                    DateReceived = data.DateReceived,
                    DateSendedQueue = data.DateSendedQueue,
                    IsCumulative = data.IsCumulative
                };

                _infoGenericRepository.Create(infoMongo);

                _infoProducer.Send(new Info
                {
                    ObjectId = infoMongo.Id,
                    InfoMachine = data.info
                });
                
                _stateGenericRepository.Create(data);
                data.DateReceived = DateTime.UtcNow;

                _stateProducer.Send(new State
                {
                    ObjectId = data.Id,
                    InfoMachine = data.info,
                    StateMachine = data.state
                });

                return true;
            }


            var schemaDataTool = JsonSchema.FromFileAsync(Path.Combine(pathSchemas, "tool.json")).Result;
            var errorTool = schemaDataTool.Validate(json);


            if (!errorTool.Any())
            {
                var data = JsonConvert.DeserializeObject<Mongo.Dto.Tool>(json);
                if (data.info[0].LoginDate <= DateTime.MinValue)
                {
                    return false;
                }

                if (data.tool != null && data.tool.Any(m => m.DateLoaded <= DateTime.MinValue ||
                                                            m.DateReplaced <= DateTime.MinValue))
                {
                    return false;
                }

                data.DateSendedQueue = DateTime.UtcNow;
                data.DateReceived = DateTime.UtcNow;
                var infoMongo = new Mongo.Dto.Info
                {
                    info = data.info,
                    DateReceived = data.DateReceived,
                    DateSendedQueue = data.DateSendedQueue,
                    IsCumulative = data.IsCumulative
                };

                _infoGenericRepository.Create(infoMongo);

                _infoProducer.Send(new Info
                {
                    ObjectId = infoMongo.Id,
                    InfoMachine = data.info
                });

                _toolGenericRepository.Create(data);

                data.DateReceived = DateTime.UtcNow;

                _toolProducer.Send(new Tool
                {
                    ObjectId = data.Id,
                    InfoMachine = data.info,
                    ToolMachine = data.tool
                });
                
                return true;
            }


            var schemaDataHistoryJobPieceBar =
                JsonSchema.FromFileAsync(Path.Combine(pathSchemas, "historyJobPieceBar.json")).Result;
            var errorHistoryJobPieceBar = schemaDataHistoryJobPieceBar.Validate(json);


            if (!errorHistoryJobPieceBar.Any())
            {
                var data = JsonConvert
                    .DeserializeObject<Mongo.Dto.HistoryJobPieceBar>(json);
                if (data.info[0].LoginDate <= DateTime.MinValue)
                {
                    return false;
                }
                if (data.bar != null && data.bar.Any(m => m.StartTime <= DateTime.MinValue))
                {
                    return false;
                }

                if (data.piece != null && data.piece.Any(m => m.StartTime <= DateTime.MinValue ||
                                                          m.EndTime <= DateTime.MinValue))
                {
                    return false;
                }

                if (data.historyjob != null && data.historyjob.Any(m => m.Day <= DateTime.MinValue))
                {
                    return false;
                }

                data.DateSendedQueue = DateTime.UtcNow;
                data.DateReceived = DateTime.UtcNow;
                var infoMongo = new Mongo.Dto.Info
                {
                    info = data.info,
                    DateReceived = data.DateReceived,
                    DateSendedQueue = data.DateSendedQueue,
                    IsCumulative = data.IsCumulative
                };

                _infoGenericRepository.Create(infoMongo);

                _infoProducer.Send(new Info
                {
                    ObjectId = infoMongo.Id,
                    InfoMachine = data.info
                });

                _historyJobPieceBarGenericRepository.Create(data);

                _historyJobPieceBarProducer.Send(new HistoryJobPieceBar
                {
                    ObjectId = data.Id,
                    InfoMachine = data.info,
                    HistoryJobMachine = data.historyjob,
                    PieceMachine = data.piece,
                    BarMachine = data.bar
                });
                
                return true;
            }

            var dataUnknown = JsonConvert.DeserializeObject<BaseModel>(json);
            var en = new Unknown(dataUnknown)
            {
                EntityUnknown = json,
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
            return true;
        }
         
    }
}