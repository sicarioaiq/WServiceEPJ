using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSServiceEPJ.Entity;
using WSServiceEPJ.DataAccess;
namespace WSServiceEPJ.Logic
{
    public class TMPagoMensualBL
    {
        #region Singleton

        private static TMPagoMensualBL _instancia;

        /// <summary>
        /// Singleton
        /// </summary>
        public static TMPagoMensualBL Instancia
        {
            get { return _instancia ?? (_instancia = new TMPagoMensualBL()); }
        }

        #endregion

        public List<TMPagoMensual> ListarUsuario()
        {
            try
            {
                return TMPagoMensualDA.Instancia.ListarPagoMensual();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
