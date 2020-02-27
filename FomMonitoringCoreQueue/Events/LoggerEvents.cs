using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCore.Service;

namespace FomMonitoringCoreQueue.Events
{
    public class LoggerEventsQueue : EventArgs
    {
        public LogService.TypeLevel TypeLevel { get; set; }
        public TypeEvent Type { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }

    public enum TypeEvent
    {
        Variable,
        Messages,
        HistoryBarJobPiece,
        State,
        Info,
        Tool
    }

}
