using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCore.DataProcessing.Dto.Mongo;
using FomMonitoringCore.Repository.MongoDb;

namespace FomMonitoringCoreQueue.Forwarder
{

    public interface IUnknownForwarder
    {
        void ReForward();
    }

    public class UnknownForwarder : IUnknownForwarder
    {
        private readonly IGenericRepository<Unknown> _unknownGenericRepository;
        private readonly IQueueForwarder _queueForwarder;

        public UnknownForwarder(
            IGenericRepository<Unknown> unknownGenericRepository, IQueueForwarder queueForwarder)
        {
            _unknownGenericRepository = unknownGenericRepository;
            _queueForwarder = queueForwarder;
            ;
        }
        public void ReForward()
        {
            var unknownList = _unknownGenericRepository.Query().ToList();
            foreach (var unknownJson in unknownList)
            {
                _queueForwarder.Forward(unknownJson.EntityUnknown);
                _unknownGenericRepository.Delete(unknownJson.Id);
            }

        }
    }
}
