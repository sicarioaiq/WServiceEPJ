using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Script.Serialization;
using WSServiceEPJ.Entity;
using WSServiceEPJ.Logic;
using System.Configuration;
using System.Reflection;
using System.Threading;
using WSServiceEPJ.Utility;
using System.Net.Mail;
using System.Security.Cryptography;


namespace WServiceEPJ
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        readonly string FTPRutaSubida = ConfigurationManager.AppSettings["FTPRutaSubida"];
        readonly string FTPRutaDescarga = ConfigurationManager.AppSettings["FTPRutaDescarga"];
        readonly string FTPUsername = ConfigurationManager.AppSettings["FTPUsername"];
        readonly string FTPPassword = ConfigurationManager.AppSettings["FTPPassword"];
        readonly string GoogleSMTP = ConfigurationManager.AppSettings["GoogleSMTP"];
        readonly string GooglePORT = ConfigurationManager.AppSettings["GooglePORT"];
        readonly string GoogleUSER = ConfigurationManager.AppSettings["GoogleUSER"];
        readonly string GooglePSWD = ConfigurationManager.AppSettings["GooglePSWD"];
        readonly string GoogleSSL = ConfigurationManager.AppSettings["GoogleSSL"];
        readonly string URLPortal = ConfigurationManager.AppSettings["URLPortal"];
        SQLiteConnection m_dbConnection = null;
        List<TMUsuario> lstTMUsuario;
        List<TMPagoMensual> lstTMPagoMensual;
        List<TMPersona> lstTMPersona;

        [WebInvoke(Method = "POST",
         ResponseFormat = WebMessageFormat.Json,
         RequestFormat = WebMessageFormat.Json,
         BodyStyle = WebMessageBodyStyle.Wrapped,
         UriTemplate = "ObtenerGetData")]
        public String GetData(int value)
        {
            var json = "";
            JavaScriptSerializer jss = new JavaScriptSerializer();
            json = jss.Serialize(value);
            return json;
        }

        [WebInvoke(Method = "POST",
         ResponseFormat = WebMessageFormat.Json,
         RequestFormat = WebMessageFormat.Json,
         BodyStyle = WebMessageBodyStyle.Wrapped,
         UriTemplate = "ObtenerGetDataUsingDataContract")]
        public String GetDataUsingDataContract(String BoolValue, String StringValue)
        {
            CompositeType composite = new CompositeType();
            var json = "";
            try
            {

                composite.BoolValue = Convert.ToBoolean(BoolValue);
                composite.StringValue = StringValue;

                JavaScriptSerializer jss = new JavaScriptSerializer();
                json = jss.Serialize(composite);
            }
            catch (Exception e)
            {

            }

            return json;
        }

        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "Login")]
        public TMUsuario LoginEPJ(String strUsuario, String strPassword)
        {
            TMUsuario objTMUsuario = new TMUsuario();
            string ruta = string.Empty;
            bool flag = true;
            var strRutaServidor = HostingEnvironment.MapPath("~/BD/") + @"Escuela.sqlite";
            string rutaftp = FTPRutaSubida + "Escuela.sqlite";
            try
            {
                if (!string.IsNullOrEmpty(strUsuario) && !string.IsNullOrEmpty(strPassword))
                {
                    objTMUsuario = TMUsuarioBL.Instancia.LoginEPJ(strUsuario, strPassword);
                    if (objTMUsuario != null)
                    {
                        if (string.IsNullOrEmpty(objTMUsuario.ErrorMensaje))
                        {
                            if (!string.IsNullOrEmpty(objTMUsuario.strUsuario) && objTMUsuario.intId > 0)
                            {
                                try
                                {
                                    if (!FtpDirectoryExists(FTPRutaSubida))
                                    {
                                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPRutaSubida);
                                        request.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                                        request.Method = WebRequestMethods.Ftp.MakeDirectory;

                                        FtpWebResponse createresponse = (FtpWebResponse)request.GetResponse();
                                        if (createresponse.StatusCode == FtpStatusCode.PathnameCreated)
                                        {
                                            flag = true;
                                        }
                                    }
                                    if (FtpFileExists(rutaftp))
                                    {
                                        FtpWebRequest requestFileDelete = (FtpWebRequest)WebRequest.Create(rutaftp);
                                        requestFileDelete.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                                        requestFileDelete.Method = WebRequestMethods.Ftp.DeleteFile;
                                        requestFileDelete.GetResponse();
                                    }
                                    if (File.Exists(strRutaServidor))
                                        File.Delete(strRutaServidor);
                                    SQLiteConnection.CreateFile(strRutaServidor);
                                    flag = createMovilDB();
                                }
                                catch (Exception ex)
                                {
                                    LogEvent.AlmacenarErrorLog(ex, null, HostingEnvironment.MapPath("~/BD/"), true, "1er. exception - public TMUsuario LoginEPJ(String strUsuario, String strPassword)", "Service1.svc.cs", "Login");
                                    objTMUsuario.ErrorMensaje = ex.Message;
                                    objTMUsuario.ErrorCode = "1er. exception - public TMUsuario LoginEPJ(String strUsuario, String strPassword)";
                                    objTMUsuario.strIndicadorError = "1";
                                }
                                finally
                                {
                                    SQLiteConnection.ClearAllPools();
                                    GC.Collect();
                                }
                                getUsuario(false);
                                getPagoMensual(false);
                                getPersona(false);
                                using (m_dbConnection = new SQLiteConnection(@"Data Source=" + HostingEnvironment.MapPath("~/BD/") + @"Escuela.sqlite;Version=3;"))
                                {
                                    m_dbConnection.Open();
                                    using (var transaction = m_dbConnection.BeginTransaction())
                                    {
                                        setValor<TMUsuario>(lstTMUsuario, "TMUsuario");
                                        setValor<TMPagoMensual>(lstTMPagoMensual, "TMPagoMensual");
                                        setValor<TMPersona>(lstTMPersona, "TMPersona");
                                        transaction.Commit();
                                    }
                                    m_dbConnection.Close();
                                }
                            }
                            else
                            {
                                return null;
                            }

                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogEvent.AlmacenarErrorLog(e, null, HostingEnvironment.MapPath("~/BD/"), true, "2da. exception - public TMUsuario LoginEPJ(String strUsuario, String strPassword)", "Service1.svc.cs", "Login");
                objTMUsuario.ErrorMensaje = e.Message;
                objTMUsuario.ErrorCode = "2do. exception - public TMUsuario LoginEPJ(String strUsuario, String strPassword)";
                objTMUsuario.strIndicadorError = "1";
            }
            finally
            {
                SQLiteConnection.ClearAllPools();
                if (m_dbConnection != null)
                {
                    GC.Collect();
                }
            }
            try
            {
                //if (flag)
                //{
                //    FtpWebRequest ftprequest = (FtpWebRequest)WebRequest.Create(rutaftp);
                //    ftprequest.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                //    ftprequest.Method = WebRequestMethods.Ftp.UploadFile;
                //    using (var requestStream = ftprequest.GetRequestStream())
                //    {

                //        Thread.Sleep(5000);
                //        using (var input = File.OpenRead(strRutaServidor))
                //        {
                //            input.CopyTo(requestStream);
                //        }
                //    }
                //}
                var file = new FileInfo(strRutaServidor);
                //ruta = string.Concat(FTPRutaDescarga + @"Escuela.sqlite", "|", file.Length);
                ruta = FTPRutaDescarga + @"Escuela.sqlite";
                objTMUsuario.strRuta = ruta;
            }
            catch (IOException ee)
            {
                LogEvent.AlmacenarErrorLog(ee, null, HostingEnvironment.MapPath("~/BD/"), true, "3era. exception - public TMUsuario LoginEPJ(String strUsuario, String strPassword)", "Service1.svc.cs", "Login");
                objTMUsuario.ErrorMensaje = ee.Message;
                objTMUsuario.strIndicadorError = "1";
            }
            catch (Exception e)
            {
                LogEvent.AlmacenarErrorLog(e, null, HostingEnvironment.MapPath("~/BD/"), true, "3era. exception - public TMUsuario LoginEPJ(String strUsuario, String strPassword)", "Service1.svc.cs", "Login");
                objTMUsuario.ErrorMensaje = e.Message;
                objTMUsuario.ErrorCode = "3era. exception -- public TMUsuario LoginEPJ(String strUsuario, String strPassword)";
                objTMUsuario.strIndicadorError = "1";
            }
            return objTMUsuario;
        }

        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "RegistroUsuario")]
        public TMUsuario RegistroUsuarioEPJ(String strUsuario, String strPassword, String strMail)
        {
            TMUsuario objTMUsuario = new TMUsuario();
            try
            {
                if (!string.IsNullOrEmpty(strUsuario) && !string.IsNullOrEmpty(strPassword) && !string.IsNullOrEmpty(strMail))
                {
                    objTMUsuario = TMUsuarioBL.Instancia.BusquedaUsuario(strUsuario, strMail);
                    if (objTMUsuario != null)
                    {
                        if (string.IsNullOrEmpty(objTMUsuario.ErrorMensaje))
                        {
                            if (!string.IsNullOrEmpty(objTMUsuario.strUsuario) && objTMUsuario.intId > 0)
                            {
                                objTMUsuario.strIndicadorError = "1";
                                objTMUsuario.ErrorCode = "0001";
                                objTMUsuario.ErrorMensaje = "El Usuario y/o email ya existe";
                            }
                            else
                            {
                                try
                                {
                                    objTMUsuario = TMUsuarioBL.Instancia.Insertar(strUsuario, strPassword, strMail);
                                    SmtpClient smtpClient = new SmtpClient();
                                    smtpClient.Host = GoogleSMTP;
                                    smtpClient.Port = Convert.ToInt32(GooglePORT);
                                    smtpClient.EnableSsl = GoogleSSL == "1" ? true : false;
                                    smtpClient.UseDefaultCredentials = false;
                                    NetworkCredential credentials = new NetworkCredential(GoogleUSER, GooglePSWD);
                                    smtpClient.Credentials = credentials;
                                    MailMessage mailMessage = new MailMessage();
                                    mailMessage.From = new MailAddress("no-reply@epj.com.pe");
                                    mailMessage.To.Add(strMail);
                                    var bodyPath = HostingEnvironment.MapPath("~/html/index.html");
                                    var body = File.ReadAllText(bodyPath);
                                    string strUser = HttpUtility.UrlEncode(Encrypt(strUsuario.Trim()));
                                    string streMail = HttpUtility.UrlEncode(Encrypt(strMail.Trim()));
                                    var uri = new Uri(URLPortal +"xyzab=param1&abxyz=param2");
                                    var qs = HttpUtility.ParseQueryString(uri.Query);
                                    qs.Set("xyzab", strUser);
                                    qs.Set("abxyz", streMail);

                                    var uriBuilder = new UriBuilder(uri);
                                    uriBuilder.Query = qs.ToString();
                                    var newUri = uriBuilder.Uri;

                                    body = body.Replace("@usuario@", strUsuario).Replace("@mail@", strMail).Replace("@url@",newUri.ToString().Trim());
                                    mailMessage.Subject = "no-reply - Confirmación de cuenta";
                                    mailMessage.IsBodyHtml = true;
                                    mailMessage.Body = body;

                                    smtpClient.Send(mailMessage);
                                }
                                catch (Exception ex)
                                {
                                    objTMUsuario.strIndicadorError = "1";
                                    objTMUsuario.ErrorCode = "00001";
                                    objTMUsuario.ErrorMensaje = ex.Message;
                                }

                            }
                        }
                    }
                }
                else
                {
                    objTMUsuario.strIndicadorError = "1";
                    objTMUsuario.ErrorCode = "0002";
                    objTMUsuario.ErrorMensaje = "Debe Ingresar Usuario, Password y Correo";
                }
            }
            catch (Exception ex)
            {
                objTMUsuario = null;
            }
            return objTMUsuario;
        }

        [WebInvoke(Method = "POST", 
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "ConfirmarUsuario")]
        public TMUsuario ConfirmarUsuarioEPJ(String strUsuario, String strMail)
        {
            TMUsuario objTMUsuario = new TMUsuario();
            try
            {
             
            }
            catch (Exception ex)
            {

            }
            return objTMUsuario;
        }

        private bool FtpDirectoryExists(string dirPath)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(dirPath);
                request.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                //FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                request.GetResponse();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                return response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable;
            }

            return true;
        }

        private bool FtpFileExists(string filePath)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(filePath);
                ftpRequest.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                ftpRequest.UseBinary = true;
                ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;

                //FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpRequest.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse ftpResponse = (FtpWebResponse)ex.Response;
                return ftpResponse.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable;
            }
        }

        private bool createMovilDB()
        {
            bool booOk = false;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("DROP TABLE IF EXISTS \"TMUsuario\";");
                sb.AppendLine("CREATE TABLE \"TMUsuario\" " +
                              "(" +
                                    "\"intId\" INTEGER PRIMARY KEY," +
                                    "\"strUsuario\" TEXT DEFAULT (null)," +
                                    "\"strPassword\" TEXT DEFAULT (null)," +
                                    "\"intIdPerfil\" INTEGER," +
                                    "\"intActivo\" INTEGER," +
                                    "\"UltimaActualizacion\" TEXT DEFAULT (null)" +
                              ");");
                sb.AppendLine("DROP TABLE IF EXISTS \"TMPersona\";");
                sb.AppendLine("CREATE TABLE \"TMPersona\" " +
                              "(" +
                                    "\"intIdPersona\" INTEGER PRIMARY KEY," +
                                    "\"strNombrePersona\" TEXT DEFAULT (null)," +
                                    "\"intActivo\" INTEGER " +
                              ");");
                sb.AppendLine("DROP TABLE IF EXISTS \"TMPagoMensual\";");
                sb.AppendLine("CREATE TABLE \"TMPagoMensual\" " +
                             "(" +
                                   "\"intIdPago\" INTEGER PRIMARY KEY," +
                                   "\"intIdPersona\" TEXT DEFAULT (null)," +
                                   "\"strMonto\" TEXT DEFAULT (null)," +
                                   "\"intAnio\" TEXT DEFAULT (null)," +
                                   "\"intMes\" INTEGER," +
                                   "\"strMes\" TEXT DEFAULT (null)" +
                             ");");

                using (m_dbConnection = new SQLiteConnection(@"Data Source=" + HostingEnvironment.MapPath("~/BD/") + @"Escuela.sqlite;Version=3;"))
                {
                    m_dbConnection.Open();
                    using (var command = new SQLiteCommand(sb.ToString(), m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                booOk = true;
            }
            catch (Exception ex)
            {
                booOk = false;
            }
            return booOk;
        }

        private void getUsuario(bool sync)
        {
            if (!sync)
            {
                lstTMUsuario = TMUsuarioBL.Instancia.ListarUsuario();
            }
            else
            {

            }
        }

        private void getPagoMensual(bool sync)
        {
            if (!sync)
            {
                lstTMPagoMensual = TMPagoMensualBL.Instancia.ListarUsuario();
            }
            else
            {

            }
        }

        private void getPersona(bool sync)
        {
            if (!sync)
            {
                lstTMPersona = TMPersonaBL.Instancia.ListarPersona();
            }
            else
            {

            }
        }

        private void setUsuario()
        {
            StringBuilder sQuery = new StringBuilder("");
            SQLiteCommand sqlCommand;
            Int32 nPaginas = 0;
            try
            {
                nPaginas = ((Int32)(lstTMUsuario.Count / 100.0)) + (lstTMUsuario.Count % 100 == 0 ? 0 : 1);
                for (int j = 0; j < nPaginas; j++)
                {
                    sQuery.Clear();
                    sQuery.AppendLine("INSERT INTO TMUsuario (" +
                        "\"intId\"," +
                        "\"strUsuario\"," +
                        "\"strPassword\"," +
                        "\"intIdPerfil\"," +
                        "\"intActivo\"," +
                        "\"UltimaActualizacion\") VALUES");

                    for (int i = 100 * j; i < lstTMUsuario.Count && i < 100 * (j + 1); i++)
                    {
                        sQuery.AppendLine("('" +
                             Convert.ToInt32(lstTMUsuario[i].intId) + "','" +
                            lstTMUsuario[i].strUsuario.Trim().Replace("'", " ") + "','" +
                            lstTMUsuario[i].strPassword.Trim().Replace("'", " ") + "','" +
                             Convert.ToInt32(lstTMUsuario[i].intIdPerfil) + "','" +
                             Convert.ToInt32(lstTMUsuario[i].intActivo) + "','" +
                             lstTMUsuario[i].UltimaActualizacion.Trim().Replace("'", " ") + "'),");

                        if (i + 1 == 100 * (j + 1) || i + 1 == lstTMUsuario.Count)
                        {
                            //using (m_dbConnection = new SQLiteConnection(@"Data Source=" + HostingEnvironment.MapPath("~/BD/") + @"Escuela.sqlite;Version=3;"))
                            //{
                            //    m_dbConnection.Open();
                            //using (sqlCommand = new SQLiteCommand(sQuery.ToString().Substring(0, sQuery.ToString().ToString().Length - 3), m_dbConnection))
                            //{
                            //    sqlCommand.CommandType = System.Data.CommandType.Text;
                            //    sqlCommand.ExecuteNonQuery();
                            //}
                            //m_dbConnection.Close();
                            //}
                            using (sqlCommand = new SQLiteCommand(sQuery.ToString().Substring(0, sQuery.ToString().ToString().Length - 3), m_dbConnection))
                            {
                                sqlCommand.CommandType = System.Data.CommandType.Text;
                                sqlCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Console.WriteLine("Procesos Por Flujos Terminó.");
            }

            return;
        }

        private void setValor<T>(List<T> lst, String strNombreTabla)
        {
            StringBuilder sQuery = new StringBuilder("");
            SQLiteCommand sqlCommand;
            Int32 nPaginas = 0;
            try
            {
                nPaginas = ((Int32)(lst.Count / 100.0)) + (lst.Count % 100 == 0 ? 0 : 1);
                for (int j = 0; j < nPaginas; j++)
                {
                    sQuery.Clear();
                    String strQuery = string.Empty;
                    strQuery = "INSERT INTO " + strNombreTabla + "(";
                    T tp = Activator.CreateInstance<T>();
                    PropertyInfo[] propertyInfos = typeof(T).GetProperties();
                    var conta = propertyInfos.Count();
                    var intIni = 0;
                    for (int i = 100 * j; i < lst.Count && i < 100 * (j + 1); i++)
                    {
                        foreach (PropertyInfo propertyInfo in propertyInfos)
                        {
                            if (tp.GetType().GetProperty(propertyInfo.Name).GetValue(lst[i]) != null)
                            {
                                strQuery += @"" + propertyInfo.Name + @",";
                            }
                        }
                        break;
                    }
                    strQuery = strQuery.Trim().Substring(0, strQuery.ToString().Length - 1).ToString();
                    strQuery += ") VALUES";
                    intIni = 0;
                    for (int i = 100 * j; i < lst.Count && i < 100 * (j + 1); i++)
                    {
                        strQuery += "('";
                        foreach (PropertyInfo propertyInfo in propertyInfos)
                        {
                            if (tp.GetType().GetProperty(propertyInfo.Name).GetValue(lst[i]) != null)
                                strQuery += tp.GetType().GetProperty(propertyInfo.Name).GetValue(lst[i]).ToString() + "','";
                        }
                        if (strQuery.EndsWith("','"))
                        {
                            strQuery = strQuery.Trim().Substring(0, strQuery.ToString().Length - 3) + "'),";
                        }
                        if (i + 1 == 100 * (j + 1) || i + 1 == lst.Count)
                        {
                            sQuery.AppendLine(strQuery);
                            //using (m_dbConnection = new SQLiteConnection(@"Data Source=" + HostingEnvironment.MapPath("~/BD/") + @"\Escuela.sqlite;Version=3;"))
                            //{
                            //    m_dbConnection.Open();
                            //    using (sqlCommand = new SQLiteCommand(sQuery.ToString().Substring(0, sQuery.ToString().ToString().Length - 3), m_dbConnection))
                            //    {
                            //        sqlCommand.CommandType = System.Data.CommandType.Text;
                            //        sqlCommand.ExecuteNonQuery();
                            //    }
                            //}
                            using (sqlCommand = new SQLiteCommand(sQuery.ToString().Substring(0, sQuery.ToString().ToString().Length - 3), m_dbConnection))
                            {
                                sqlCommand.CommandType = System.Data.CommandType.Text;
                                sqlCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Console.WriteLine("Procesos Por Flujos Terminó.");
            }

            return;
        }

        private string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        private string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

    }
}
