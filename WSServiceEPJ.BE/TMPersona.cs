using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WSServiceEPJ.Entity
{
    [DataContract]
    public class TMPersona : ResultObject
    {
        [DataMember]
        public int intIdPersona { get; set; }
        [DataMember]
        public string strNombrePersona { get; set; }
        [DataMember]
        public int intActivo { get; set; }
    }
}
