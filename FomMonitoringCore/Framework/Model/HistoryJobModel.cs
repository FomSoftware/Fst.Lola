using System;

namespace FomMonitoringCore.Framework.Model
{
    public class HistoryJobModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime? Day { get; set; }
        public long? ElapsedTime { get; set; }
        public int? MachineId { get; set; }
        public int? Period { get; set; }
        public int? PiecesProduced { get; set; }
        public int? PiecesProducedDay { get; set; }
        public int? TotalPieces { get; set; }
    }
}
