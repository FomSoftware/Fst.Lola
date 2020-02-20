using System;

namespace FomMonitoringCore.DataProcessing.Dto
{
    public class HistoryJob : BaseModel
    {
        public int Id { get; set; }
        public string JobCode { get; set; }
        public int? TotalPiecesInJob { get; set; }
        public int? CurrentlyProducedPieces { get; set; }
        public DateTime? Day { get; set; }
        public long? ElapsedTime { get; set; }
    }
}