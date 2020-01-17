using System;

namespace FomMonitoringCore.Framework.Model
{
    public class MessageMachineModel
    {
        private DateTime? _day;
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime? Day {
            get => _day;
            set
            {
                if(value.HasValue)
                    _day = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                else
                    _day = null;
            }
        }
        public string Params { get; set; }
        public long? ElapsedTime { get; set; }
        public int? MachineId { get; set; }
        public string Operator { get; set; }
        public int Type { get; set; }
        public int? Group { get; set; }
        public string GroupName { get; set; }
        public DateTime? StartTime { get; set; }
        public int? StateId { get; set; }       
        public DateTime? IgnoreDate { get; set; }
        public bool? IsPeriodicMsg { get; set; }
        public object TypeName { get; internal set; }
        public string Description { get; set; }
    }
}
