using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Configuration;
using WSServiceEPJ.Utility;

namespace WSServiceEPJ.Entity
{
    /// <summary>
    /// Métodos de acceso a los procedimientos almacenados
    /// </summary>
    public class SqlHelper
    {
        private static readonly string _strConn = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        private static readonly string _strProvider = "System.Data.SqlClient";

        #region Singleton

        private static SqlHelper _instance;

        /// <summary>
        /// Singleton
        /// </summary>
        public static SqlHelper Instance
        {
            get { return _instance ?? (_instance = new SqlHelper()); }
        }

        #endregion

        /// <summary>
        /// Carga una una DataTable con los resultados de un procedimiento almacenado
        /// </summary>
        /// <param name="storedProcedureName">Nombre del procedimiento almacenado</param>
        /// <param name="parameters">Parámetros del procedimiento almacenado</param>
        /// <returns>DataTable con los resultados</returns>
        public DataTable LoadTable(string storedProcedureName, Dictionary<string, object> parameters)
        {
            DataTable dtTable = new DataTable();
            DbProvider database = new DbProvider(_strConn, _strProvider);
            DbCommand command = database.GetStoredProcCommand(storedProcedureName);
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                database.AddInParameter(command, parameter.Key, parameter.Value);
            }
            dtTable.Load(database.ExecuteReader(command));
            return dtTable;
        }

        /// <summary>
        /// Carga una una DataTable con los resultados de un procedimiento almacenado sin parámetros
        /// </summary>
        /// <param name="storedProcedureName">Nombre del procedimiento almacenado</param>
        /// <returns>DataTable con los resultados</returns>
        public DataTable LoadTable(string storedProcedureName)
        {
            DataTable dtTable = new DataTable();
            DbProvider database = new DbProvider(_strConn, _strProvider);
            DbCommand command = database.GetStoredProcCommand(storedProcedureName);
            dtTable.Load(database.ExecuteReader(command));
            return dtTable;
        }

        /// <summary>
        /// Retorna el número de filas afectadas
        /// </summary>
        /// <param name="storedProcedureName">Nombre del procedimiento almacenado</param>
        /// <param name="parameters">Parámetros del procedimiento almacenado</param>
        /// <returns>Número de filas afectadas</returns>
        public int ExecuteNonQuery(string storedProcedureName, Dictionary<string, object> parameters)
        {
            DbProvider database = new DbProvider(_strConn, _strProvider);
            DbCommand command = database.GetStoredProcCommand(storedProcedureName);
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                database.AddInParameter(command, parameter.Key, parameter.Value);
            }
            return database.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Retorna la primera fila resultante
        /// </summary>
        /// <param name="storedProcedureName">Nombre del procedimiento almacenado</param>
        /// <param name="parameters">Parámetros del procedimiento almacenado</param>
        /// <returns>Objeto con la primera fila resultante</returns>
        public object ExecuteScalar(string storedProcedureName, Dictionary<string, object> parameters)
        {
            DbProvider database = new DbProvider(_strConn, _strProvider);
            DbCommand command = database.GetStoredProcCommand(storedProcedureName);
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                database.AddInParameter(command, parameter.Key, parameter.Value);
            }
            return database.ExecuteScalar(command);
        }

        /// <summary>
        /// Retorna el DataReader con los datos obtenidos de un procedimiento almacenado
        /// </summary>
        /// <param name="storedProcedureName">Nombre del procedimiento almacenado</param>
        /// <param name="parameters">Parámetros del procedimiento almacenado</param>
        /// <returns>DataReader con los resultados</returns>
        public IDataReader ExecuteReader(string storedProcedureName, Dictionary<string, object> parameters)
        {
            DbProvider database = new DbProvider(_strConn, _strProvider);
            DbCommand command = database.GetStoredProcCommand(storedProcedureName);
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                database.AddInParameter(command, parameter.Key, parameter.Value);
            }
            IDataReader dr = database.ExecuteReader(command);
            return dr;
        }

        /// <summary>
        /// Retorna el DataReader con los datos obtenidos de un procedimiento almacenado sin parámetros
        /// </summary>
        /// <param name="storedProcedureName">Nombre del procedimiento almacenado</param>
        /// <returns>DataReader con los resultados</returns>
        public IDataReader ExecuteReader(string storedProcedureName)
        {
            DbProvider database = new DbProvider(_strConn, _strProvider);
            DbCommand command = database.GetStoredProcCommand(storedProcedureName);
            IDataReader dr = database.ExecuteReader(command);
            return dr;
        }

        //public IDataReader JobExecuteReader(string storedProcedureName, Dictionary<string, object> parameters)
        //{
        //    //string cn = @"Data Source=.;Initial Catalog=COSAPI_Test; IntegratedSecurity=True";

        //    DbProvider database = new DbProvider(_strConn, _strProvider);
        //    DbCommand command = database.GetStoredProcCommand(storedProcedureName);
        //    foreach (KeyValuePair<string, object> parameter in parameters)
        //    {
        //        database.AddInParameter(command, parameter.Key, parameter.Value);
        //    }
        //    IDataReader dr = database.ExecuteReader(command);
        //    return dr;
        //}

        //public void SetEntity(object entidad, IDataReader dr)
        //{
        //    PropertyInfo[] propertyInfoList = entidad.GetType().GetProperties();
        //    foreach (PropertyInfo propertyInfo in propertyInfoList)
        //    {
        //        try
        //        {
        //            int cant = dr.FieldCount;
        //            for (int indice = 0; indice < cant; indice++)
        //            {
        //                if (dr.GetName(indice).Equals(propertyInfo.Name))
        //                {
        //                    if (!dr.IsDBNull(indice))
        //                    {
        //                        propertyInfo.SetValue(entidad, dr.GetValue(indice), null);
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //        catch
        //        {
        //            throw;
        //        }
        //    }
        //}
    }
}
