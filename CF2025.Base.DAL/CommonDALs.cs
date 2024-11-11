using CF.Core.Config;
using CF.SQLServer.DAL;
using CF2025.Base.Contract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Base.DAL
{
    public class CommonDAL
    {
        private static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        private static PubFunDAL pubFun = new PubFunDAL();

        public static string GeoEncrypt(string strEncrypt)
        {
            //函數說明：傳入用戶密碼(原碼),返回加密之後的字串
            //參數：as_code(輸入的密碼原碼).
            //返回值：ls_EncryptPass 加密之後的字串
            //ChingFung可以寫一個類似的函數，將加密之後的字串與目前資料庫中保存的密碼去比較,如果相等就表示輸入的密碼正確。
            //定義函數使用到的變數
            string ls_TempString, ls_Work, ls_EncryptPass, ls_DecryptString, ls_EncryptString, as_code;
            int li_Length, li_Position, li_Multiplier, li_Offset, li_Count;
            as_code = strEncrypt;// 輸入的密碼
            //--將輸入的密碼轉化為小寫字元
            ls_TempString = as_code.ToLower();
            //定義加密解密的字串,這一些字元是固定的.不允許修改.
            //以雙引號開始,同樣以雙引號結束，比如："123456",表示123456這幾個字元.

            ls_DecryptString = " !" + "\"" + "#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[" + "\\" + "]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            ls_EncryptString = "~{[}u;Ce83KX%:VIm!|gs]_aL-QEOpx<UlzZjBq6#1($" + "\\" + "\"" + "FS5H0'cM&>Po.NGA*Jr)Y" + " " + "Dv/t9kd?^fni,hR2Wy=`+4T@7wb";


            //取得輸入的密碼長度
            li_Length = as_code.Trim().Length;
            //--根據不同的密碼長度得到不同的加密方法的字元長度倍數
            if (1 <= li_Length && li_Length <= 3)
                li_Multiplier = 1;
            else
                if (4 <= li_Length && li_Length <= 6)
                li_Multiplier = 2;
            else
                    if (7 <= li_Length && li_Length <= 9)
                li_Multiplier = 3;
            else
                li_Multiplier = 4;
            ls_EncryptPass = "";//先將保存加密之後字串清空。

            //以下為迴圈密碼每一位元字元,對每一位元字元進行加密
            for (li_Count = 1; li_Count <= li_Length; li_Count++)
            {
                li_Offset = li_Count * li_Multiplier;
                //取密碼中的第li_count位元的字元，長度為1
                //使用方法：Mid(需要取值的字串,開始位置，長度)
                ls_Work = as_code.Substring(li_Count - 1, 1);//SUBSTR(as_code, li_Count, 1);
                //取到ls_work字元在ls_decryptstring中的第一個位置
                //使用方法：Pos(用來查找的字串，需要查找的字串)
                li_Position = ls_DecryptString.IndexOf(ls_Work) + 1;//substring(ls_Work,ls_DecryptString);
                li_Position = li_Position + li_Offset;
                //Mod是取模函數，即取到Li_positon除以95之後的餘數
                li_Position = li_Position % 95;//Mod(li_Position, 95);
                //將li_position值加1 ，相當於li_postion = li_postion + 1
                li_Position = li_Position + 1;
                //取到ls_EncryptString中第li_Position開始的1位元字元
                ls_Work = ls_EncryptString.Substring(li_Position - 1, 1);//SUBSTR(ls_EncryptString, li_Position, 1);
                //將加密之後的字元相加,得到加密結果字串.
                ls_EncryptPass = ls_EncryptPass + ls_Work;
                //重新設置加密方法的字元長度倍數
                if (1 <= li_Multiplier && li_Multiplier <= 3)
                    li_Multiplier = li_Multiplier + 1;
                else
                    li_Multiplier = 1;
            }
            //--將加密之後的字元返回
            return ls_EncryptPass;
        }

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
            @"SELECT A.field_name AS value,CASE WHEN LEN(ISNULL(B.col_name,''))>0 THEN B.col_name ELSE a.field_desc END AS label,
            A.table_name,A.field_type,ISNULL(A.from_table,'') AS from_table,ISNULL(A.table_relation,'') AS table_relation,A.order_by,col_width
            FROM sys_query_initialize A Left join sys_dictionary B ON A.field_desc=B.col_code
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
            obj1.order_by = "";
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
                obj.order_by = dr["order_by"].ToString();
                obj.col_width = int.Parse(dr["col_width"].ToString());
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
            FROM sys_query_users A LEFT JOIN sys_query_initialize B ON A.window_id=B.window_id And A.field_name=B.field_name AND A.table_name=B.table_name
            WHERE A.user_no='{0}' AND A.window_id='{1}' 
            ORDER BY A.sequence_id", user_id, window_id);
            List<ModelQuerySavedList> lst = new List<ModelQuerySavedList>();
            DataTable dtSaved = sh.ExecuteSqlReturnDataTable(strSql);
            for (int i = 0; i < dtSaved.Rows.Count; i++)
            {
                DataRow dr = dtSaved.Rows[i];
                ModelQuerySavedList obj = new ModelQuerySavedList();
                obj.id = int.Parse(dr["id"].ToString());
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

        public static List<ModelItemQuery> ItemQueryList(ModelItemQuery SearchAry)
        {
            SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@results",SearchAry.results),
                new SqlParameter("@type",SearchAry.type),
                new SqlParameter("@blueprint_id",string.IsNullOrEmpty(SearchAry.blueprint_id)?"":SearchAry.blueprint_id),
                new SqlParameter("@goods_id",string.IsNullOrEmpty(SearchAry.goods_id)?"":SearchAry.goods_id),
                new SqlParameter("@goods_name",string.IsNullOrEmpty(SearchAry.goods_name)?"":SearchAry.goods_name),
                new SqlParameter("@modality",SearchAry.modality),
                new SqlParameter("@datum",string.IsNullOrEmpty(SearchAry.datum)?"":SearchAry.datum),
                new SqlParameter("@size_id",string.IsNullOrEmpty(SearchAry.size_id)?"":SearchAry.size_id),
                new SqlParameter("@big_class",string.IsNullOrEmpty(SearchAry.big_class)?"":SearchAry.big_class),
                new SqlParameter("@base_class",string.IsNullOrEmpty(SearchAry.base_class)?"":SearchAry.base_class),
                new SqlParameter("@small_class",string.IsNullOrEmpty(SearchAry.small_class)?"":SearchAry.small_class)
            };           
            DataSet dts = sh.RunProcedure("zz_base_item_query", paras,"items",20);            
            DataTable dt = dts.Tables[0];
            List<ModelItemQuery> lst = DataTableToList<ModelItemQuery>(dt);
            return lst;            
        }

        //檢查頁數是否正確
        public static string CheckMo(string mo_id)
        {
            string strSql = string.Format(@"Select mo_id FROM jo_bill_mostly a with(nolock) Where within_code='0000' and mo_id='{0}' and state not in('0','2','V')", mo_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            string result = "";
            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["mo_id"].ToString();
            }
            return result;
        }
        
        //檢查貨品編碼是否正確
        public static string CheckItem(string goods_id)
        {
            string strSql = string.Format(@"Select name as goods_name FROM it_goods a with(nolock) Where within_code='0000' and id='{0}' and state='0'", goods_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            string result = "";
            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["goods_name"].ToString();
            }
            return result;
        }

        /// <summary> 
        /// Allen 2022-02-20
        /// 利用反射將DataTable转换为List<T>对象
        /// 調用方式：List<Entity> list = CommonDAL.DataTableToList<Entity>(dt);
        /// Entity即為要轉成List的數據模型,必須預先定義,省不了
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
        }

        /// <summary>
        /// Allen 2022-08-23
        /// 集合轉成DataTable
        /// 調用方式：DataTable dt = CommonDAL.ListToDataTable<Entity>(listData);
        /// 參數:Entity 即為實體數據模型,必須預先定義,
        ///     listData,與Entity類相同的集合數據
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(IEnumerable<T> list)
        {
            PropertyInfo[] modelItemType = typeof(T).GetProperties();
            DataTable dataTable = new DataTable();
            dataTable.Columns.AddRange(modelItemType.Select(Columns => new DataColumn(Columns.Name, Columns.PropertyType)).ToArray());
            if (list.Count() > 0)
            {
                for (int i = 0; i < list.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in modelItemType)
                    {
                        object obj = pi.GetValue(list.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] dataRow = tempList.ToArray();
                    dataTable.LoadDataRow(dataRow, true);
                }
            }
            return dataTable;
        }
        
        //倉庫調整最大單據號
        public static string GetMaxID(string bill_id, int serial_len)
        {
            string strSql = string.Format(@"SELECT dbo.fn_zz_sys_bill_max_separate_st('{0}',{1}) as id", bill_id, serial_len);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            string id = dt.Rows[0]["id"].ToString(); // Return value DT10560134510
            return id;
        }

        //最大單據號
        public static string GetMaxID(string bill_id, string dept_id, int serial_len)
        {
            string strSql = string.Format("Select dbo.fn_zz_sys_bill_max_separate('{0}','{1}',{2}) as max_id", bill_id, dept_id, serial_len);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            string id = dt.Rows.Count > 0 ? dt.Rows[0]["max_id"].ToString() : "";
            return id;
        }

        //轉出單最大單據號
        public static string GetMaxID(string billId, string billType,string groupNo,int serialLen)
        {
            string strSql = string.Format("Select dbo.fn_zz_sys_bill_max_separate_out('{0}','{1}','{2}',{3}) as max_id", billId, billType, groupNo, serialLen);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            string id = dt.Rows.Count > 0 ? dt.Rows[0]["max_id"].ToString() : "";
            return id;
        }        

        //移交單最大單據號
        public static string GetMaxIDJo07(string out_dept, string in_dept, string doc_type)
        {
            string strSql = string.Format(@"SELECT dbo.fn_zz_sys_bill_max_jo07('{0}','{1}','{2}') as id", out_dept, in_dept, doc_type);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            string id = dt.Rows[0]["id"].ToString(); // Return value DT10560134510
            return id;
        }        

         //取單據批準狀態
        public static string CheckApproveState(string table_name, string id)
        {
            string strSql = string.Format("Select state From {0} with(nolock) WHERE within_code='0000' and id='{1}'", table_name, id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            string result = "0";
            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["state"].ToString();
            }
            return result;
        }

        //檢查用戶是否存在
        public static string GetUserName(string user_id)
        {
            string strSql = string.Format("Select user_name From sys_user WHERE within_code='' and user_id='{0}'", user_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            string result = "";
            if (dt.Rows.Count > 0)
                result = dt.Rows[0]["user_name"].ToString();
            return result;
        }
        //檢查用戶與密碼
        public static string GetUserInfo(string user_id, string password)
        {
            string strSql = string.Format("Select user_name,password From sys_user WHERE within_code='' and user_id='{0}'", user_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            string result = "";
            if (dt.Rows.Count > 0)
            {
                string strPwd = "";
                if (!string.IsNullOrEmpty(password))
                {
                    strPwd = GeoEncrypt(password);
                }                
                if (strPwd != dt.Rows[0]["password"].ToString())
                {
                    //密碼錯誤
                    result = "PASSWORD_ERROR";
                }
            }
            else
            {
                result = "USER_ID_ERROR";
            }           
            return result;
        }

        public static string GetDbDateTime(string type)
        {            
            string strSql = "";
            if (type == "S")
            {
                strSql = "SELECT CONVERT(varchar(10),GETDATE(),120) as dbdate";                
            }else
            {
                strSql= "SELECT CONVERT(varchar(19),GETDATE(),120) as dbdate";
            }
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            string result = dt.Rows[0]["dbdate"].ToString();
            return result;
        }

        public static string CheckCanApprove(string id,string window_id)
        {
            string result = "";
            int rtn = pubFun.wf_check_inventory_date(id,window_id);
            result = rtn.ToString();
            return result;
        }

        public static decimal QtyToSecQty(string within_code, string location_id, string goods_id, decimal qty)
        {
            decimal sec_qty = 0;
            string strSql = string.Format(@"Select dbo.fn_z_qty_to_sec_qty('{0}','{1}','{2}',{3}) As sec_qty", within_code, location_id, goods_id,qty);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            sec_qty = decimal.Parse(dt.Rows[0]["sec_qty"].ToString());           
            return sec_qty;
        }

        //檢查當前用戶部門的操作權限:0無權限,1有權限
        public static string CheckUserDeptRights(string user_id, string dept_id)
        {
            string result = "0";
            if (user_id.ToUpper() == "ADMIN")
            {
                result = "1";
                return result;
            }
            string strSql = string.Format(
               @"SELECT '1' as result FROM cd_storehouse_popedom WHERE within_code='0000' AND user_id='{0}' and location_id='{1}'", user_id, dept_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                result = "1";
            }
            return result;
        }

        //檢查移交單是否已簽收
        public static string CheckSignfor(string id)
        {
            string result = "0";
            string sql = string.Format(
                @"Select Count(1) as cnt From jo_materiel_con_details WITH(NOLOCK) Where within_code='0000' and id='{0}' and Isnull(signfor,'0')='1'", id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(sql);
            result = dt.Rows[0]["cnt"].ToString();
            return result;
        }

        //判断計劃是否批準狀態
        public static int GetPlanApproveState(string mo_id)
        {
            int result = 0;
            string sql = string.Format(
                @" Select Count(1) as cnt From jo_bill_mostly A WITH (NOLOCK) Where A.within_code='0000' And A.mo_id ='{0}' And A.state='0'", mo_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(sql);
            result = int.Parse(dt.Rows[0]["cnt"].ToString());
            return result;
        }

        //判断工单是否被hold住
        public static int GetPlanHoldState(string mo_id,string goods_id,string out_dept,string in_dept)
        {
            int result = 0;
            string sql = string.Format(
                @"Select Count(1) As cnt From jo_bill_mostly a WITH (NOLOCK),jo_bill_goods_details b WITH (NOLOCK)
                 Where a.within_code=b.within_code And a.id=b.id And a.ver=b.ver And a.state NOT IN ('2','V') 
                 And a.within_code='0000' And a.mo_id='{0}' And b.goods_id='{1}' And b.wp_id='{2}' And b.next_wp_id='{3}'
                 And Isnull(b.hold,'')<>''", mo_id,goods_id,out_dept, in_dept);
            DataTable dt = sh.ExecuteSqlReturnDataTable(sql);
            result = int.Parse(dt.Rows[0]["cnt"].ToString());
            return result;
        }

        //判断OC是否被hold住
        public static int GetOcHoldState(string mo_id)
        {
            int result = 0;
            string sql = string.Format(
                @"Select Count(1) As cnt From so_order_manage a WITH (NOLOCK),so_deliver_date b WITH (NOLOCK)
                Where a.within_code=b.within_code And a.id=b.id And a.ver=b.ver And b.within_code='0000' and b.mo_id='{0}' And b.hold_state='1' ", mo_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(sql);
            result = int.Parse(dt.Rows[0]["cnt"].ToString());
            return result;
        }

        //檢查單據號是否已存在
        public static bool CheckIdIsExists(string tableName,string id)
        {
            bool result = false;
            string strSql = string.Format(@"Select id FROM {0} with(nolock) Where within_code='0000' AND id='{1}'", tableName,id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
                result = true;
            else
                result = false;
            return result;
        }

        //獲取部門移交單批號
        public static string GetDeptLotNo(string out_dept,string in_dept)
        {
            string result = "";
            string strSql = string.Format(
            @" DECLARE @lot_no nvarchar(20) 
               EXEC usp_create_lot_no '{0}','{1}','{2}',@lot_no OUTPUT 
               SELECT @lot_no AS lot_no",
            "0000", out_dept, out_dept);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            result = dt.Rows[0]["lot_no"].ToString();
            return result;
        }

        public static List<ModelStLotNo> GetStDetailsLotNo(string location_id,string goods_id,string mo_id )
        {
            string strSql = string.Format(
            @"SELECT a.within_code,a.location_id,a.carton_code,a.mo_id,a.lot_no,a.qty,a.sec_qty,a.vendor_id, 
	             a.average_cost,b.id,b.name,b.sec_unit,b.unit,b.type,b.datum,b.attribute,b.unit_code,b.spec,
                 d.name as vendor_name
            FROM st_details_lot a 
	            Left Join it_goods b On a.within_code = b.within_code And a.goods_id = b.id
	            Left Join cd_productline c On a.within_code=c.within_code And a.location_id=c.id
	            Left Join it_vendor d on a.within_code=d.within_code and a.vendor_id=d.id
            WHERE a.within_code='0000' And a.location_id='{0}' And a.carton_code<>'ZZZ' And
	              a.goods_id='{1}' And ('{2}'='*' Or Isnull(a.mo_id,'')='{2}') And a.state<>'2' And
                  a.qty>0 And c.type<>'07'",location_id, goods_id, mo_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            List<ModelStLotNo> lst = new List<ModelStLotNo>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ModelStLotNo mdj = new ModelStLotNo();
                mdj.lot_no = dt.Rows[i]["lot_no"].ToString();
                mdj.qty = decimal.Parse(dt.Rows[i]["qty"].ToString());
                mdj.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                mdj.mo_id = dt.Rows[i]["mo_id"].ToString();
                mdj.vendor_name = dt.Rows[i]["vendor_name"].ToString();
                lst.Add(mdj);
            }
            return lst;
        }

        public static decimal ReturnFloat2(string pValue)
        {
            float fResult;
            fResult= (!string.IsNullOrEmpty(pValue))? float.Parse(pValue): 0.00f;
            fResult = (fResult > 0) ? (float)Math.Round(fResult, 2) : 0.00f;            
            return decimal.Parse(fResult.ToString());
        }

        public static decimal ReturnFloat4(string pValue)
        {
            float fResult;
            fResult = (!string.IsNullOrEmpty(pValue)) ? float.Parse(pValue) : 0.0000f;
            fResult = (fResult > 0) ? (float)Math.Round(fResult, 4) : 0.0000f;
            return decimal.Parse(fResult.ToString());
        }

    }
}
