using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WSServiceEPJ.Utility
{
    public static class LogEvent
    {
        /// <summary>
        /// Log del Sistema
        /// </summary>
        /// <param name="ex">Objeto Excepción</param>
        /// <param name="parameters">Parámetros</param>
        /// <param name="booError">(Opcional) Booleano que indica error</param>
        /// <param name="strMensaje">(Opcional) Mensaje de Error</param>
        /// <param name="strClase">(Opcional) Clase de donde proviene</param>
        /// <param name="strMetodo">(Opcional) Método de donde proviene</param>
        public static void AlmacenarErrorLog(Exception ex, Dictionary<string, object> parameters, string strRutaServer,bool booError = true, string strMensaje = "", string strClase = "", string strMetodo = "")
        {
            try
            {
                StreamWriter objWriter;
                FileStream objFile;

                string strRuta;
                strRuta = strRutaServer + "EventLog";
                var objDirectorio = new DirectoryInfo(strRuta);

                if (objDirectorio.Exists == false)
                {
                    objDirectorio.Create();
                }
                foreach (FileInfo objFileOld in objDirectorio.GetFiles("*.log"))
                {
                    if (objFileOld.LastAccessTime.AddDays(60) < DateTime.Now)
                    {
                        objFileOld.Delete();
                    }
                }
                objFile = new FileStream(objDirectorio.FullName + "\\EventLog_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log", FileMode.OpenOrCreate, FileAccess.Write);
                objWriter = new StreamWriter(objFile);
                objWriter.BaseStream.Seek(0, SeekOrigin.End);
                if (booError)
                {
                    if (ex != null)
                    {
                        objWriter.WriteLine("[###############################################################################]");
                        objWriter.WriteLine("[Fecha              ][" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]");
                        objWriter.WriteLine("[Tipo Log           ][ " + "ERROR" + " ]");
                        objWriter.WriteLine("[Source             ][ " + ex.Source + " ]");
                        objWriter.WriteLine("[Clase              ][ " + strClase + " ]");
                        objWriter.WriteLine("[Metodo             ][ " + strMetodo + " ]");
                        if (parameters != null)
                        {
                            foreach (KeyValuePair<string, object> parameter in parameters)
                            {
                                objWriter.WriteLine("[Parametro  -  " + parameter.Key + "  ][ " + parameter.Value + " ]");
                            }
                        }
                        objWriter.WriteLine("[Mensaje            ][ " + ex.Message + " ]");
                        objWriter.WriteLine("[StackTrace         ][ " + ex.StackTrace + " ]");
                        objWriter.WriteLine("[InnerException     ][ " + Convert.ToString(ex.InnerException) + " ]");
                        if (!string.IsNullOrEmpty(strMensaje))
                        {
                            objWriter.WriteLine("[Mensaje Info.      ][ " + strMensaje + " ]");
                        }
                        objWriter.WriteLine("");
                        objWriter.WriteLine("");
                    }
                    else
                    {
                        objWriter.WriteLine("[###############################################################################]");
                        objWriter.WriteLine("[Fecha              ][" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]");
                        objWriter.WriteLine("[Tipo Log           ][ " + "ERROR" + " ]");
                        objWriter.WriteLine("[Clase              ][ " + strClase + " ]");
                        objWriter.WriteLine("[Metodo             ][ " + strMetodo + " ]");
                        if (parameters != null)
                        {
                            foreach (KeyValuePair<string, object> parameter in parameters)
                            {
                                objWriter.WriteLine("[Parametro  -  " + parameter.Key + "  ][ " + parameter.Value + " ]");
                            }
                        }
                        objWriter.WriteLine("");
                        objWriter.WriteLine("");
                    }
                }
                else
                {
                    objWriter.WriteLine("[###############################################################################]");
                    objWriter.WriteLine("[Fecha              ][ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]");
                    objWriter.WriteLine("[Tipo Log           ][ " + "INFORMACION" + " ]");
                    objWriter.WriteLine("[Mensaje Info.      ][ " + strMensaje + " ]");
                    objWriter.WriteLine("[Clase              ][ " + strClase + " ]");
                    objWriter.WriteLine("[Metodo             ][ " + strMetodo + " ]");
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, object> parameter in parameters)
                        {
                            objWriter.WriteLine("[Parametro  -  " + parameter.Key + "  ][ " + parameter.Value + " ]");
                        }
                    }
                    objWriter.WriteLine("");
                    objWriter.WriteLine("");
                }
                objWriter.WriteLine();
                objWriter.Flush();
                objWriter.Close();
                objFile.Close();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

       
    }
}
