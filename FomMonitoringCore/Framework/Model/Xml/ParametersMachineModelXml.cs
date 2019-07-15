using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FomMonitoringCore.Framework.Model.Xml
{


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [Serializable()]
    [XmlRoot("MACHINE")]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class ParametersMachineModelXml
    {

        /// <remarks/>
        [XmlElement("MODELCODEv997")]
        public int ModelCodeV997 { get; set; }

        /// <remarks/>
        [XmlElement("PARAMETERS")]
        public ParametersListMachineModelXml Parameters { get; set; }
    }

}



