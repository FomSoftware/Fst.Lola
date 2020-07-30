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
        public long? ResidueWorkingTimeBar { get; set; }
        public long? ResidueWorkingTimeJob { get; set; }

        public virtual string ResidueWorkingTimeMin => ResTimeMin(this.ResidueWorkingTime);
        public virtual string ResidueWorkingTimeSec => ResTimeSec(this.ResidueWorkingTime);

        public virtual string ResidueWorkingTimeBarMin => ResTimeMin(this.ResidueWorkingTimeBar);
        public virtual string ResidueWorkingTimeBarSec => ResTimeSec(this.ResidueWorkingTimeBar);

        public virtual string ResidueWorkingTimeJobMin => ResTimeMin(this.ResidueWorkingTimeJob);
        public virtual string ResidueWorkingTimeJobSec => ResTimeSec(this.ResidueWorkingTimeJob);

        public string ResTimeMin(long? _ResidueWorkingTime)
        {
            string result = "0";
            if (_ResidueWorkingTime != null)
            {
                //ResidueWorkingTime arriva in secondi e non in ticks
                TimeSpan interval = new TimeSpan((long)_ResidueWorkingTime * 10000000);
                int res = interval.Days * 24 * 60 +
                          interval.Hours * 60 +
                          interval.Minutes;
                result = res.ToString("D");
            }

            return result;
        }

        public string ResTimeSec(long? _ResidueWorkingTime)
        {
            string result = "0";
            if (_ResidueWorkingTime != null)
            {
                TimeSpan interval = new TimeSpan((long)_ResidueWorkingTime * 10000000);
                int res = interval.Seconds;
                result = res.ToString("D");
            }
            return result;
        }

    }
}