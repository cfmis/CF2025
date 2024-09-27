using CF.Core.Config;
using CF.SQLServer.DAL;
using CF2025.Base.DAL;
using CF2025.Prod.Contract;
using CF2025.Store.Contract.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CF2025.Store.DAL
{
    public static class AdjustmentDAL
    {
        private static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        static PubFunDAL pubFun = new PubFunDAL();
        static string within_code = "0000";
        static string ls_servername = "hkserver.cferp.dbo";
        static string ldt_check_date = "";

        //public static List<details_mo_lot> GetLotNoList(string goods_id, string location_id, string mo_id)
        //{
        //    SqlParameter[] paras = new SqlParameter[]{
        //        new SqlParameter("@within_code", within_code),
        //        new SqlParameter("@goods_id", goods_id),
        //        new SqlParameter("@location_id",location_id),
        //        new SqlParameter("@mo_id", mo_id)
        //    };
        //    DataSet dts = sh.RunProcedure("p_details_mo_lot", paras, "mo_lot");
        //    List<details_mo_lot> lstDetail = new List<details_mo_lot>();
        //    for (int i = 0; i < dts.Tables[0].Rows.Count; i++)
        //    {
        //        details_mo_lot mdjDetail = new details_mo_lot();
        //        mdjDetail.lot_no = dts.Tables[0].Rows[i]["lot_no"].ToString();
        //        mdjDetail.qty = decimal.Parse(dts.Tables[0].Rows[i]["qty"].ToString());
        //        mdjDetail.sec_qty = decimal.Parse(dts.Tables[0].Rows[i]["sec_qty"].ToString());
        //        mdjDetail.mo_id = dts.Tables[0].Rows[i]["mo_id"].ToString();
        //        mdjDetail.goods_id = dts.Tables[0].Rows[i]["goods_id"].ToString();
        //        mdjDetail.vendor_name = dts.Tables[0].Rows[i]["vendor_name"].ToString();
        //        mdjDetail.is_qc = dts.Tables[0].Rows[i]["is_qc"].ToString();
        //        lstDetail.Add(mdjDetail);
        //    }
        //    return lstDetail;
        //}
        public static st_adjustment_mostly GetHeadByID(string id)
        {
            /*批準日期需按這種格式轉換,否則反準比較對不上而出錯
            *Convert(nvarchar(19),check_date,121) As check_date
            */
            st_adjustment_mostly mdjHead = new st_adjustment_mostly();
            string strSql = string.Format(
                @"Select id,Convert(nvarchar(10),date,121) As date,department_id,adjust_reason, handler,remark, create_by,
                create_date,update_by,update_date,check_by,Convert(nvarchar(19),check_date,121) As check_date,state,update_count
                FROM st_adjustment_mostly with(nolock) Where within_code='{0}' AND id='{1}'", within_code, id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                mdjHead.id = dr["id"].ToString();
                mdjHead.date = dr["date"].ToString();               
                mdjHead.department_id = dr["department_id"].ToString();
                mdjHead.adjust_reason = dr["adjust_reason"].ToString();               
                mdjHead.handler = dr["handler"].ToString();
                mdjHead.remark = dr["remark"].ToString();
                mdjHead.state = dr["state"].ToString();               
                mdjHead.create_by = dr["create_by"].ToString();
                mdjHead.update_by = dr["update_by"].ToString();
                mdjHead.check_by = dr["check_by"].ToString();
                mdjHead.update_count = dr["update_count"].ToString();
                mdjHead.create_date = dr["create_date"].ToString();
                mdjHead.update_date = dr["update_date"].ToString();
                mdjHead.check_date = dr["check_date"].ToString();
            }
            return mdjHead;
        }

        public static List<StDetailsGoods> GetStDetailsGoods(string within_code,string location_id,string mo_id)
        {
            string strSql = string.Format(
            @"SELECT B.within_code,A.mo_id,A.lot_no, A.goods_id,B.name As goods_name,B.sec_unit,A.sec_qty, B.unit,
            A.qty,A.carton_code,A.location_id, D.name As vendor_name, A.vendor_id, A.average_cost
            FROM st_details_lot A 
            Left Join it_goods B On A.within_code = B.within_code And A.goods_id = B.id
			Left Join cd_productline C On A.within_code = C.within_code And A.location_id = C.id
			Left Join it_vendor D on A.within_code = D.within_code and A.vendor_id =D.id
            WHERE A.state <> '2' And A.qty > 0 And A.carton_code <> 'ZZZ' And C.type <>'07' And
			B.within_code='{0}' And A.location_id='{1}' And ('{2}'='*' Or Isnull(A.mo_id,'')='{2}')",
            within_code, location_id,mo_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);            
            List<StDetailsGoods> lstDetail = new List<StDetailsGoods>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                StDetailsGoods mdjDetail = new StDetailsGoods();
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();
                mdjDetail.lot_no = dt.Rows[i]["lot_no"].ToString();
                mdjDetail.qty = decimal.Parse(dt.Rows[i]["qty"].ToString());
                mdjDetail.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                mdjDetail.mo_id = dt.Rows[i]["mo_id"].ToString();
                mdjDetail.vendor_name = dt.Rows[i]["vendor_name"].ToString();               
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }

        public static string Save(st_adjustment_mostly headData, List<st_a_subordination> lstDetailData1, List<st_a_subordination> lstDelData1, string user_id)
        {
            string str = "", lot_no = "";
            StringBuilder sbSql = new StringBuilder(" SET XACT_ABORT ON ");
            sbSql.Append(" BEGIN TRANSACTION ");
            string id = headData.id;
            string bill_id = "ST02";
            if (headData.head_status == "NEW")//全新的單據
            {
                bool id_exists = CommonDAL.CheckIdIsExists("st_adjustment_mostly", id);
                if (id_exists)
                {
                    //已存在此單據號,重新取最大單據號
                    //取最大單號
                    headData.id = CommonDAL.GetMaxID(bill_id, 4);
                }
                //更新系統表移交單最大單號
                id = headData.id;
                string bill_Code = id.Substring(1);   //biiCode value is  DWA23090452-->WA23090452
                string year_month = id.Substring(3,4);//2309
                string strSql_i = string.Format(
                @" INSERT INTO sys_bill_max_separate(within_code,bill_id,year_month,bill_code,bill_text1,bill_text2,bill_text3,bill_text4,bill_text5) 
                VALUES('0000','{0}','{1}','{2}','','','','','')", bill_id, year_month, bill_Code);
                string strSql_u = string.Format(
                @" UPDATE sys_bill_max_separate SET bill_code='{0}' 
                WHERE within_code='0000' And bill_id='{1}' And year_month='{2}' And bill_text1='' And bill_text2='' And bill_text3='' And bill_text4='' And bill_text5='' ",
                bill_Code, bill_id, year_month);
                string sql_sys_update2 = "";
                if (bill_Code.Substring(6, 4) != "0001")
                    sql_sys_update2 = strSql_u;
                else
                    sql_sys_update2 = strSql_i;
                str = sql_sys_update2;
                sbSql.Append(str);

                //生成調整單
                GenNumberDAL objNum = new GenNumberDAL();
                string update_count = "1", transfers_state = "0",node="1", servername = "hkserver.cferp.dbo",sequence_id = "";                
                decimal price = 0;
                int index;
                if (lstDetailData1.Count > 0)
                {
                    //主檔                   
                    str = string.Format(
                    @" Insert Into st_adjustment_mostly(within_code,id,department_id,date,mode,handler,remark,state,transfers_state,update_count,adjust_reason,servername,
                    create_by,create_date,update_by,update_date) VALUES
                    ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',getdate(),'{12}',getdate())",
                    within_code,headData.id,headData.department_id,headData.date,node,headData.handler,headData.remark,headData.state,transfers_state,update_count,headData.adjust_reason,servername,headData.create_by);
                    sbSql.Append(str);
                    //明細                   
                    index = 0;
                    foreach (var item in lstDetailData1)
                    {
                        index += 1;
                        sequence_id = objNum.GetSequenceID(index);// index.ToString().PadLeft(4, '0')+"h"; //序號                        
                        str = string.Format(
                        @" Insert Into st_a_subordination
                        (within_code,id,sequence_id,mo_id,goods_id,location,carton_code,qty,unit,ib_amount, price,transfers_state,sec_unit,sec_qty,ib_weight,lot_no,remark) Values
                        ('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8}',{9},{10},'{11}','{12}',{13},{14},'{15}','{16}')",
                        within_code, headData.id,sequence_id,item.mo_id,item.goods_id,item.location,item.location,item.qty,"PCS",item.ib_amount,price,transfers_state,"KG",
                        item.sec_qty,item.ib_weight,item.lot_no,item.remark);
                        sbSql.Append(str);
                    }
                }
            }
            else //已保存移交退回單的基礎上進行增刪改
            {
                //首先處理刪除(表格一) 
                //注意加選項with(ROWLOCK),只有用到主鍵時才會用到行級鎖
                if (lstDelData1 != null)
                {
                    foreach (var item in lstDelData1)
                    {
                        str = string.Format(
                            @" DELETE FROM st_a_subordination with(ROWLOCK) Where within_code='0000' AND id='{0}' AND sequence_id='{1}'",
                            item.id, item.sequence_id);
                        sbSql.Append(str);
                    }
                }
                //更新移交退回單表頭                
                str = string.Format(
                @" UPDATE st_adjustment_mostly with(ROWLOCK) 
                   SET department_id='{2}',date='{3}',handler='{4}',remark='{5}',adjust_reason='{6}',update_by='{7}', update_date=getdate(),
                       update_count = Convert(nvarchar(5),Convert(int,update_count)+1)
                   WHERE within_code ='{0}' AND id ='{1}'",
                within_code, headData.id, headData.department_id, headData.date, headData.handler, headData.remark,headData.adjust_reason, headData.update_by);
                sbSql.Append(str);
                //更新明細表一                 
                foreach (var item in lstDetailData1)
                {
                    //item.row_status非空,數據有新增或修改
                    if (!string.IsNullOrEmpty(item.row_status))
                    {
                        if (item.row_status == "EDIT")
                        {
                            lot_no = item.lot_no; //編輯時原記錄已有批號                            
                            //更新移交單明細                            
                            str = string.Format(
                            @" UPDATE st_a_subordination
                            SET mo_id='{3}', goods_id='{4}',location='{5}',carton_code='{6}',qty={7},unit='{8}',ib_amount={9},sec_qty={10},ib_weight={11},sec_unit='{12}',lot_no='{13}',remark='{14}'                            
                            WHERE within_code='{0}' And id='{1}' And sequence_id='{2}'", within_code, item.id, item.sequence_id,
                            item.mo_id, item.goods_id,item.location,item.location, item.qty, item.unit, item.ib_amount,item.sec_qty,item.ib_weight, item.sec_unit,item.lot_no, item.remark);
                            sbSql.Append(str);
                        }
                        else //INSERT ITEM//有項目新增
                        {
                            //插入新增的記錄                            
                            //新增的記錄插入已存在的移交單中
                            string transfers_state="0";
                            decimal price = 0;
                            str = string.Format(
                            @" Insert Into st_a_subordination
                            (within_code,id,sequence_id,mo_id,goods_id,location,carton_code,qty,unit,ib_amount,price,transfers_state,sec_unit,sec_qty,ib_weight,lot_no,remark) 
                            Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8}',{9},{10},'{11}','{12}',{13},{14},'{15}','{16}')",
                            within_code,headData.id,item.sequence_id,item.mo_id,item.goods_id,item.location,item.location,item.qty,"PCS",item.ib_amount,price,transfers_state,"KG",
                            item.sec_qty, item.ib_weight, item.lot_no, item.remark);
                            sbSql.Append(str);
                        } //end of item add                       

                    } //end is edit
                } //end for
            }
            sbSql.Append(@" COMMIT TRANSACTION ");
            string result = sh.ExecuteSqlUpdate(sbSql.ToString());
            sbSql.Clear();

            return result;
        }

        public static List<st_a_subordination> GetDetailsByID(string id)
        {
            string strSql = string.Format(
            @"SELECT A.id,A.sequence_id,A.mo_id, A.goods_id, B.name AS goods_name,C.name AS color,A.location,A.carton_code,A.unit,
            A.qty, A.ib_amount,A.price,A.sec_unit,A.sec_qty, A.ib_weight, A.lot_no, A.remark
            FROM st_a_subordination A with(nolock)
                INNER JOIN it_goods B ON A.within_code = B.within_code And A.goods_id = B.id
                LEFT JOIN cd_color C ON B.within_code = C.within_code AND B.color = C.id
            WHERE A.within_code='{0}' AND A.id='{1}' Order By A.id,A.sequence_id", within_code, id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            List<st_a_subordination> lstDetail = new List<st_a_subordination>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                st_a_subordination mdjDetail = new st_a_subordination();
                mdjDetail.id = dt.Rows[i]["id"].ToString();
                mdjDetail.sequence_id = dt.Rows[i]["sequence_id"].ToString();
                mdjDetail.mo_id = dt.Rows[i]["mo_id"].ToString();
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();
                mdjDetail.color = dt.Rows[i]["color"].ToString();
                mdjDetail.location = dt.Rows[i]["location"].ToString();
                mdjDetail.carton_code = dt.Rows[i]["carton_code"].ToString();
                mdjDetail.qty = decimal.Parse(dt.Rows[i]["qty"].ToString());
                mdjDetail.unit = dt.Rows[i]["unit"].ToString();
                mdjDetail.ib_amount = decimal.Parse(dt.Rows[i]["ib_amount"].ToString());
                mdjDetail.price = decimal.Parse(dt.Rows[i]["price"].ToString());
                mdjDetail.sec_unit = dt.Rows[i]["sec_unit"].ToString();
                mdjDetail.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());//
                mdjDetail.ib_weight = decimal.Parse(dt.Rows[i]["ib_weight"].ToString());
                mdjDetail.lot_no = dt.Rows[i]["lot_no"].ToString();                
                mdjDetail.remark = dt.Rows[i]["remark"].ToString();
                mdjDetail.row_status = "";
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }


        public static string Approve(st_adjustment_mostly head, string user_id, string approve_type)
        {
            string result = "", active_name = "";
            //批準移交單(其中包含有調用生成移交單交易、更新移交單相關庫存的方法)
            if (approve_type == "1")
            {
                active_name = "pfc_ok";
                //設置全局的批準日期
                ldt_check_date = CommonDAL.GetDbDateTime("L");//批準日期(長日期時間)
            }
            else
            {
                active_name = "pfc_unok";
                ldt_check_date = head.check_date; //這個日期很重要
            }            
            result = wf_check_inv(head.id, active_name);//庫存檢查
            if (result.Substring(0, 2) == "01")
            {
                return "01";//庫存檢查通不過
            }
            result = ApproveAdjustment(head, ldt_check_date, active_name, user_id);
            return result;
        }

        /// <summary>
        /// 移交單批準
        /// 單獨移交單畫面的批準只有一張移交單,有另于組裝轉換
        /// 批準移交單,更改移交單的批準狀態,交易的生成,庫存的更改
        /// </summary>
        /// <param name="headData"></param>       
        /// <param name="check_date"></param>
        /// <param name="user_id"></param>
        /// <returns>返回字串00,表示成功</returns>     
        public static string ApproveAdjustment(st_adjustment_mostly headData, string check_date, string active_name, string user_id)
        {
            //更新移交單相關庫存
            string window_id = "";
            string result = SetAdjustmentStBusiness(headData.id, check_date, active_name, "JO", user_id, window_id); //active_name:"pfc_ok","pfc_unok"

            if (result.Substring(0, 2) == "00") // SetReturnRechangeStBusiness()執行成功
            {
                bool is_pfc_ok = (active_name == "pfc_ok") ? true : false;
                string checkDate = is_pfc_ok ? ldt_check_date : "";
                string checkBy = is_pfc_ok ? user_id : "";
                string state = is_pfc_ok ? "1" : "0";                
                string strSql = string.Format(
                        @" Update st_adjustment_mostly with(Rowlock) 
                           SET check_date=(Case When '{2}'<>'' Then '{2}' Else null End), check_by='{3}',
                               update_date=(Case When '{2}'<>'' Then '{2}' Else getdate() End),update_by='{4}',state='{5}' 
                           WHERE within_code='{0}' And id='{1}'", within_code, headData.id, checkDate, checkBy, user_id, state);
                result = sh.ExecuteSqlUpdate(strSql);
                result = (result == "") ? "00" : "-1" + result;
            }
            return result;
        }

        //注銷
        public static string DeleteHead(st_adjustment_mostly head, string user_id)
        {
            string result = "";
            string strSql = string.Format(
                @"UPDATE jo_assembly_mostly with(Rowlock) 
                SET state='2',update_by='{0}',update_date=getdate() 
                WHERE within_code='0000' AND id='{1}'", user_id, head.id);
            result = sh.ExecuteSqlUpdate(strSql);
            return result;//成功返回空格
        }


        /// <summary>
        /// 生成調整單交易數據(返回值為字符串,字串前兩位:-1代表失敗;00:成功)
        /// </summary>
        /// <param name="as_id">移交單據編號</param>
        /// <param name="as_check_date">當前批準日期時間</param>
        /// <param name="as_active_name">pfc_ok/pfc_unok</param>
        /// <param name="as_bill_type">"JO:移交單"</param>
        /// <param name="as_user_id"></param>
        /// <returns></returns>
        public static string SetAdjustmentStBusiness(string as_id, string as_check_date, string as_active_name, string as_bill_type, string as_user_id, string as_window_id)
        {
            string result = "00";//strStatus字符串前兩位:-1代表失敗;00:成功
            string return_value = "", str = "", strSql = "";

            string ls_id, ls_goods_id, ls_error = "", ls_check_date = "";
            string ls_mode,ls_location, ls_carton_code, ls_lot_no, ls_unit, ls_sec_unit,ls_sequence_id;
            decimal ldc_qty, ldc_sec_qty, ldc_stock_qty, ldc_stock_sec_qty, ldc_adjust_qty, ldc_adjust_sec_qty;
            int ll_count;
            ls_id = as_id;
            StringBuilder sbSql = new StringBuilder(" SET XACT_ABORT ON ");
            sbSql.Append(" BEGIN TRANSACTION ");
            DataTable dt = new DataTable();
            if (as_active_name == "pfc_ok" || as_active_name == "pfc_unok")
            {
                ls_check_date = as_check_date;
                //反批准时清空日期变量
                if (as_active_name == "pfc_unok")
                {
                    ls_check_date = "";  //**** 更新要注意NULL轉換  ****
                }
                //无条件控制负数的出现
                result = pubFun.wf_check_inventory_qty(as_id, "w_st_adjustment");//调整
                if(result.Substring(0,2)=="-1")
                {
                    return result;
                }
                DataTable dtCount = new DataTable();
                strSql = string.Format(
                @"Select A.mode,B.sequence_id,B.goods_id,B.location,B.carton_code,B.lot_no,B.unit,B.sec_unit,B.qty,B.sec_qty
                From st_adjustment_mostly A with(nolock), st_a_subordination B with(nolock)
                Where A.within_code=B.within_code And A.id=B.id And A.within_code='{0}' And A.id='{1}'",within_code,ls_id);                
                dt = sh.ExecuteSqlReturnDataTable(strSql);
                for(int i=0;i<dt.Rows.Count;i++)
                {
                    ls_mode = dt.Rows[i]["mode"].ToString();
                    ls_sequence_id = dt.Rows[i]["sequence_id"].ToString();
                    ls_goods_id = dt.Rows[i]["goods_id"].ToString(); 
                    ls_location = dt.Rows[i]["location"].ToString();
                    ls_carton_code = dt.Rows[i]["carton_code"].ToString(); 
                    ls_lot_no = dt.Rows[i]["lot_no"].ToString(); 
                    ls_unit = dt.Rows[i]["unit"].ToString();
                    ls_sec_unit= dt.Rows[i]["sec_unit"].ToString();
                    ldc_qty = decimal.Parse(dt.Rows[i]["qty"].ToString());
                    ldc_sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                    ll_count = 0;
                    strSql = string.Format(
                    @"Select Count(1) as cnt From st_business_record WITH(NOLOCK) 
                    Where within_code='{0}' And id ='{1}' And sequence_id='{2}' And action_id='03'", 
                    within_code, ls_id, ls_sequence_id);
                    dtCount = sh.ExecuteSqlReturnDataTable(strSql);
                    ll_count = int.Parse(dtCount.Rows[0]["cnt"].ToString()); 
                    
                    //***批準***
                    if(as_active_name.ToLower()=="pfc_ok" && ll_count == 0)
                    {
                        if (ls_lot_no.Length > 0)
                        {                            
                            strSql = string.Format(
                            @"Select qty, sec_qty From st_details_lot with(nolock)
                            Where within_code='{0}' And location_id='{1}' And carton_code='{2}' And goods_id='{3}' And lot_no='{4}'",
                            within_code, ls_location, ls_carton_code, ls_goods_id, ls_lot_no);                           
                        }
                        else
                        {
                            strSql = string.Format(
                            @"Select Sum(Isnull(qty, 0)) as qty, Sum(Isnull(sec_qty, 0)) as sec_qty                           
                            From st_details_lot
                            Where within_code='{0}' And location_id='{1}' And carton_code='{2}' And goods_id='{3}' And lot_no='{4}'", 
                            within_code,ls_location,ls_carton_code, ls_goods_id,ls_lot_no);                           
                        }
                        dtCount = sh.ExecuteSqlReturnDataTable(strSql);
                        if (dtCount.Rows.Count > 0)
                        {
                            ldc_stock_qty = decimal.Parse(dtCount.Rows[0]["qty"].ToString());
                            ldc_stock_sec_qty = decimal.Parse(dtCount.Rows[0]["sec_qty"].ToString());
                        }
                        else
                        {
                            ldc_stock_qty = 0;
                            ldc_stock_sec_qty = 0;
                        }
                        //--start 转化为当前单位
                        strSql = string.Format(
                        @"Select Top 1 DBO.FN_CHANGE_UNITBYCV('{0}','{1}','{2}','{3}','','','*') as stock_qty,
									   DBO.fn_change_units_sec('{0}','{1}', '','{4}','{5}') as stock_sec_qty
                          From sysobjects", within_code, ls_goods_id, ls_unit, ldc_stock_qty, ls_sec_unit, ldc_stock_sec_qty);
                        dtCount = sh.ExecuteSqlReturnDataTable(strSql);
                        if (!string.IsNullOrEmpty(dtCount.Rows[0]["stock_qty"].ToString()))                        
                            ldc_stock_qty = decimal.Parse(dtCount.Rows[0]["stock_qty"].ToString());                        
                        else
                            ldc_stock_qty = 0;
                        if (!string.IsNullOrEmpty(dtCount.Rows[0]["stock_sec_qty"].ToString()))
                            ldc_stock_sec_qty = decimal.Parse(dtCount.Rows[0]["stock_sec_qty"].ToString());
                        else
                            ldc_stock_sec_qty = 0;
                        //--end 转化为当前单位
                        if (ls_mode == "1")//--在原数量上进行调整
                        {
                            ldc_adjust_qty = ldc_qty;
                            ldc_adjust_sec_qty = ldc_sec_qty;
                        }
                        else
                        {
                            ldc_adjust_qty = ldc_stock_qty - ldc_qty;
                            ldc_adjust_sec_qty = ldc_stock_sec_qty - ldc_sec_qty;
                        }
                        //--
                        str = string.Format(
                        @" Insert Into st_business_record
                           (within_code,id,sequence_id,goods_id,goods_name,unit,action_time,action_id,ii_qty,sec_qty,sec_unit,
                           ii_location_id, ii_code, check_date, 
                           ib_qty, qty, lot_no, mo_id, dept_id, servername)
                        Select B.within_code,B.id,B.sequence_id,B.goods_id,B.goods_name,B.unit,A.date,'03', {4} , {5} ,B.sec_unit,
                               B.location,B.carton_code,  '{6}',
                               dbo.FN_CHANGE_UNITBYCV(B.within_code, B.goods_id, B.unit, 1,'','','*') as ib_qty,
                               Round(dbo.FN_CHANGE_UNITBYCV(B.within_code, B.goods_id, B.unit, {7} ,'','','/'), 4) as qty,
                               B.lot_no,B.mo_id,A.department_id,A.servername
                        FROM st_adjustment_mostly A With(nolock), st_a_subordination B With(nolock)
                        WHERE A.within_code=B.within_code And A.id=B.id And B.within_code='{0}' And B.id='{1}' And B.sequence_id='{2}' And B.goods_id='{3}'",
                        within_code,ls_id,ls_sequence_id,ls_goods_id, ldc_adjust_qty, ldc_adjust_sec_qty, ls_check_date, ldc_adjust_qty);
                        sbSql.Append(str);
                    } //end of if pfc_ok                   
                    //***批準結束***

                    //***反批準***
                    if (as_active_name.ToLower() == "pfc_unok" && ll_count > 0)
                    {
                        //先更新库存,再删除交易数据
                        string rtns = pubFun.of_update_st_details("D", "03", ls_id, ls_sequence_id, ls_check_date, "");
                        if (rtns.Substring(0, 2) == "-1")
                        {                            
                            result = rtns + "数据保存失败!" + "\r\n <" + as_active_name + ">(st_details)" + "\r\n";
                            return result;
                        }
                        //--
                        str = string.Format(
                        @" Delete FROM st_business_record WITH(ROWLOCK)
                        WHERE within_code='{0}' AND id='{1}' And sequence_id='{2}' And action_id ='03'", within_code, ls_id, ls_sequence_id);
                        sbSql.Append(str);                        
                    }
                    //***反批準結束***

                }  //end of for

                sbSql.Append(@" COMMIT TRANSACTION ");
                result = sh.ExecuteSqlUpdate(sbSql.ToString());
                sbSql.Clear();
                if (result != "")
                {
                    if (as_active_name.ToLower() == "pfc_ok")
                        result = "-1" + "批準失敗!<" + result + ">";
                    else
                        result = "-1" + "反批準失敗!<" + result + ">";

                    return result;
                }
                else
                    result = "00";

                //--统一更新库存
                if (as_active_name.ToLower() == "pfc_ok")
                {
                    return_value = pubFun.of_update_st_details("I", "03", ls_id, "*", ls_check_date, ls_error);
                    if (return_value.Substring(0, 2) == "-1")
                    {
                        result = return_value + "\r\n" + "庫存數據保存失败![统一更新库存]+" + "\r\n" + "<" + as_active_name + ">(st_details)" + "\r\n";
                        return result;
                    }
                }
                
            } //end fo if (as_active_name == "pfc_ok" || as_active_name == "pfc_unok")            
            return result;
        }//--end SetAdjustmentStBusiness()


        /*********************************************************************************
        功能：检查库存
        日期：2023-09-20
        *********************************************************************************/
        public static string wf_check_inv(string id,string is_active_name)
        {
            string ls_goods_id = string.Empty, ls_mo_id = string.Empty, ls_lot_no = string.Empty, ls_id = string.Empty;
            string ls_msg = "00", ls_qty_format = string.Empty, ls_location_id = string.Empty;
            decimal ldec_qty=0, ldec_stock_qty=0, ldec_sec_qty=0, ldec_stock_sec_qty=0;           
            //int ll_rc=0;
            //ls_qty_format = '##0.00'
            ls_id = id;
            string strSql = string.Empty;
            DataTable dt = new DataTable();
            //批準時檢查
            if (is_active_name.ToLower() == "pfc_ok")
            {
                //检查批号库存
                strSql = string.Format(
                @"SELECT Top 1 S.location_id, S.goods_id,IsNull(S.mo_id,'') AS mo_id, IsNull(S.lot_no,'') AS lot_no,IsNull(S.qty, 0) AS qty,
                    IsNull(S.sec_qty,0) AS sec_qty,IsNull(b.qty, 0) AS stock_qty, IsNull(b.sec_qty, 0) AS stock_sec_qty                 
                FROM
                    (Select b.within_code, b.location As location_id, b.carton_code, b.goods_id,Isnull(b.mo_id, '') As mo_id, 
                    IsNull(b.lot_no, '') As lot_no, SUM(b.qty) as qty, SUM(b.sec_qty) As sec_qty
                    From st_adjustment_mostly a, st_a_subordination b
                    Where a.within_code = b.within_code and a.id = b.id and a.within_code ='0000' and a.id ='{0}'
                    Group by b.within_code,b.location,b.carton_code,b.goods_id,Isnull(b.mo_id,''),IsNull(b.lot_no,'')
                    ) S
                    Left Join st_details_lot b On S.within_code = b.within_code and
                                                  S.location_id = b.location_id and S.carton_code = b.carton_code and
                                                  S.goods_id = b.goods_id and S.mo_id = b.mo_id and S.lot_no = b.lot_no
                WHERE( IsNull(S.qty,0) + IsNull(b.qty,0)) < 0 Or(IsNull(S.sec_qty,0) + IsNull(b.sec_qty, 0) ) < 0", ls_id);                
                dt = sh.ExecuteSqlReturnDataTable(strSql);
                if (dt.Rows.Count > 0)
                {
                    ls_location_id = dt.Rows[0]["location_id"].ToString();
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    ls_mo_id = dt.Rows[0]["mo_id"].ToString();
                    ls_lot_no = dt.Rows[0]["lot_no"].ToString();
                    ldec_qty = decimal.Parse(dt.Rows[0]["qty"].ToString());
                    ldec_sec_qty = decimal.Parse(dt.Rows[0]["sec_qty"].ToString());
                    ldec_stock_qty = decimal.Parse(dt.Rows[0]["stock_qty"].ToString());
                    ldec_stock_sec_qty = decimal.Parse(dt.Rows[0]["stock_sec_qty"].ToString());
                }
                if (ls_goods_id.Length > 0)
                {
                    ls_msg += "\r\n[倉位:" + ls_location_id + " 貨品:" + ls_goods_id +" 頁數:"+ ls_mo_id + " 批號:" + ls_lot_no +"]";                    
                    if (ldec_stock_sec_qty + ldec_sec_qty < 0)
                    {
                        ls_msg += "\r\n" + "當前倉位庫存" + ":" + ldec_stock_sec_qty.ToString();
                        ls_msg += "\r\n" + "相差庫存" + ":" + (ldec_stock_sec_qty + ldec_sec_qty).ToString();
                    }
                    else
                    {
                        ls_msg += "\r\n" + "當前倉位庫存" + ":" + ldec_stock_qty.ToString();//T_current_carton_qty
                        ls_msg += "\r\n" + "相差庫存" + ":" + (ldec_stock_qty + ldec_qty).ToString();//t_lack_qty
                    }
                    //invo_function.of_Message(gs_language, '', '100035', '库存不足，不能进行此操作', ls_msg)
                    //ll_rc = dw_detail.find("location_id='" + ls_location_id + "' and goods_id='" + ls_goods_id + "' and mo_id='" + ls_mo_id + "' and lot_no='" + ls_lot_No + "'", 1, dw_detail.Rowcount())                    
                    //if (ll_rc > 0)
                    //{ 
                    //滾動到有問題的行
                    //dw_detail.Selectrow(0, false)
                    //dw_detail.Selectrow(ll_rc, true)
                    //dw_detail.Scrolltorow(ll_rc)
                    //}
                    //return "-1";
                    ls_msg = "-1" + "库存不足，不能进行此操作!" + "\r\n" + ls_msg;
                    return ls_msg;
                }

                //检查库存总表               
                strSql = string.Format(
                @"SELECT Top 1 S.location_id,S.goods_id,Isnull(S.qty, 0) As qty,IsNull(S.sec_qty, 0) As sec_qty,
                    IsNull(b.qty, 0) As stock_qty,IsNull(b.sec_qty, 0) As stock_sec_qty               
                FROM
                    (Select b.within_code, b.location as location_id, b.carton_code, b.goods_id,
                     SUM(b.qty) as qty, SUM(b.sec_qty) as sec_qty
                     From st_adjustment_mostly a, st_a_subordination b
                     Where a.within_code=b.within_code and a.id=b.id and a.within_code='{0}' and a.id='{1}'
                     Group by b.within_code, b.location, b.carton_code, b.goods_id
                    ) S
                    Left Join st_details b On S.within_code=b.within_code and S.location_id=b.location_id and 
                                     S.carton_code=b.carton_code and S.goods_id=b.goods_id
                WHERE (IsNull(S.qty, 0) + IsNull(b.qty, 0)) < 0 Or(IsNull(S.sec_qty, 0) + IsNull(b.sec_qty, 0)) < 0 ", within_code,ls_id);
                dt = sh.ExecuteSqlReturnDataTable(strSql);
                if (dt.Rows.Count > 0)
                {
                    ls_location_id = dt.Rows[0]["location_id"].ToString();
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();                   
                    ldec_qty = decimal.Parse(dt.Rows[0]["qty"].ToString());
                    ldec_sec_qty = decimal.Parse(dt.Rows[0]["sec_qty"].ToString());
                    ldec_stock_qty = decimal.Parse(dt.Rows[0]["stock_qty"].ToString());
                    ldec_stock_sec_qty = decimal.Parse(dt.Rows[0]["stock_sec_qty"].ToString());
                }
                if (ls_goods_id.Length > 0)
                {
                    ls_msg = "\r\n[倉位:" + ls_location_id + " 貨品:" + ls_goods_id + "]";
                    if (ldec_stock_sec_qty + ldec_sec_qty < 0)
                    {
                        ls_msg += "\r\n" + "當前倉位庫存" + ":" + ldec_stock_sec_qty.ToString();//t_current_carton_qty
                        ls_msg += "\r\n" + "相差庫存" + ":" + (ldec_stock_sec_qty + ldec_sec_qty).ToString();//t_lack_qty
                    }
                    else
                    {
                        ls_msg += "\r\n" + "當前倉位庫存" + ":" + ldec_stock_qty.ToString();
                        ls_msg += "\r\n" + "相差庫存" + ":" + (ldec_stock_qty + ldec_qty).ToString();
                    }
                    //invo_function.of_Message(gs_language, '', '100035', '库存不足，不能进行此操作', ls_msg)
                    //ll_rc = dw_detail.find("location_id='" + ls_location_id + "' and goods_id='" + ls_goods_id + "'", 1, dw_detail.Rowcount())
                    //If ll_rc > 0 Then
                    //    dw_detail.Selectrow(0, false)
                    //    dw_detail.Selectrow(ll_rc, true)
                    //    dw_detail.Scrolltorow(ll_rc)
                    //End if
                    //Return - 1
                    ls_msg = "-1" + "库存不足，不能进行此操作!" + ls_msg;
                    return ls_msg;
                }

            }// end of pfc_ok
            
            //反批準時檢查
            if (is_active_name.ToLower()== "pfc_unok")
            {
                //检查批号库存                
                strSql = string.Format(
                 @"Select Top 1 S.location_id,S.goods_id,IsNull(S.mo_id,'') As mo_id,IsNull(S.lot_no,'') As lot_no,IsNull(S.qty,0) As qty,
                                IsNull(S.sec_qty,0) As sec_qty,IsNull(b.qty,0) As stock_qty,IsNull(b.sec_qty,0)	As stock_sec_qty               
	               From (Select b.within_code,b.location As location_id,b.carton_code,b.goods_id,Isnull(b.mo_id,'') As mo_id,IsNull(b.lot_no,'') As lot_no,
			               SUM(-b.qty) As qty,SUM(-b.sec_qty) As sec_qty
			             From st_adjustment_mostly a,st_a_subordination b
			             Where a.within_code =b.within_code And a.id =b.id And a.within_code='{0}' And a.id ='{1}'
			             Group by b.within_code,b.location,b.carton_code,b.goods_id,Isnull(b.mo_id,''),IsNull(b.lot_no,'') 
                        ) S 
	                Left Join st_details_lot b On S.within_code=b.within_code And S.location_id=b.location_id And S.carton_code=b.carton_code And
												S.goods_id = b.goods_id And S.mo_id =b.mo_id And S.lot_no = b.lot_no 
	                Where ( IsNull(S.qty,0) + IsNull(b.qty,0)) <0 OR ( IsNull(S.sec_qty,0) + IsNull(b.sec_qty,0)) <0 ", within_code, ls_id);
                dt = sh.ExecuteSqlReturnDataTable(strSql);
                if (dt.Rows.Count > 0)
                {
                    ls_location_id = dt.Rows[0]["location_id"].ToString();
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    ls_mo_id = dt.Rows[0]["mo_id"].ToString();
                    ls_lot_no = dt.Rows[0]["lot_no"].ToString();
                    ldec_qty = decimal.Parse(dt.Rows[0]["qty"].ToString());
                    ldec_sec_qty = decimal.Parse(dt.Rows[0]["sec_qty"].ToString());
                    ldec_stock_qty = decimal.Parse(dt.Rows[0]["stock_qty"].ToString());
                    ldec_stock_sec_qty = decimal.Parse(dt.Rows[0]["stock_sec_qty"].ToString());
                }
                if (ls_goods_id.Length > 0)
                {                   
                    ls_msg += "\r\n[倉位:" + ls_location_id + " 貨品:" + ls_goods_id + " 頁數:" + ls_mo_id + " 批號:" + ls_lot_no + "]";//t_lot_no                  	
                    if (ldec_stock_sec_qty + ldec_sec_qty < 0)
                    {
                        ls_msg += "\r\n" + "當前倉位庫存" + ":" + ldec_stock_sec_qty.ToString();//t_current_carton_qty
                        ls_msg += "\r\n" + "相差庫存" + ":" + (ldec_stock_sec_qty + ldec_sec_qty).ToString();//t_lack_qty
                    }
                    else
                    {
                        ls_msg += "\r\n" + "當前倉位庫存" + ":" + ldec_stock_qty.ToString();
                        ls_msg += "\r\n" + "相差庫存" + ":" + (ldec_stock_qty + ldec_qty).ToString();
                    }
                    //invo_function.of_Message(gs_language,'','100035','库存不足，不能进行此操作',ls_msg)
                    //ll_rc = dw_detail.find("location_id='" +ls_location_id+"' and goods_id='" +ls_goods_id +"' and mo_id='" +ls_mo_id+"' and lot_no='" +ls_lot_No+"'",1,dw_detail.Rowcount())
                    //If ll_rc >0 Then
                    //    dw_detail.Selectrow(0,false)
                    //    dw_detail.Selectrow(ll_rc,true)
                    //    dw_detail.Scrolltorow(ll_rc)
                    //End if
                    // Return -1
                    ls_msg = "-1" + "库存不足，不能进行此操作!" + ls_msg;
                    return ls_msg;
                }

                //检查库存总表                
                strSql = string.Format(
                @"SELECT Top 1 S.location_id,S.goods_id,Isnull(S.qty,0) As qty,IsNull(S.sec_qty,0) As sec_qty,IsNull(b.qty,0) As stock_qty,IsNull(b.sec_qty,0) As stock_sec_qty	  
	            FROM  (Select b.within_code ,b.location as location_id,b.carton_code,b.goods_id,
			            SUM(-b.qty) As qty,SUM(-b.sec_qty) As sec_qty
			            From st_adjustment_mostly a,st_a_subordination b
			            Where a.within_code =b.within_code And a.id =b.id And a.within_code='{0}' And a.id ='{1}'
			            Group by b.within_code ,b.location,b.carton_code,b.goods_id 
                        ) S 
	            LEFT Join st_details b On S.within_code=b.within_code And S.location_id=b.location_id And S.carton_code=b.carton_code And S.goods_id=b.goods_id 
	            WHERE (IsNull(S.qty,0) + IsNull(b.qty,0)) < 0 OR (IsNull(S.sec_qty,0) + IsNull(b.sec_qty,0)) < 0 ", within_code, ls_id);
                dt = sh.ExecuteSqlReturnDataTable(strSql);
                if (dt.Rows.Count > 0)
                {
                    ls_location_id = dt.Rows[0]["location_id"].ToString();
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();                    
                    ldec_qty = decimal.Parse(dt.Rows[0]["qty"].ToString());
                    ldec_sec_qty = decimal.Parse(dt.Rows[0]["sec_qty"].ToString());
                    ldec_stock_qty = decimal.Parse(dt.Rows[0]["stock_qty"].ToString());
                    ldec_stock_sec_qty = decimal.Parse(dt.Rows[0]["stock_sec_qty"].ToString());
                }
                if (ls_goods_id.Length > 0)
                {
                    ls_msg = "\r\n[倉位:" + ls_location_id + " 貨品:" + ls_goods_id + "]";
                    if (ldec_stock_sec_qty + ldec_sec_qty < 0)
                    {
                        ls_msg += "\r\n" + "當前倉位庫存" + ":" + ldec_stock_sec_qty.ToString();
                        ls_msg += "\r\n" + "相差庫存" + ":" + (ldec_stock_sec_qty + ldec_sec_qty).ToString();
                    }
                    else
                    {
                        ls_msg += "\r\n" + "當前倉位庫存" + ":" + ldec_stock_qty.ToString();
                        ls_msg += "\r\n" + "相差庫存" + ":" + (ldec_stock_qty + ldec_qty).ToString();
                    }
                    //invo_function.of_Message(gs_language,'','100035','库存不足，不能进行此操作',ls_msg)
                    //ll_rc = dw_detail.find("location_id='" +ls_location_id+"' and goods_id='" +ls_goods_id +"'",1,dw_detail.Rowcount())
                    //If ll_rc >0 Then
                    //    dw_detail.Selectrow(0,false)
                    //    dw_detail.Selectrow(ll_rc,true)
                    //    dw_detail.Scrolltorow(ll_rc)
                    //End if
                    //Return -1
                    ls_msg = "-1" + "库存不足，不能进行此操作!" + ls_msg;
                }

            }//end of pfc_unok

            return ls_msg; //00表示成功,代表檢查通過

        }//end of wf_check_inv

    }
}
