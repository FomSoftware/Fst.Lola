using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FomMonitoringCore.Framework.Model.Xml
{
    /// <remarks/>
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class ParameterMachineModelXml
    {
        /// <remarks/>
        public string VAR_NUMBER { get; set; }

        /// <remarks/>
        public string HMI_LABEL { get; set; }

        /// <remarks/>
        public string MACHINE_GROUP { get; set; }

        /// <remarks/>
        public string CN_TYPE { get; set; }

        /// <remarks/>
        public string CN_UM { get; set; }

        /// <remarks/>
        public string KEYWORD { get; set; }

        /// <remarks/>
        public string HMI_UM { get; set; }

        /// <remarks/>
        public string DEFAULT_VALUE { get; set; }

        /// <remarks/>
        public string W_LEVEL { get; set; }

        /// <remarks/>
        public string R_LEVEL { get; set; }

        /// <remarks/>
        public string HMI_SECTION { get; set; }

        /// <remarks/>
        public string LOLA_LABEL { get; set; }

        /// <remarks/>
        public string THRESHOLD_MIN { get; set; }

        /// <remarks/>
        public string THRESHOLD_MAX { get; set; }

        /// <remarks/>
        public string THRESHOLD_LABEL { get; set; }

        /// <remarks/>
        public int PANEL { get; set; }

        /// <remarks/>
        public string CLUSTER { get; set; }

        /// <remarks/>
        public string HISTORICIZED { get; set; }
    }

}



