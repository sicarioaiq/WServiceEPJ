using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WSServiceEPJ.Utility
{
    /// <summary>
    /// Define los metodos de conexion y ejecuccion de comandos en la Base de Datos
    /// </summary>
    public class DbProvider
    {
        private readonly string _cadenaConexion;
        private readonly string _providerName;
        private readonly DbProviderFactory _factoty;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="cadenaConexion">Cadena de conexion</param>
        /// <param name="provider">Tipo de proveedor de Datos</param>
        public DbProvider(string cadenaConexion, string provider)
        {
            //Inicializamos la cadena de conexion a la base de datos
            _cadenaConexion = cadenaConexion;
            _providerName = provider;
            _factoty = SqlClientFactory.Instance;
            switch (_providerName)
            {
                case "System.Data.SqlClient":
                    _factoty = SqlClientFactory.Instance;
                    break;
            }
        }

        /// <summary>
        /// Crea un objecto DbCommand que ejecutara el procedure especificado
        /// </summary>
        /// <param name="spNombre">Nombre del procedure</param>
        /// <returns>Comando DbCommand</returns>
        public DbCommand GetStoredProcCommand(String spNombre)
        {
            DbCommand cmd = _factoty.CreateCommand();
            if (cmd != null)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spNombre;
            }
            return cmd;
        }

        /// <summary>
        /// Devuelve un objeto DBCommand que ejecutara el comando especificado especificado
        /// </summary>
        /// <param name="strCmd">Script a ejecutar</param>
        /// <param name="typo">Tipo de comando</param>
        /// <returns>Comando DbCommand</returns>
        public DbCommand GetCommand(string strCmd, CommandType typo)
        {
            DbCommand cmd = _factoty.CreateCommand();
            if (cmd != null)
            {
                cmd.CommandType = typo;
                cmd.CommandText = strCmd;
            }
            return cmd;
        }

        /// <summary>
        /// Ejecuta el command especificado y devuelve un objecto
        /// </summary>
        /// <param name="cmd">el DbCommand que se ejecutara en la BD</param>
        /// <returns>object</returns>
        public object ExecuteScalar(IDbCommand cmd)
        {
            object r;
            //se instancia un objeto para la conexion
            using (DbConnection cn = CreaConnexion())
            {
                cmd.Connection = cn;
                cmd.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"]);
                cn.Open();
                r = cmd.ExecuteScalar();
            }
            return r;
        }

        /// <summary>
        /// Ejecuta el commando y devuelve un DataReader para leer los datos obtenidos.
        /// </summary>
        /// <param name="cmd">DbCommand que se ejecutara en la BD</param>
        /// <returns>IDataReader</returns>
        public IDataReader ExecuteReader(IDbCommand cmd)
        {
            //se instancia un objeto para la conexion
            DbConnection cn = CreaConnexion();
            cmd.Connection = cn;
            cmd.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"]);
            cn.Open();
            IDataReader r = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            return r;
        }

        /// <summary>
        /// Devuelve el numero de filas afectas al ejecutar el DbCommand
        /// no se necesita abrir la cadena de conexion
        /// </summary>
        /// <param name="cmd">DbCommand que se ejecutara en la BD</param>
        /// <returns>Número de filas afectadas</returns>
        public int ExecuteNonQuery(IDbCommand cmd)
        {
            int r;
            using (DbConnection cn = CreaConnexion())
            {
                cmd.Connection = cn;
                cmd.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"]);
                cn.Open();
                r = cmd.ExecuteNonQuery();
            }
            return r;
        }

        /// <summary>
        /// Devuelve el resultado en un DataSet
        /// </summary>
        /// <param name="cmd">Comando a ejecutar</param>
        /// <returns>DataSet con resultados</returns>
        public DataSet ExecuteDataSet(DbCommand cmd)
        {
            DataSet ds = new DataSet();
            using (DbConnection cn = CreaConnexion())
            {
                cmd.Connection = cn;
                cmd.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"]);
                DbDataAdapter da = _factoty.CreateDataAdapter();
                if (da != null)
                {
                    da.SelectCommand = cmd;
                    da.Fill(ds);
                }
            }
            return ds;
        }

        /// <summary>
        /// Devuelve la cadena de conexión en un objeto DbConnection
        /// </summary>
        /// <returns>DbConnection</returns>
        private DbConnection CreaConnexion()
        {
            DbConnection cn = _factoty.CreateConnection();
            if (cn != null)
            {
                cn.ConnectionString = _cadenaConexion;
            }
            return cn;
        }

        /// <summary>
        /// Agrega un parámetro al comando IDbCommand
        /// </summary>
        /// <param name="cmd">Comando IDbCommand</param>
        /// <param name="parNombre">Nombre del parámetro</param>
        /// <param name="tipo">Tipo de variable en BD</param>
        /// <param name="valor">Valor del parámetro</param>
        public void AddInParameter(IDbCommand cmd, string parNombre, DbType tipo, object valor)
        {
            DbParameter par = _factoty.CreateParameter();
            if (par != null)
            {
                par.DbType = tipo;
                par.ParameterName = parNombre;
                par.Value = valor;
                par.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(par);
            }
        }

        /// <summary>
        /// Agrega un parámetro al comando IDbCommand sin usar el tipo de variable
        /// </summary>
        /// <param name="cmd">Comando IDbCommand</param>
        /// <param name="parNombre">Nombre del parámetro</param>
        /// <param name="valor">Valor del parámetro</param>
        public void AddInParameter(IDbCommand cmd, string parNombre, object valor)
        {
            DbParameter par = _factoty.CreateParameter();
            if (par != null)
            {
                par.ParameterName = parNombre;
                par.Value = valor;
                par.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(par);
            }
        }

        /// <summary>
        /// Agrega parametros de salida para ejecutar los commandos en la BD
        /// </summary>
        /// <param name="cmd">DBCommand a ejecutar</param>
        /// <param name="parNombre">Nombre del parametro</param>
        /// <param name="tipo">Tipo de dato del parametro</param>
        /// <param name="valor">Valor del parametro</param>
        public void AddOuputParameter(IDbCommand cmd, string parNombre, DbType tipo, object valor)
        {
            DbParameter par = _factoty.CreateParameter();
            if (par != null)
            {
                par.DbType = tipo;
                par.ParameterName = parNombre;
                par.Value = valor;
                par.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(par);
            }
        }

        /// <summary>
        /// Agrega parametros de salida para ejecutar los commandos en la BD sin usar el tipo de variable
        /// </summary>
        /// <param name="cmd">DBCommand a ejecutar</param>
        /// <param name="parNombre">Nombre del parametro</param>
        /// <param name="valor">Valor del parametro</param>
        public void AddOuputParameter(IDbCommand cmd, string parNombre, object valor)
        {

            DbParameter par = _factoty.CreateParameter();
            if (par != null)
            {
                par.ParameterName = parNombre;
                par.Value = valor;
                par.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(par);
            }
        }

        /// <summary>
        /// Agrega parametros de salida para ejecutar los commandos en la BD
        /// </summary>
        /// <param name="cmd">DBCommand a ejecutar</param>
        /// <param name="parNombre">Nombre del parametro</param>
        /// <param name="tipo">Tipo de dato del parametro</param>
        /// <param name="size">Tamaño máximo en bytes del valor en la columna</param>
        /// <param name="valor">Valor del parametro</param>
        public void AddOuputParameter(IDbCommand cmd, string parNombre, DbType tipo, int size, object valor)
        {
            DbParameter par = _factoty.CreateParameter();
            if (par != null)
            {
                par.DbType = tipo;
                par.ParameterName = parNombre;
                par.Size = size;
                par.Value = valor;
                par.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(par);
            }
        }

        /// <summary>
        /// Obtiene el valor de los parametros de salida obtenidos despues de ejecutar el Command
        /// </summary>
        /// <param name="cmd">DbCommand</param>
        /// <param name="parameterName">Nombre del parametro de salida</param>
        /// <returns>Objeto con el valor del parámetro ingresado</returns>
        public object GetParameterValue(DbCommand cmd, string parameterName)
        {
            return cmd.Parameters[parameterName].Value;
        }

    }
}
