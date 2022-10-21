using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;

namespace CF.Framework.Utility
{
    /// <summary>  
    /// 将DataTable转换成泛型集合IList<>助手类  
    /// </summary>  
    public class ConvertHelper
    {

        /// <summary>
        /// List泛型转换DataTable.
        /// </summary>
        /// 

        public DataTable ListToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        /// <summary>
        /// model转换DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public DataTable ModelToDataTable<T>(T items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }


            var values = new object[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(items, null);
            }

            tb.Rows.Add(values);


            return tb;
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }


        /// <summary>  
        /// 单表查询结果转换成泛型集合  
        /// </summary>  
        /// <typeparam name="T">泛型集合类型</typeparam>  
        /// <param name="dt">查询结果DataTable</param>  
        /// <returns>以实体类为元素的泛型集合</returns>  
        public static List<T> DataTableToList<T>(DataTable dt) where T : new()
        {
            // 定义集合  
            List<T> ts = new List<T>();

            // 获得此模型的类型  
            Type type = typeof(T);
            //定义一个临时变量  
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行   
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性  
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性  
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量    
                    //检查DataTable是否包含此列（列名==对象的属性名）      
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter    
                        if (!pi.CanWrite) continue;//该属性不可写，直接跳出    
                        //取值    
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性    
                        if (value != DBNull.Value)
                        {
                            try
                                {
                                if (dr[tempName].GetType().Name == "DateTime")
                                    pi.SetValue(t, Converter.ConvertFieldToCnDataString(dr[tempName].ToString()), null);
                                else
                                    pi.SetValue(t, value, null);
                            }
                            catch(Exception ex)
                            {
                                var errorMsg = ex.Message;
                            }
                        }
                    }
                }
                //对象添加到泛型集合中  
                ts.Add(t);
            }

            return ts;
        }


        public static T DataTableToModel<T>(DataTable dt) where T : new()
        {
            // 定义实体    
            T t = new T();
            //StringUtil aa = new StringUtil();
            //string t = new string();
            // 获得此模型的类型   
            Type type = typeof(T);
             string tempName = "";
 
             foreach (DataRow dr in dt.Rows)
             {
 
                 // 获得此模型的公共属性      
                 PropertyInfo[] propertys = t.GetType().GetProperties();
                 foreach (PropertyInfo pi in propertys)
                 {
                     tempName = pi.Name;  // 检查DataTable是否包含此列    
 
                     if (dt.Columns.Contains(tempName))
                     {
                         // 判断此属性是否有Setter      
                         if (!pi.CanWrite) continue;
 
                         object value = dr[tempName];
                        if (value != DBNull.Value)
                        {
                            try
                            {
                                if (dr[tempName].GetType().Name == "DateTime")
                                    pi.SetValue(t, Converter.ConvertFieldToCnDateTimeString(dr[tempName].ToString()), null);
                                else
                                    pi.SetValue(t, value, null);
                            }
                            catch(Exception ex)
                            {
                                var errorMsg = ex.Message;
                            }
                        }
                     }
                 }
                 break;
             }
             return t;
         }

        public static string DataTableJsonReturnExcel(DataTable dt)
        {
            StringBuilder json = new StringBuilder();
            StringBuilder jsonBuilder = new StringBuilder();
            json.Append("[");



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    string aa = WipeRiskString(dt.Rows[i][j].ToString().Trim());
                    jsonBuilder.Append(WipeRiskString(dt.Rows[i][j].ToString().Trim()));
                    jsonBuilder.Append("\",");
                }
                if (dt.Columns.Count > 0)
                {
                    jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                }
                jsonBuilder.Append("},");
            }
            if (dt.Rows.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }

            json.Append(jsonBuilder.ToString());
            json.Append("]");

            return json.ToString();
        }

        #region dataTable转换成Json格式  
        /// <summary>       
        /// dataTable转换成Json格式       
        /// </summary>       
        /// <param name="dt"></param>       
        /// <returns></returns>       
        public static string DataTableJsonReturnTextBox(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"");
            jsonBuilder.Append(dt.TableName.ToString());
            jsonBuilder.Append("\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }


        public static string DataTableJsonReturnList(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            //jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        //JASON格式，返回給EasyUI Table使用
        public static string DataTableJsonReturnTable(DataTable dt)
        {
            StringBuilder json = new StringBuilder();
            StringBuilder jsonBuilder = new StringBuilder();
            json.Append("{\"total\":");
            json.Append(dt.Rows.Count);
            json.Append(",\"rows\":[");



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    string aa = WipeRiskString(dt.Rows[i][j].ToString().Trim());
                    jsonBuilder.Append(WipeRiskString(dt.Rows[i][j].ToString().Trim()));
                    jsonBuilder.Append("\",");
                }
                if (dt.Columns.Count > 0)
                {
                    jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                }
                jsonBuilder.Append("},");
            }
            if (dt.Rows.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }

            json.Append(jsonBuilder.ToString());
            json.Append("]}");

            return json.ToString();
        }

        #endregion



        //去處非法的字符
        public static string WipeRiskString(string fstr)
        {
            string tstr = fstr;
            tstr = tstr.Replace("\r\n", "");
            tstr = tstr.Replace("\r", "");
            tstr = tstr.Replace("\n", "");
            tstr = tstr.Replace("\\", "");//SBL012647
            tstr = tstr.Replace("\u0002", "");//GBV043482
            tstr = tstr.Replace("\t", "");
            tstr = tstr.Replace("%", "");
            tstr = tstr.Replace("!", "");
            tstr = tstr.Replace("\"", "");
            tstr = tstr.Replace("”", "");
            tstr = tstr.Replace("“", "");
            //tstr = tstr.Replace(".", "");
            //tstr = tstr.Replace("~", "");
            tstr = tstr.Replace("{", "");
            tstr = tstr.Replace("}", "");
            tstr = tstr.Replace("?", "");
            return tstr;
        }
    }
}
