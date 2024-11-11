using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Reflection;
//using CF.Core.Config;

namespace CF.SQLServer.DAL
{
    public class SQLHelper
    {
        public SqlConnection Conn;
        //public string strCon = CachedConfigContext.Current.DaoConfig.Crm;
        public string strCon = "";
        public SQLHelper(string dbStrCon)
        {
            strCon = dbStrCon;
        }

        private SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }

        private SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    if (parameter != null)
                    {
                        if (((parameter.Direction == ParameterDirection.InputOutput) || (parameter.Direction == ParameterDirection.Input)) && (parameter.Value == null))
                        {
                            parameter.Value = DBNull.Value;
                        }
                        command.Parameters.Add(parameter);
                    }
                }
            }
            return command;
        }

        public void close()
        {
            if ((Conn != null) && (Conn.State == ConnectionState.Open))
            {
                Conn.Close();
            }
        }

        public int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (Conn = new SqlConnection(strCon))
            {
                PrepareCommand(cmd, Conn, null, cmdType, cmdText, commandParameters);
                int num = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                Conn.Close();
                return num;
            }
        }

        private SqlConnection getConn()
        {
            return new SqlConnection(strCon);
        }

        private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        public SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            SqlConnection connection = getConn();
            try
            {
                connection.Open();
                SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
                command.CommandType = CommandType.StoredProcedure;
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch
            {
                return null;
            }
        }

        public int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (SqlConnection connection = new SqlConnection(strCon))
            {
                connection.Open();
                SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                int num = (int)command.Parameters["ReturnValue"].Value;
                connection.Close();
                return num;
            }
        }

        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(strCon))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                adapter.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }

        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
        {
            using (SqlConnection connection = new SqlConnection(strCon))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                adapter.SelectCommand.CommandTimeout = Times;
                adapter.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }


        /// <summary>
        /// 執行SQL，返回 dataTable 類型
        /// </summary>
        /// <returns></returns>
        public DataTable ExecuteSqlReturnDataTable(string strSQL)
        {
            using (SqlConnection connection = new SqlConnection(strCon))
            {
                DataTable dtData = new DataTable();
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = strSQL;
                //cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 2400;//連接20分鐘
                //cmd.Parameters.AddRange(paras);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dtData);
                sda.Dispose();
                return dtData;
            }
        }

        /// <summary>
        /// 執行SQL，返回 dataSet 類型
        /// </summary>
        /// <returns></returns>
        public DataSet ExecuteSqlReturnDataSet(string strSQL)
        {
            using (SqlConnection connection = new SqlConnection(strCon))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sda = new SqlDataAdapter(strSQL, connection);
                sda.Fill(dataSet);
                sda.Dispose();
                connection.Close();
                return dataSet;
            }            
        }

        /// <summary>
        ///執行存儲過程，返回dataTable 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public DataTable ExecuteProcedureRetrunDataTable(string strSql, SqlParameter[] paras)
        {
            using (SqlConnection connection = new SqlConnection(strCon))
            {
                DataTable dtData = new DataTable();
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = strSql;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 2400;//連接20分鐘
                cmd.Parameters.AddRange(paras);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dtData);
                sda.Dispose();
                return dtData;
            }
        }

        public string ExecuteSqlUpdate(string strSql)
        {
            string result = "";
            Conn = new SqlConnection(strCon);
            Conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Conn;
                cmd.CommandText = strSql;
                cmd.CommandTimeout = 1200;//連接20分鐘
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                result = ex.Message;//
            }
            finally
            {
                Conn.Close();
            }

            return result;
        }

        /// <summary>
        /// 轉換為GEO中對應的語言
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public string ConvertLanguage(string language)
        {
            string LanguageID = "";
            if (string.IsNullOrEmpty(language))
            {
                language = "0";//WEB中登入的語言,0:繁體中文
            }                   
            switch (language)
            {
                //GEO中語言:3.繁體中文,2.英文,1.簡體中文
                case "0"://繁體中文
                    LanguageID = "3";//GEO中3:為繁體中文
                    break;
                case "2"://英文
                    LanguageID = "2";
                    break;
                default:
                    LanguageID = "3";
                    break;
            }
            return LanguageID;
        }

        /// <summary>
        /// DataTable轉換為Json字符串格式
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public string DataTableToJson(DataTable table)
        {
            var jsonString = new StringBuilder();
            //var keyValue = "";
            if (table.Rows.Count > 0)
            {
                jsonString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    jsonString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        //keyValue = string.IsNullOrEmpty(table.Rows[i][j].ToString()) ? "" : table.Rows[i][j].ToString();//2022/07/14 add
                        //keyValue = keyValue.Replace("/", "//");
                        if (j < table.Columns.Count - 1)
                        {
                            jsonString.Append("\"" + table.Columns[j].ColumnName.ToString()+ "\":" + "\"" + table.Rows[i][j].ToString() + "\","); //2022/07/14 cancel
                            //jsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + keyValue + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            jsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                            //jsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + keyValue + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        jsonString.Append("}");
                    }
                    else
                    {
                        jsonString.Append("},");
                    }
                }
                jsonString.Append("]");
            }          
            return jsonString.ToString(); 
        }

        /// <summary>
        /// 執行SQL 語句，返回string
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public String ExecuteSqlReturnObject(string strSQL)
        {
            string objStrValue = "";
            try
            {
                DataTable dts = GetDataTable(strSQL);
                if (dts.Rows.Count > 0)
                {
                    objStrValue = dts.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                objStrValue = ex.Message;
            }
            return objStrValue;
        }

        public DataTable GetDataTable(string strSQL)
        {
            DataTable dtData = new DataTable();          
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 2400;//連接20分鐘
                cmd.CommandText = strSQL;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dtData);
                sda.Dispose();
            }
            return dtData;
        }



    }
}
