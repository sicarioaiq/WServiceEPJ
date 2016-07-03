using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using WSServiceEPJ.Entity;
using WSServiceEPJ.Utility;
namespace WSServiceEPJ.DataAccess
{
    public class TMPersonaDA
    {
        #region Singleton

        private static TMPersonaDA _instancia;

        /// <summary>
        /// Singleton
        /// </summary>
        public static TMPersonaDA Instancia
        {
            get { return _instancia ?? (_instancia = new TMPersonaDA()); }
        }

        #endregion

        public List<TMPersona> ListarPersona()
        {
            List<TMPersona> lstTMPersona = new List<TMPersona>();
            try
            {
                using (IDataReader oReader = SqlHelper.Instance.ExecuteReader("USP_TMPERSONA"))
                {
                    while (oReader.Read())
                    {
                        lstTMPersona.Add(ReaderUtility.MapearObjeto<TMPersona>(oReader));
                    }
                }
            }
            catch (Exception ex)
            {
                lstTMPersona = null;
                throw;
            }
            return lstTMPersona;
        }
    }
}
