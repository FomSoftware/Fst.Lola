using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FomMonitoringCore.Framework.Model
{


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [Serializable()]
    [XmlRoot("MACHINE")]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class ParametersMachineModel
    {

        /// <remarks/>
        [XmlElement("MODELCODEv997")]
        public int ModelCodeV997 { get; set; }

        /// <remarks/>
        [XmlElement("PARAMETERS")]
        public ParametersListMachineModel Parameters { get; set; }
    }
        
    

    /// <remarks/>
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class ParametersListMachineModel
    {
        
        [XmlElement("PARAMETER")]
        public List<ParameterMachineModel> Parameter { get; set; }
    
    }

    /// <remarks/>
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class ParameterMachineModel
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
        public string PANEL { get; set; }

        /// <remarks/>
        public string CLUSTER { get; set; }

        /// <remarks/>
        public string HISTORICIZED { get; set; }
    }

}



