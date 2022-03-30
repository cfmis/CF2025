using CF.Core.Config;
using CF.SQLServer.DAL;
using CF2025.Base.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Base.DAL
{
    public class CommonDAL
    {
        private static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);

        /// <summary>
        /// 欄位名稱下拉列表框
        /// </summary>
        /// <param name="window_id">頁面ID</param>
        /// <param name="language_id">標簽語言翻譯</param>
        /// <returns></returns>
        public static List<ModelQueryList> GetFieldNameList(string window_id, string language_id)
        {
            string LanguageID = sh.ConvertLanguage(language_id);
            string strSql = string.Format(
            @"SELECT A.field_name AS value,B.col_name AS label,A.table_name,A.field_type,
            ISNULL(A.from_table,'') AS from_table,ISNULL(A.table_relation,'') AS table_relation
            FROM sys_query_initialize A Left join sys_dictionary B ON a.field_desc=b.col_code
            WHERE A.window_id ='{0}' AND B.language_id='{1}'
            ORDER BY A.sequence_id", window_id, LanguageID);
            DataTable dtQuery = sh.ExecuteSqlReturnDataTable(strSql);
            List<ModelQueryList> lst = new List<ModelQueryList>();
            //在第一行添加一行空記錄
            ModelQueryList obj1 = new ModelQueryList();
            obj1.value = "";
            obj1.label = "";
            obj1.from_table = "";
            obj1.table_relation = "";
            lst.Add(obj1);
            for (int i = 0; i < dtQuery.Rows.Count; i++)
            {
                DataRow dr = dtQuery.Rows[i];
                ModelQueryList obj = new ModelQueryList();
                obj.value = dr["value"].ToString();
                obj.label = dr["label"].ToString().Trim();
                obj.table_name = dr["table_name"].ToString();
                obj.field_type = dr["field_type"].ToString();
                obj.from_table = dr["from_table"].ToString();
                obj.table_relation = dr["table_relation"].ToString();
                lst.Add(obj);
            }
            return lst;
        }

        /// <summary>
        /// 已保存用戶ID對應頁面通用查詢數據
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="window_id"></param>
        /// <returns></returns>
        public static List<ModelQuerySavedList> GetSavedList(string user_id, string window_id)
        {
            string strSql = string.Format(
            @"SELECT A.id,A.window_id,A.field_name,A.operators,A.field_value,A.logic,A.table_name,A.sequence_id,B.field_type 
            FROM sys_query_users A LEFT JOIN sys_query_initialize B ON A.field_name=B.field_name AND A.table_name=B.table_name
            WHERE A.user_no='{0}' AND A.window_id='{1}' 
            ORDER BY A.sequence_id", user_id, window_id);
            List<ModelQuerySavedList> lst = new List<ModelQuerySavedList>();
            DataTable dtSaved = sh.ExecuteSqlReturnDataTable(strSql);
            for (int i = 0; i < dtSaved.Rows.Count; i++)
            {
                DataRow dr = dtSaved.Rows[i];
                ModelQuerySavedList obj = new ModelQuerySavedList();
                obj.id = Int32.Parse(dr["id"].ToString());
                obj.window_id = dr["window_id"].ToString();
                obj.field_name = dr["field_name"].ToString();
                obj.operators = dr["operators"].ToString().Trim();
                obj.field_value = dr["field_value"].ToString().Trim();
                obj.logic = dr["logic"].ToString().Trim();
                obj.table_name = dr["table_name"].ToString();
                obj.field_type = dr["field_type"].ToString().Trim();
                obj.sequence_id = dr["sequence_id"].ToString().Trim();
                obj.row_status = "EDIT";
                lst.Add(obj);
            }
            return lst;
        }

        //更新通用查詢
        public static string SaveQueryList(string user_id, List<ModelQuerySavedList> lstMdl)
        {
            string result = "";
            string strSql = "";
            strSql += string.Format(@" SET XACT_ABORT ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");
            //更新明細
            if (lstMdl != null)
            {
                string sql_details_i = "", sql_details_u = "";
                for (int i = 0; i < lstMdl.Count; i++)
                {
                    //明細新增
                    sql_details_i = string.Format(
                        @" Insert Into sys_query_users(user_no,window_id,field_name,operators,field_value,logic,sequence_id,table_name,create_by,create_date)
                        VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',getdate())", user_id, lstMdl[i].window_id, lstMdl[i].field_name,
                        lstMdl[i].operators, lstMdl[i].field_value, lstMdl[i].logic, lstMdl[i].sequence_id, lstMdl[i].table_name, user_id);
                    sql_details_u = string.Format(
                        @" UPDATE sys_query_users 
                        SET user_no='{0}',window_id='{1}',field_name='{2}',operators='{3}',field_value='{4}',logic='{5}',sequence_id='{6}',table_name='{7}',
                            update_by='{8}',update_date=getdate()
                        WHERE id='{9}'", user_id, lstMdl[i].window_id, lstMdl[i].field_name, lstMdl[i].operators, lstMdl[i].field_value, lstMdl[i].logic,
                        lstMdl[i].sequence_id, lstMdl[i].table_name, user_id, lstMdl[i].id);
                    //新增狀態
                    if (lstMdl[i].row_status == "NEW")
                    {
                        strSql += sql_details_i;
                    }
                    //編輯狀態
                    if (lstMdl[i].row_status == "EDIT")
                    {
                        strSql += sql_details_u;
                    }
                    //刪除狀態
                    if (lstMdl[i].row_status == "DEL")
                    {
                        strSql += string.Format(@" DELETE FROM sys_query_users WHERE id={0}", lstMdl[i].id);
                    }
                }
            }
            strSql += string.Format(@" COMMIT TRANSACTION ");
            result = sh.ExecuteSqlUpdate(strSql);
            return result;//成功返回空格
        }

        public static string QueryList(string sqlText)
        {
            DataTable dt = sh.ExecuteSqlReturnDataTable(sqlText);
            var Result = sh.DataTableToJson(dt);
            return Result;
        }
        
        /// <summary> 
        /// 利用反射將DataTable转换为List<T>对象
        /// </summary> 
        /// <param name="dt">DataTable 对象</param> 
        /// <returns>List<T>集合</returns> 
        public static List<T> DataTableToList<T>(DataTable dt) where T : class, new()
        {
            // 定义集合 
            List<T> ts = new List<T>();
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
                    //將属性名称赋值给临时变量 
                    tempName = pi.Name;
                    //检查DataTable是否包含此列（列名==对象的属性名）  
                    if (dt.Columns.Contains(tempName))
                    {
                        //取值 
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性 
                        if (value != DBNull.Value)
                        {
                            pi.SetValue(t, value, null);
                        }
                    }
                }
                //对象添加到泛型集合中 
                ts.Add(t);
            }
            return ts;
            //使用方式：Entity即為要轉成List的數據模型,必須預先定義,省不了
            //List<Entity> list = CommonDAL.DataTableToList<Entity>(dt);
        }

    }
}
