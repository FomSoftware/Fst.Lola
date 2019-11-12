using FomMonitoringCore.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.Framework.Model
{
    public class ParameterMachineValueModel
    {
        public int VarNumber { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string ConvertedValue => Convert();
        public DateTime? UtcDateTime { get; set; }
        public string CnUm { get; set; }
        public string HmiUm { get; set; }

        public string ConvertedDistanceValue(string format = "0.000")
        {
            string res = Value;
            double temp;
            if(double.TryParse(Value, out temp) && !string.IsNullOrEmpty(CnUm) && !string.IsNullOrEmpty(HmiUm)
                && Enum.IsDefined(typeof(enUnitaMisuraLength), CnUm)
                && Enum.IsDefined(typeof(enUnitaMisuraLength), HmiUm))
            {
                enUnitaMisuraLength cn;
                enUnitaMisuraLength hmi;
                if (Enum.TryParse(CnUm, true, out cn) && Enum.TryParse(HmiUm, true, out hmi))
                {                    
                    if (format != null)
                        res = (double.Parse(Value) * ((double)hmi / (double)cn)).ToString(format);
                    else
                        res = (double.Parse(Value) * ((double)hmi / (double)cn)).ToString();                    
                }
            }
            return res;
        }

        public string ConvertedTimeValue()
        {
            string res = Value;
            if (double.TryParse(Value, out var temp) && !string.IsNullOrEmpty(CnUm) && !string.IsNullOrEmpty(HmiUm)
                && Enum.IsDefined(typeof(enUnitaMisuraTime), CnUm)
                && Enum.IsDefined(typeof(enUnitaMisuraTime), HmiUm))
            {
                var d = temp.RoundToInt();
                enUnitaMisuraTime cn;
                enUnitaMisuraTime hmi = enUnitaMisuraTime.S;
                if (Enum.TryParse(CnUm, true, out cn) && Enum.TryParse(HmiUm, true, out hmi))
                {
                    d = (double.Parse(Value).RoundToInt() * ((int)hmi / (int)cn));
                }
                TimeSpan timeSpan;
                switch (hmi)
                {
                    case enUnitaMisuraTime.S:
                        timeSpan = new TimeSpan(0, 0, d);
                        res = $"{timeSpan.TotalHours.RoundToInt()}h {timeSpan.Minutes}min";
                        return res;
                    case enUnitaMisuraTime.M:
                        timeSpan = new TimeSpan(0, d, 0);
                        res = $"{timeSpan.TotalHours.RoundToInt()}h {timeSpan.Minutes}min";
                        return res;
                    case enUnitaMisuraTime.H:
                        timeSpan = new TimeSpan(d, 0, 0);
                        res = $"{timeSpan.TotalHours.RoundToInt()}h {timeSpan.Minutes}min";
                        return res;
                }
                
            }

            return res;
        }

        public string ConvertedNumberValue()
        {
            string res = Value;
            if (double.TryParse(Value, out var temp))
            {
                res = temp.ToString("N0");
            }
            return res;
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
