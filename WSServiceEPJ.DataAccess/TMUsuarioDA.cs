using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using WSServiceEPJ.Entity;
using WSServiceEPJ.Utility;

namespace WSServiceEPJ.DataAccess
{
    public class TMUsuarioDA
    {
        #region Singleton

        private static TMUsuarioDA _instancia;

        /// <summary>
        /// Singleton
        /// </summary>
        public static TMUsuarioDA Instancia
        {
            get { return _instancia ?? (_instancia = new TMUsuarioDA()); }
        }

        #endregion

        public TMUsuario LoginEPJ(String strUsuario, String strPassword)
        {
            TMUsuario objTMUsuario= new TMUsuario();
            try
            {
                Dictionary<string, object> parameter = new Dictionary<string, object>
                {
                    {"@strUsuario", strUsuario},
                    {"@strPassword", strPassword}
                };
                using (IDataReader oReader = SqlHelper.Instance.ExecuteReader("USP_GET_TMUSUARIO", parameter))
                {
                    while (oReader.Read())
                    {
                        objTMUsuario = ReaderUtility.MapearObjeto<TMUsuario>(oReader);
                    }
                }
                objTMUsuario.ErrorMensaje = string.Empty;
                objTMUsuario.ErrorCode= string.Empty;
            }
            catch (Exception ex)
            {
                objTMUsuario.ErrorMensaje = ex.Message;
                objTMUsuario.ErrorCode = "public TMUsuario LoginEPJ(String strUsuario, String strPassword)";
                throw;
            }
            return objTMUsuario;
        }

        public List<TMUsuario> ListarUsuario()
        {
            List<TMUsuario> lstTMUsuario = new List<TMUsuario>();
            try
            {
                using (IDataReader oReader = SqlHelper.Instance.ExecuteReader("USP_TMUSUARIO"))
                {
                    while (oReader.Read())
                    {
                        lstTMUsuario.Add( ReaderUtility.MapearObjeto<TMUsuario>(oReader));
                    }
                }                      
            }
            catch (Exception ex)
            {
                lstTMUsuario = null;
                throw;
            }
            return lstTMUsuario;
        }
    }
}
