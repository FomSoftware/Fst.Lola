using System;
using System.Linq;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Dto;

namespace FomMonitoringCoreQueue.ProcessData
{
    public class MessageProcessor : IProcessor<Message>
    {
        private readonly IFomMonitoringEntities _context;
        private readonly IMessageService _messageService;

        public MessageProcessor(IFomMonitoringEntities context, IMessageService messageService)
        {
            _messageService = messageService;
            _context = context;
        }

        public bool ProcessData(Message data)
        {

            try
            {
                _context.Refresh();
                var mac = _context.Set<Machine>()
                    .FirstOrDefault(m => m.Serial == data.InfoMachine.FirstOrDefault().MachineSerial);
                if (mac == null)
                    return false;


                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}