using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WSServiceEPJ.Utility
{
    public class ReaderUtility
    {
        /// <summary>
        /// Mapear Objeto a un obteto especificado
        /// </summary>
        /// <param name="pDbrLectu">IDataReader</param>
        /// <typeparam name="T">Obtejo</typeparam>
        /// <returns>Objeto con parametros llenos</returns>
        public static T MapearObjeto<T>(IDataReader pDbrLectu)
        {
            T tp = Activator.CreateInstance<T>();
            string strNoCampo = string.Empty;
            MemberInfo[] members = tp.GetType().GetMembers();
            int num = pDbrLectu.FieldCount - 1;
            for (int i = 0; i <= num; i++)
            {
                try
                {
                    strNoCampo = pDbrLectu.GetName(i).Trim();
                    MemberInfo memberInfo = (
                    from vt in members
                    //where System.String.Compare(vt.Name.Trim(), strNoCampo.Trim(), System.StringComparison.OrdinalIgnoreCase) == 0
                    where string.Compare(vt.Name.Trim(), strNoCampo.Trim(), true) == 0
                    select vt).FirstOrDefault<MemberInfo>();
                    strNoCampo = memberInfo.Name.Trim();
                    if (!pDbrLectu.IsDBNull(pDbrLectu.GetOrdinal(strNoCampo)))
                    {
                        Type propertyType = tp.GetType().GetProperty(strNoCampo).PropertyType;
                        string name = propertyType.Name;
                        if (string.Compare(name, typeof(string).Name, false) == 0)
                        {
                            tp.GetType().GetProperty(strNoCampo).SetValue(tp, pDbrLectu.GetString(pDbrLectu.GetOrdinal(strNoCampo)), null);
                        }
                        if (string.Compare(name, typeof(int).Name, false) == 0)
                        {
                            tp.GetType().GetProperty(strNoCampo).SetValue(tp, pDbrLectu.GetInt32(pDbrLectu.GetOrdinal(strNoCampo)), null);
                        }
                        if (string.Compare(name, typeof(DateTime).Name, false) == 0)
                        {
                            tp.GetType().GetProperty(strNoCampo).SetValue(tp, pDbrLectu.GetDateTime(pDbrLectu.GetOrdinal(strNoCampo)), null);
                        }
                        if (string.Compare(name, typeof(bool).Name, false) == 0)
                        {
                            tp.GetType().GetProperty(strNoCampo).SetValue(tp, pDbrLectu.GetBoolean(pDbrLectu.GetOrdinal(strNoCampo)), null);
                        }
                        if (string.Compare(name, typeof(int).Name, false) == 0)
                        {
                            tp.GetType().GetProperty(strNoCampo).SetValue(tp, pDbrLectu.GetInt32(pDbrLectu.GetOrdinal(strNoCampo)), null);
                        }
                        if (string.Compare(name, typeof(long).Name, false) == 0)
                        {
                            tp.GetType().GetProperty(strNoCampo).SetValue(tp, pDbrLectu.GetInt64(pDbrLectu.GetOrdinal(strNoCampo)), null);
                        }
                        if (string.Compare(name, typeof(short).Name, false) == 0)
                        {
                            tp.GetType().GetProperty(strNoCampo).SetValue(tp, pDbrLectu.GetInt16(pDbrLectu.GetOrdinal(strNoCampo)), null);
                        }
                        if (string.Compare(name, typeof(byte).Name, false) == 0)
                        {
                            tp.GetType().GetProperty(strNoCampo).SetValue(tp, pDbrLectu.GetByte(pDbrLectu.GetOrdinal(strNoCampo)), null);
                        }
                        if (string.Compare(name, typeof(char).Name, false) == 0)
                        {
                            tp.GetType().GetProperty(strNoCampo).SetValue(tp, pDbrLectu.GetChar(pDbrLectu.GetOrdinal(strNoCampo)), null);
                        }
                        if (string.Compare(name, typeof(decimal).Name, false) == 0)
                        {
                            tp.GetType().GetProperty(strNoCampo).SetValue(tp, pDbrLectu.GetDecimal(pDbrLectu.GetOrdinal(strNoCampo)), null);
                        }
                        if (string.Compare(name, typeof(double).Name, false) == 0)
                        {
                            tp.GetType().GetProperty(strNoCampo).SetValue(tp, pDbrLectu.GetDouble(pDbrLectu.GetOrdinal(strNoCampo)), null);
                        }
                        if (string.Compare(name, typeof(byte[]).Name, false) == 0)
                        {
                            //byte[] array = null;
                            //if (!pDbrLectu.IsDBNull(pDbrLectu.GetOrdinal(strNoCampo)))
                            //{
                            //    long bytes = pDbrLectu.GetBytes(pDbrLectu.GetOrdinal(strNoCampo), 0L, null, 0, 0);
                            //    array = new byte[Convert.ToInt32(bytes) + 1];
                            //    pDbrLectu.GetBytes(pDbrLectu.GetOrdinal(strNoCampo), 0L, array, 0, (int)bytes);
                            //}
                            //tp.GetType().GetProperty(strNoCampo).SetValue(tp, array, null);
                            int bufferSize = 64;
                            // The BLOB byte[] buffer to be filled by GetBytes.
                            byte[] outByte = new byte[bufferSize];
                            long retval;
                            long startIndex = 0;
                            retval = pDbrLectu.GetBytes(pDbrLectu.GetOrdinal(strNoCampo), startIndex, outByte, 0, bufferSize);

                            // Continue while there are bytes beyond the size of the buffer.
                            while (retval == bufferSize)
                            {
                                //writer.Write(outByte);
                                //writer.Flush();

                                // Reposition start index to end of last buffer and fill buffer.
                                startIndex += bufferSize;
                                retval = pDbrLectu.GetBytes(pDbrLectu.GetOrdinal(strNoCampo), startIndex, outByte, 0, bufferSize);
                            }



                            byte[] Bytess = (byte[])pDbrLectu[strNoCampo];
                            byte[] array = null;
                            if (!pDbrLectu.IsDBNull(pDbrLectu.GetOrdinal(strNoCampo)))
                            {
                                long bytes = pDbrLectu.GetBytes(pDbrLectu.GetOrdinal(strNoCampo), 0L, null, 0, 0);
                                array = new byte[Convert.ToInt32(bytes) + 1];
                                pDbrLectu.GetBytes(pDbrLectu.GetOrdinal(strNoCampo), 0L, array, 0, (int)bytes);
                            }
                            tp.GetType().GetProperty(strNoCampo).SetValue(tp, outByte, null);
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("El campo " + strNoCampo + " no se encuentra correctamente definido");
                }
            }
            return tp;
        }
    }
}
