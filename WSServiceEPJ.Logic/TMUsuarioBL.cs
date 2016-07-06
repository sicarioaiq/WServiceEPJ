using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSServiceEPJ.Entity;
using WSServiceEPJ.DataAccess;
namespace WSServiceEPJ.Logic
{
    public class TMUsuarioBL
    {
        #region Singleton

        private static TMUsuarioBL _instancia;

        /// <summary>
        /// Singleton
        /// </summary>
        public static TMUsuarioBL Instancia
        {
            get { return _instancia ?? (_instancia = new TMUsuarioBL()); }
        }

        #endregion

        /// <summary>
        /// Lista las Empresas 
        /// </summary>
        /// <returns>Lista de Empresas</returns>
        public TMUsuario LoginEPJ(String strUsuario, String strPassword)
        {
            try
            {
                return TMUsuarioDA.Instancia.LoginEPJ(strUsuario, strPassword);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<TMUsuario> ListarUsuario()
        {
            try
            {
                return TMUsuarioDA.Instancia.ListarUsuario();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TMUsuario BusquedaUsuario(String strUsuario, String strMail)
        {
            try
            {
                return TMUsuarioDA.Instancia.BusquedaUsuario(strUsuario, strMail);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public TMUsuario Insertar(String strUsuario, String strPassword, String strMail)
        {
            try
            {
                return TMUsuarioDA.Instancia.Insertar(strUsuario, strPassword, strMail);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public TMUsuario ConfirmarCuenta(String strUsuario, String strMail)
        {
            try
            {
                return TMUsuarioDA.Instancia.ConfirmarCuenta(strUsuario, strMail);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
