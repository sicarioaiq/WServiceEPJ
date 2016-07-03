using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using WSServiceEPJ.Entity;
using WSServiceEPJ.Utility;

namespace WSServiceEPJ.DataAccess
{
    public class TMPagoMensualDA
    {
        #region Singleton

        private static TMPagoMensualDA _instancia;

        /// <summary>
        /// Singleton
        /// </summary>
        public static TMPagoMensualDA Instancia
        {
            get { return _instancia ?? (_instancia = new TMPagoMensualDA()); }
        }

        #endregion

        public List<TMPagoMensual> ListarPagoMensual()
        {
            List<TMPagoMensual> lstTMPagoMensual = new List<TMPagoMensual>();
            try
            {
                using (IDataReader oReader = SqlHelper.Instance.ExecuteReader("USP_TMPAGOMENSUAL"))
                {
                    while (oReader.Read())
                    {
                        lstTMPagoMensual.Add(ReaderUtility.MapearObjeto<TMPagoMensual>(oReader));
                    }
                }
            }
            catch (Exception ex)
            {
                lstTMPagoMensual = null;
                throw;
            }
            return lstTMPagoMensual;
        }
    }
}
