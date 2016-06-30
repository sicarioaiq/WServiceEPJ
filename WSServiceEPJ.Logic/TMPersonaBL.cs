using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSServiceEPJ.Entity;
using WSServiceEPJ.DataAccess;
namespace WSServiceEPJ.Logic
{
    public class TMPersonaBL
    {
        #region Singleton

        private static TMPersonaBL _instancia;

        /// <summary>
        /// Singleton
        /// </summary>
        public static TMPersonaBL Instancia
        {
            get { return _instancia ?? (_instancia = new TMPersonaBL()); }
        }

        #endregion

        public List<TMPersona> ListarPersona()
        {
            try
            {
                return TMPersonaDA.Instancia.ListarPersona();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
