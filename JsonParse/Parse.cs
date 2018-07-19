using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonParse
{
    public class Parse
    {
        //private string FilePath { get; set; }
        private string TableName { get; set; }
        private Stream InputStream { get; set; }
        public int Capacity { get; set; }
        private string Connstr { get; set; }
        private Encoding Encoding { get; set; }

        private int RowCount { get; set; }
        public int DataCount = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputstream"></param>
        /// <param name="connstr">连接字符串</param>
        /// <param name="TableName">表名</param>
        /// <param name="Capacity">缓冲区大小</param>
        /// <param name="encoding">编码</param>
        public Parse(Stream inputstream, string connstr, string TableName, int? Capacity = null, Encoding encoding = null)
        {
            inputstream.Seek(0, SeekOrigin.Begin);
            this.InputStream = inputstream;
            //this.FilePath = FilePath;
            this.TableName = TableName;
            this.Connstr = connstr;
            if (Capacity == null)
            {
                this.Capacity = 4096 * 10;
            }
            else
            {
                this.Capacity = (int)Capacity;
            }
            if (encoding == null)
            {
                this.Encoding = System.Text.Encoding.UTF8;
            }
            else
            {
                this.Encoding = encoding;
            }
            //if (RowCount == null)
            //{
            this.RowCount = (int)ExecuteScalar(string.Format("select count(*) from syscolumns s  where s.id = object_id('{0}');", TableName));
            //}
            //else
            //{
            //    this.RowCount = (int)RowCount;
            //}

        }
        public void Start()
        {
            //FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate);
            byte[] buffer = new byte[Capacity];
            int len = 0;
            byte[] lastbyte = null;
            int i = 0;
            while ((len = InputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string body = null;
                if (lastbyte != null)
                {
                    //待优化
                    byte[] tempbuffer = new byte[len];
                    Array.Copy(buffer, 0, tempbuffer, 0, len);
                    byte[] newbuffer = new byte[lastbyte.Length + len];
                    newbuffer = ArraySum(lastbyte, tempbuffer);
                    body = this.Encoding.GetString(newbuffer);

                }
                else
                {
                    body = this.Encoding.GetString(buffer, 0, len);

                }
                //125 45
                int position = 0;
                for (int ii = buffer.Length - 1; ii >= 0; ii--)
                {
                    if (buffer[ii] == 125)
                    {//},{ 125 44 123
                        //if (ii + 1 < buffer.Length)
                        //{
                        //    if (buffer[ii + 1] == 44)
                        //    {
                        //        position = ii + 2;
                        //        break;
                        //    }
                        //}
                        //else
                        //{
                        //    position = ii + 1;
                        //    break;
                        //}
                        position = ii + 1;
                        break;
                    }
                }
                //Console.WriteLine(body);
                string[] banks = body.Split(new string[1] { "}," }, StringSplitOptions.None);
                string sql = "";
                for (int iiii = 0; iiii < banks.Length; iiii++)
                {//获取每一条数据
                    string bank = banks[iiii];
                    string[] kvs = bank.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string fieid = null;
                    string parameter = null;

                    if (kvs.Length < RowCount)
                    {
                        continue;
                    }
                    if (iiii == banks.Length - 1)
                    {
                        if (!kvs[kvs.Length - 1].Contains('}'))
                        {
                            continue;
                        }
                    }
                    for (int k = 0; k < kvs.Length; k++)
                    {
                        string kv = kvs[k];
                        if (kv.Contains(":"))
                        {
                            string value = kv.Split(':')[1];
                            string key = kv.Split(':')[0];

                            //TODO:
                            key = key.Replace("\"", "").Replace("{", "").Replace("[", "").Replace("}", "").Replace("]", "");
                            value = value.Replace("{", "").Replace("[", "").Replace("}", "").Replace("]", "");
                            //if (value.Length > 1 && value.Substring(value.Length - 1) == "\"")
                            //{
                            fieid += key + ',';
                            //}
                            value = value.Replace("\"", "");

                            if (value == "NULL" || value == "null")
                            {
                                parameter += "'" + "" + "'" + ",";
                            }
                            else if (value.Length == 0)
                            {
                                parameter += "'" + "" + "'" + ",";
                            }
                            else
                            {
                                parameter += "'" + value + "'" + ",";
                            }
                        }
                    }

                    if (fieid != null)
                    {
                        var fieidlength = (fieid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length);

                        if (RowCount == fieidlength)
                        {
                            fieid = fieid.Substring(0, fieid.Length - 1);
                            parameter = parameter.Substring(0, parameter.Length - 1);
                            //Console.WriteLine(parameter);
                            Log(parameter);
                            sql += "insert into " + TableName + "(" + fieid + ") values(" + parameter + ")";
                            DataCount++;
                        }
                    }

                }
                if (!string.IsNullOrEmpty(sql))
                {
                    ExecuteNonQuery(sql);

                }


                byte[] lastb = new byte[buffer.Length - position];
                for (int iii = 0; iii < lastb.Length; iii++)
                {
                    lastb[iii] = buffer[iii + position];
                }
                lastbyte = lastb;
                //File.WriteAllText(@"C:\Users\ZZJ\Desktop\data\" + TableName + i + ".txt", body);
                i++;
                Console.WriteLine("进度:{0}%,{1}", Math.Round((InputStream.Position / (double)InputStream.Length) * 100, 2), i);

            }
            Console.WriteLine("插入了{0}条数据", DataCount);
        }

        public static byte[] ArraySum(byte[] bs1, byte[] bs2)
        {
            long len = bs1.Length + bs2.Length;
            byte[] newbytes = new byte[len];
            for (int i = 0; i < bs1.Length; i++)
            {
                newbytes[i] = bs1[i];
            }
            for (int i = bs1.Length; i < len; i++)
            {
                newbytes[i] = bs2[i - bs1.Length];
            }
            return newbytes;
        }

        public int ExecuteNonQuery(string sql, params IDbDataParameter[] parameters)
        {
            using (IDbConnection conn = new SqlConnection(this.Connstr))
            using (SqlCommand cmd = (SqlCommand)conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(string sql, params IDbDataParameter[] parameter)
        {
            using (IDbConnection conn = new SqlConnection(this.Connstr))
            using (SqlCommand cmd = (SqlCommand)conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(parameter);
                return cmd.ExecuteScalar();
            }
        }

        public static void Log(string Msg)
        {
            //Console.WriteLine(Msg);

        }
    }
}
