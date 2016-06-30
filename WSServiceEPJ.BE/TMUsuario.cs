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
    public class TMUsuario : ResultObject
    {
        [DataMember]
        public int intId { get; set; }
        [DataMember]
        public String strUsuario { get; set; }
        [DataMember]
        public String strPassword { get; set; }
        [DataMember]
        public int intIdPerfil { get; set; }
        [DataMember]
        public int intActivo { get; set; }
        [DataMember]
        public String strRuta { get; set; }
        [DataMember]
        public String UltimaActualizacion { get; set; }


    }
}
