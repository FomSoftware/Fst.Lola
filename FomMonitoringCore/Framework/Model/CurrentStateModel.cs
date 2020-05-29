using System;

namespace FomMonitoringCore.Framework.Model
{
    public class CurrentStateModel
    {
        public int Id { get; set; }
        public string JobCode { get; set; }
        public int? JobProducedPieces { get; set; }
        public int? JobTotalPieces { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int? MachineId { get; set; }
        public string Operator { get; set; }
        public int? StateId { get; set; }
        public string StateTransitionCode { get; set; }
        public long? ResidueWorkingTime { get; set; }

        public virtual string ResidueWorkingTimeMin => ResTimeMin();
        public virtual string ResidueWorkingTimeSec => ResTimeSec();

        public string ResTimeMin()
        {
            string result = "0";
            if (ResidueWorkingTime != null)
            {
                //ResidueWorkingTime arriva in secondi e non in ticks
                TimeSpan interval = new TimeSpan((long)ResidueWorkingTime * 10000000);
                int res = interval.Days * 24 * 60 +
                          interval.Hours * 60 +
                          interval.Minutes;
                result = res.ToString("D");
            }

            return result;
        }

        public string ResTimeSec()
        {
            string result = "0";
            if (ResidueWorkingTime != null)
            {
                TimeSpan interval = new TimeSpan((long)ResidueWorkingTime * 10000000);
                int res = interval.Seconds;
                result = res.ToString("D");
            }
            return result;
        }

    }
}