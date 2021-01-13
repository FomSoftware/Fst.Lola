using System.Linq;
using FomMonitoringCore.Mongo.Dto;
using FomMonitoringCore.Mongo.Repository;

namespace FomMonitoringCore.Queue.Forwarder
{
    public class ReForwarder : IReForwarder
    {
        private readonly IGenericRepository<Unknown> _unknownGenericRepository;
        private readonly IQueueForwarder _queueForwarder;
        private readonly IGenericRepository<HistoryJobPieceBar> _historyJobPieceBarGenericRepository;
        private readonly IGenericRepository<State> _stateGenericRepository;
        private readonly IGenericRepository<Message> _messageGenericRepository;
        private readonly IGenericRepository<Tool> _toolGenericRepository;
        private readonly IGenericRepository<VariablesList> _variableListGenericRepository;

        public ReForwarder(
            IGenericRepository<Unknown> unknownGenericRepository,
            IGenericRepository<HistoryJobPieceBar> historyJobPieceBarGenericRepository,
            IGenericRepository<State> stateGenericRepository,
            IGenericRepository<Message> messageGenericRepository,
            IGenericRepository<Tool> toolGenericRepository,
            IGenericRepository<VariablesList> variableListGenericRepository,
            IQueueForwarder queueForwarder)
        {
            _unknownGenericRepository = unknownGenericRepository;
            _queueForwarder = queueForwarder;
            _historyJobPieceBarGenericRepository = historyJobPieceBarGenericRepository;
            _stateGenericRepository = stateGenericRepository;
            _messageGenericRepository = messageGenericRepository;
            _toolGenericRepository = toolGenericRepository;
            _variableListGenericRepository = variableListGenericRepository;
        }

        public void ReForward()
        {
            ReForwardUnknown();

            ReForwardUnSuccess();
        }

        private void ReForwardUnknown()
        {
            var unknownList = _unknownGenericRepository.Query().ToList();
            foreach (var unknownJson in unknownList)
            {
                _queueForwarder.Forward(unknownJson.EntityUnknown);
                _unknownGenericRepository.Delete(unknownJson.Id);
            }
        }

        private void ReForwardUnSuccess()
        {
            ReForwardHistoryJobPieceBar();
            ReForwardMessages();
            ReForwardStates();
            ReForwardTools();
            ReForwardVariableList();
        }

        private void ReForwardHistoryJobPieceBar()
        {
            var unSuccessJson = _historyJobPieceBarGenericRepository.Query(n => n.ElaborationSuccesfull == false)
                .ToList();

            foreach (var json in unSuccessJson.ToList())
            {
                _queueForwarder.Forward(Newtonsoft.Json.JsonConvert.SerializeObject(json), false);
            }
        }

        private void ReForwardMessages()
        {
            var unSuccessJson = _messageGenericRepository.Query(n => n.ElaborationSuccesfull == false)
                .ToList();

            foreach (var json in unSuccessJson.ToList())
            {
                _queueForwarder.Forward(Newtonsoft.Json.JsonConvert.SerializeObject(json), false);
            }
        }

        private void ReForwardStates()
        {
            var unSuccessJson = _stateGenericRepository.Query(n => n.ElaborationSuccesfull == false)
                .ToList();

            foreach (var json in unSuccessJson.ToList())
            {
                _queueForwarder.Forward(Newtonsoft.Json.JsonConvert.SerializeObject(json), false);
            }
        }

        private void ReForwardTools()
        {
            var unSuccessJson = _toolGenericRepository.Query(n => n.ElaborationSuccesfull == false)
                .ToList();

            foreach (var json in unSuccessJson.ToList())
            {
                _queueForwarder.Forward(Newtonsoft.Json.JsonConvert.SerializeObject(json), false);
            }

        }

        private void ReForwardVariableList()
        {
            var unSuccessJson = _variableListGenericRepository.Query(n => n.ElaborationSuccesfull == false)
                .ToList();

            foreach (var json in unSuccessJson.ToList())
            {
                _queueForwarder.Forward(Newtonsoft.Json.JsonConvert.SerializeObject(json), false);
            }
        }
    }
}
