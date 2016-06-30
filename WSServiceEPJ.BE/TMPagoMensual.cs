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
    public class TMPagoMensual : ResultObject
    {
        [DataMember]
        public int intIdPago { get; set; }
        [DataMember]
        public int intIdPersona { get; set; }
        [DataMember]
        public String strMonto { get; set; }
        [DataMember]
        public int intAnio { get; set; }
        [DataMember]
        public int intMes { get; set; }
        [DataMember]
        public String strMes { get; set; }
    }
}
