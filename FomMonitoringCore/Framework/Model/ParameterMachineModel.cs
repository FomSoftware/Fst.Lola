using FomMonitoringCore.Framework.Common;
using System;
using System.Globalization;

namespace FomMonitoringCore.Framework.Model
{
    public class ParameterMachineValueModel
    {
        public int VarNumber { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Cluster { get; set; }
        public string ConvertedValue => Convert();
        public string ConvertedHourValue => ConvertHour();
        public string ConvertedMinValue => ConvertMin();
        public string ConvertedSecValue => ConvertSec();
        public DateTime? UtcDateTime { get; set; }
        public string CnUm { get; set; }
        public string Keyword { get; set; }
        public string HmiUm { get; set; }

        //public string ConvertedDistanceValue(string format = "{0:0,0.00}")
        public string ConvertedDistanceValue(string format = "0.000")
        {
            var res = Value;
            if(double.TryParse(Value, out var temp) && !string.IsNullOrEmpty(CnUm) && !string.IsNullOrEmpty(HmiUm)
                && Enum.IsDefined(typeof(enUnitaMisuraLength), CnUm)
                && Enum.IsDefined(typeof(enUnitaMisuraLength), HmiUm))
            {
                if (Enum.TryParse(CnUm, true, out enUnitaMisuraLength cn) && Enum.TryParse(HmiUm, true, out enUnitaMisuraLength hmi))
                {
                    var numRes = temp * ((double) hmi / (double) cn);
                    if (format != null)
                        res = numRes.ToString(format);
                    else
                        res = numRes.ToString(CultureInfo.InvariantCulture);                    
                }
            }
            return res;
        }

        public string ConvertedTimeValue()
        {
            var res = Value;
            if (double.TryParse(Value, out var temp) && !string.IsNullOrEmpty(CnUm) && !string.IsNullOrEmpty(HmiUm)
                && Enum.IsDefined(typeof(enUnitaMisuraTime), CnUm)
                && Enum.IsDefined(typeof(enUnitaMisuraTime), HmiUm))
            {
                var d = temp.RoundToInt();
                var hmi = enUnitaMisuraTime.S;
                if (Enum.TryParse(CnUm, true, out enUnitaMisuraTime cn) && Enum.TryParse(HmiUm, true, out hmi))
                {
                    d = (temp * ((double)hmi / (double)cn)).RoundToInt();
                }
                TimeSpan timeSpan;
                switch (hmi)
                {
                    case enUnitaMisuraTime.S:
                        timeSpan = new TimeSpan(0, 0, d);
                        res = $"{Math.Floor(timeSpan.TotalHours)}h {timeSpan.Minutes}min {timeSpan.Seconds}s";
                        return res;
                    case enUnitaMisuraTime.M:
                        timeSpan = new TimeSpan(0, d, 0);
                        res = $"{Math.Floor(timeSpan.TotalHours)}h {timeSpan.Minutes}min";
                        return res;
                    case enUnitaMisuraTime.H:
                        timeSpan = new TimeSpan(d, 0, 0);
                        res = $"{Math.Floor(timeSpan.TotalHours)}h {timeSpan.Minutes}min";
                        return res;
                }
                
            }

            return res;
        }


        public string[] ConvertedTimeValueArray()
        {
            var res = new string[3];
            res[0] = Value;
            if (double.TryParse(Value, out var temp) && !string.IsNullOrEmpty(CnUm) && !string.IsNullOrEmpty(HmiUm)
                && Enum.IsDefined(typeof(enUnitaMisuraTime), CnUm)
                && Enum.IsDefined(typeof(enUnitaMisuraTime), HmiUm))
            {
                var d = temp.RoundToInt();
                var hmi = enUnitaMisuraTime.S;
                if (Enum.TryParse(CnUm, true, out enUnitaMisuraTime cn) && Enum.TryParse(HmiUm, true, out hmi))
                {
                    d = (temp * ((double)hmi / (double)cn)).RoundToInt();
                }
                TimeSpan timeSpan;
                switch (hmi)
                {
                    case enUnitaMisuraTime.S:
                        timeSpan = new TimeSpan(0, 0, d);
                        res[0] = $"{Math.Floor(timeSpan.TotalHours)}";
                        res[1] = $"{timeSpan.Minutes}";
                        res[2] = $"{timeSpan.Seconds}";
                        return res;
                    case enUnitaMisuraTime.M:
                        timeSpan = new TimeSpan(0, d, 0);
                        res[0] = $"{Math.Floor(timeSpan.TotalHours)}";
                        res[1] = $"{timeSpan.Minutes}";
                        res[2] = $"{timeSpan.Seconds}";
                        return res;
                    case enUnitaMisuraTime.H:
                        timeSpan = new TimeSpan(d, 0, 0);
                        res[0] = $"{Math.Floor(timeSpan.TotalHours)}";
                        res[1] = $"{timeSpan.Minutes}";
                        res[2] = $"{timeSpan.Seconds}";
                        return res;
                }

            }

            return res;
        }

        public string ConvertedNumberValue()
        {
            var res = Value;
            if (double.TryParse(Value, out var temp))
            {
                res = temp.ToString("N0");
            }
            return res;
        }

        public string ConvertHour()
        {
            if (!string.IsNullOrEmpty(CnUm) && !string.IsNullOrEmpty(HmiUm)
                                            && Enum.IsDefined(typeof(enUnitaMisuraTime), CnUm)
                                            && Enum.IsDefined(typeof(enUnitaMisuraTime), HmiUm))
            {
                return ConvertedTimeValueArray()[0];
            }

            return Value;
        }

        public string ConvertMin()
        {
            if (!string.IsNullOrEmpty(CnUm) && !string.IsNullOrEmpty(HmiUm)
                                            && Enum.IsDefined(typeof(enUnitaMisuraTime), CnUm)
                                            && Enum.IsDefined(typeof(enUnitaMisuraTime), HmiUm))
            {
                return ConvertedTimeValueArray()[1];
            }
            
            return Value;
        }

        public string ConvertSec()
        {
            if (!string.IsNullOrEmpty(CnUm) && !string.IsNullOrEmpty(HmiUm)
                                            && Enum.IsDefined(typeof(enUnitaMisuraTime), CnUm)
                                            && Enum.IsDefined(typeof(enUnitaMisuraTime), HmiUm))
            {
                return ConvertedTimeValueArray()[2];
            }

            return Value;
        }

        public string Convert()
        {
            if (!string.IsNullOrEmpty(CnUm) && !string.IsNullOrEmpty(HmiUm)
                && Enum.IsDefined(typeof(enUnitaMisuraTime), CnUm)
                && Enum.IsDefined(typeof(enUnitaMisuraTime), HmiUm))
            {
                return ConvertedTimeValue();
            }
            if (!string.IsNullOrEmpty(CnUm) && !string.IsNullOrEmpty(HmiUm)
                && Enum.IsDefined(typeof(enUnitaMisuraLength), CnUm)
                && Enum.IsDefined(typeof(enUnitaMisuraLength), HmiUm))
            {
                return ConvertedDistanceValue();
            }
            if (!string.IsNullOrEmpty(CnUm) && !string.IsNullOrEmpty(HmiUm)
                && CnUm == "NR"
                && HmiUm == "NR")
            {
                return ConvertedNumberValue();
            }

            return Value;
        }
    }
}
