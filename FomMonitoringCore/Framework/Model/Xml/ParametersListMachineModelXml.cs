using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FomMonitoringCore.Framework.Model.Xml
{
    /// <remarks/>
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class ParametersListMachineModelXml
    {
        
        [XmlElement("PARAMETER")]
        public List<ParameterMachineModelXml> Parameter { get; set; }
    
    }

}



