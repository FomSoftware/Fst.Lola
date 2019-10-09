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
        public DateTime? UtcDateTime { get; set; }
        public string CnUm { get; set; }
        public string HmiUm { get; set; }

        public string ConvertedValue (string format)
        {
            string res = this.Value;
            if(!string.IsNullOrEmpty(this.Value) && !string.IsNullOrEmpty(this.CnUm) && !string.IsNullOrEmpty(this.HmiUm) 
                && this.CnUm != this.HmiUm && !double.IsNaN(double.Parse(this.Value))
                && Enum.IsDefined(typeof(enUnitaMisura), this.CnUm)
                && Enum.IsDefined(typeof(enUnitaMisura), this.HmiUm))
            {
                enUnitaMisura cn;
                enUnitaMisura hmi;
                if (Enum.TryParse(this.CnUm, true, out cn) && Enum.TryParse(this.HmiUm, true, out hmi))
                {
                    
                    if (format != null)
                        res = (double.Parse(this.Value) * ((double)hmi / (double)cn)).ToString(format);
                    else
                        res = (double.Parse(this.Value) * ((double)hmi / (double)cn)).ToString();
                }
            }
            return res;
        }
    }
}
