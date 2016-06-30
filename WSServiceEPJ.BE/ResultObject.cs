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
    public class ResultObject
    {
        //Object _result;
        String _errorCode;
        String _errorMensaje;
        String _contadorLista;
       
        [DataMember]
        public String ErrorMensaje
        {
            get { return _errorMensaje; }
            set { _errorMensaje = value; }
        }
        [DataMember]
        public String ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        [DataMember]
        public String ContadorLista
        {
            get { return _contadorLista; }
            set { _contadorLista = value; }
        }      
    }
}
