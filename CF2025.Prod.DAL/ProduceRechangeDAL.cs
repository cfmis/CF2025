using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CF2025.Base.DAL;
using CF.Core.Config;
using CF.SQLServer.DAL;
using CF2025.Prod.Contract;


namespace CF2025.Prod.DAL
{
    public static class ProduceRechangeDAL
    {
        static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        static PubFunDAL pubFun = new PubFunDAL();
        static string within_code = "0000";
        static string ls_servername = "hkserver.cferp.dbo";
        static string ldt_check_date = "";

        public static jo_assembly_mostly GetHeadByID(string id)
        {
            jo_assembly_mostly mdjHead = new jo_assembly_mostly();
            string strSql = string.Format(
                @"Select id,Convert(nvarchar(10),con_date,121) As con_date,bill_origin,out_dept,in_dept,handler,remark,stock_type,
                create_by,create_date,update_by,update_date,check_by,Convert(nvarchar(19),check_date,121) As check_date,update_count,state
                FROM jo_materiel_con_mostly with(nolock) Where within_code='{0}' AND id='{1}'", within_code, id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                mdjHead.id = dr["id"].ToString();
                mdjHead.con_date = dr["con_date"].ToString();
                mdjHead.bill_origin = dr["bill_origin"].ToString();
                mdjHead.out_dept = dr["out_dept"].ToString();
                mdjHead.in_dept = dr["in_dept"].ToString();
                //mdjHead.handover_id = dr["handover_id"].ToString();
                mdjHead.handler = dr["handler"].ToString();
                mdjHead.remark = dr["remark"].ToString();
                mdjHead.state = dr["state"].ToString();
                mdjHead.stock_type = dr["stock_type"].ToString();
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

        public static List<jo_assembly_details> GetDetailsByID(string id)
        {
            string strSql = string.Format(
            @"SELECT A.id,A.sequence_id,A.mo_id, A.goods_id, B.name AS goods_name, A.con_qty, A.unit_code,
                A.sec_qty, A.sec_unit, A.lot_no, A.package_num, A.app_supply_side, A.remark, A.return_qty_nonce,
                A.location, A.carton_code, A.jo_id,A.jo_sequence_id,A.qc_result,A.aim_jo_id, A.aim_jo_sequence,
                A.aim_jo_id,A.aim_jo_sequence, C.name AS color_name,C.four_color,'' as free
            FROM jo_materiel_con_details A with(nolock)
                INNER JOIN it_goods B ON A.within_code = B.within_code And A.goods_id = B.id
                LEFT JOIN cd_color C ON B.within_code = C.within_code AND B.color = C.id
            WHERE A.within_code='{0}' AND A.id='{1}' Order By A.id,A.sequence_id", within_code, id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            List<jo_assembly_details> lstDetail = new List<jo_assembly_details>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jo_assembly_details mdjDetail = new jo_assembly_details();
                mdjDetail.id = dt.Rows[i]["id"].ToString();
                mdjDetail.sequence_id = dt.Rows[i]["sequence_id"].ToString();
                mdjDetail.mo_id = dt.Rows[i]["mo_id"].ToString();
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();
                mdjDetail.con_qty = decimal.Parse(dt.Rows[i]["con_qty"].ToString());
                mdjDetail.unit_code = dt.Rows[i]["unit_code"].ToString();
                mdjDetail.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                mdjDetail.sec_unit = dt.Rows[i]["sec_unit"].ToString();
                mdjDetail.lot_no = dt.Rows[i]["lot_no"].ToString();
                mdjDetail.package_num = decimal.Parse(string.IsNullOrEmpty(dt.Rows[i]["package_num"].ToString()) ? "0.00" : dt.Rows[i]["package_num"].ToString());
                mdjDetail.app_supply_side = dt.Rows[i]["app_supply_side"].ToString();
                mdjDetail.remark = dt.Rows[i]["remark"].ToString();
                mdjDetail.return_qty_nonce = dt.Rows[i]["return_qty_nonce"].ToString();               
                mdjDetail.location = dt.Rows[i]["location"].ToString();
                mdjDetail.carton_code = dt.Rows[i]["carton_code"].ToString();                
                mdjDetail.jo_id = dt.Rows[i]["jo_id"].ToString();
                mdjDetail.jo_sequence_id = dt.Rows[i]["jo_sequence_id"].ToString();
                mdjDetail.color_name = dt.Rows[i]["color_name"].ToString();
                mdjDetail.aim_jo_id = "";
                mdjDetail.aim_jo_sequence = "";
                mdjDetail.four_color = "";
                mdjDetail.row_status = "";
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }

        //移交單總數量
        public static List<assembly_qty> GetTotalRechange(string id,string mo_id,string goods_id,string out_dept,string in_dept)
        {           
            string sql = string.Format(
                @"Select Sum(D.con_qty) AS con_qty,Sum(D.sec_qty) AS sec_qty                
                From jo_materiel_con_mostly M With(nolock),jo_materiel_con_details D With(nolock)
                Where M.within_code=D.within_code And M.id=D.id And M.bill_type_no='T' And M.stock_type='0' And M.state<>'2' And D.within_code='{0}'
                And D.id<>'{1}' And D.mo_id='{2}' And D.goods_id='{3}' And M.out_dept='{4}' And M.in_dept='{5}'", within_code,id,mo_id,goods_id,out_dept,in_dept);
            DataTable dt = sh.ExecuteSqlReturnDataTable(sql);
            List<assembly_qty> lst = new List<assembly_qty>();
            assembly_qty row = new assembly_qty();
            if (dt.Rows.Count > 0)
            {
                row.con_qty = pubFun.checkDecimal(dt.Rows[0]["con_qty"].ToString());
                row.sec_qty = pubFun.checkDecimal(dt.Rows[0]["sec_qty"].ToString());
            }
            else
            {
                row.con_qty = 0;
                row.sec_qty = 0;
            }
            lst.Add(row);
            return lst;
        }

        //批色流程數據
        public static List<shadding_color_info> GetShadingColorData(string out_dept,string mo_id, string goods_id )
        {
            string sql = string.Format(
              @"Select D.prod_qty,D.sec_qty,D.color_qty,D.shading_color,D.shading_color_state,D.next_wp_id,D.obligate_qty
              From jo_bill_mostly M With(nolock),jo_bill_goods_details D With(nolock)
              Where M.within_code=D.within_code And M.id=D.id And M.ver=D.ver And M.state <>'2' 
	            And D.within_code='{0}' And D.wp_id='{1}' And M.mo_id='{2}' And D.goods_id='{3}'",
            within_code,out_dept,mo_id, goods_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(sql);
            List<shadding_color_info> lst = new List<shadding_color_info>();
            shadding_color_info row = new shadding_color_info();
            if (dt.Rows.Count > 0)
            {
                row.prod_qty = string.IsNullOrEmpty(dt.Rows[0]["prod_qty"].ToString()) ? 0 : decimal.Parse(dt.Rows[0]["prod_qty"].ToString());
                row.sec_qty = string.IsNullOrEmpty(dt.Rows[0]["sec_qty"].ToString()) ? 0 : decimal.Parse(dt.Rows[0]["sec_qty"].ToString());
                row.color_qty = string.IsNullOrEmpty(dt.Rows[0]["color_qty"].ToString()) ? 0 : decimal.Parse(dt.Rows[0]["color_qty"].ToString());
                row.shading_color = dt.Rows[0]["shading_color"].ToString();
                row.shading_color_state = dt.Rows[0]["shading_color_state"].ToString();
                row.next_wp_id = dt.Rows[0]["next_wp_id"].ToString();
                row.obligate_qty = string.IsNullOrEmpty(dt.Rows[0]["obligate_qty"].ToString()) ? 0 : decimal.Parse(dt.Rows[0]["obligate_qty"].ToString());
            }
            else
            {
                row.prod_qty = 0;
                row.sec_qty = 0;
                row.color_qty = 0;
                row.shading_color = "0";
                row.shading_color_state = "0";
                row.next_wp_id = "";
                row.obligate_qty = 0;
            }
            lst.Add(row);
            return lst;
        }

        //判断生产计划是否QC流程
        public static string CheckPlanQcProcess(string mo_id,string goods_id,string out_dept,string in_dept)
        {
            //開交702本身的移交單返回的結果為空,相當于不用檢查交702部門的移交單
            string result = "";
            string sql = string.Format(
              @"Select Top 1 next_wp_id From jo_bill_mostly a with(nolock),jo_bill_goods_details b with(nolock)
               Where a.within_code=b.within_code And a.id=b.id And a.ver=b.ver
                   And a.within_code ='{0}' And a.mo_id ='{1}' And a.state <>'2' And b.goods_id ='{2}'
                   And b.wp_id ='{3}' And b.next_wp_id ='702' And b.next_wp_id <>'{4}'",
            within_code,mo_id,goods_id,out_dept,in_dept);
            DataTable dt = sh.ExecuteSqlReturnDataTable(sql);
            if (dt.Rows.Count > 0)
                result = dt.Rows[0]["next_wp_id"].ToString();
            else
                result = "";
            return result;
        }

        //判斷OC是否有QC流程
        public static string CheckOcQcProcess(string mo_id, string goods_id, string out_dept, string in_dept)
        {
            string result = "0";
            string sql = string.Format(
                @"Select count(1) as cnt 
                From so_order_manage a WITH (NOLOCK) 
                    INNER JOIN so_order_details b WITH (NOLOCK) 
                        ON a.within_code=b.within_code and a.id=b.id and a.ver=b.ver 
                    INNER JOIN so_order_production_bom c WITH (NOLOCK) 
                        ON b.within_code=c.within_code And b.id=c.id And b.ver=c.ver And b.sequence_id=c.upper_sequence
                  Where b.within_code='{0}' And b.mo_id='{1}' And b.state <>'2' And a.state <>'2' And 
                        c.charge_dept='{2}' And c.next_wp_id='{3}' And c.goods_id='{4}'",
                  within_code, mo_id, out_dept,in_dept, goods_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(sql);
            result = dt.Rows[0]["cnt"].ToString();          
            return result;
        }

        //判斷對應生產計畫中的QC流程是否已開有移交單
        public static string CheckRechangeQc(string mo_id, string goods_id, string out_dept, string in_dept)
        {
            string result = "0";
            string sql = string.Format(
                @"Select count(1) as cnt          
                From jo_materiel_con_mostly a WITH (NOLOCK),
                     jo_materiel_con_details b WITH (NOLOCK)
                Where a.within_code=b.within_code And a.id=b.id And a.within_code='{0}' And 
	                  a.out_dept='{1}' And a.in_dept='{2}' And a.bill_type_no='T' And a.state<>'2' And b.mo_id='{3}' And b.goods_id='{4}'",
                  within_code,out_dept,in_dept, mo_id, goods_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(sql);
            result = dt.Rows[0]["cnt"].ToString();
            return result;
        }
        
        //保存
        public static string Save(jo_assembly_mostly headData, List<jo_assembly_details> lstDetailData1,List<jo_assembly_details> lstDelData1,string user_id)
        {
            string str = "", lot_no = "", max_handover_id = "";
            StringBuilder sbSql = new StringBuilder(" SET XACT_ABORT ON ");
            sbSql.Append(" BEGIN TRANSACTION ");
            string id = headData.id;
            string head_insert_status = headData.head_status;
                       
            if (head_insert_status == "NEW")//全新的單據
            {
                bool id_exists = CommonDAL.CheckIdIsExists("jo_materiel_con_mostly", id);
                if (id_exists)
                {
                    //已存在此單據號,重新取最大單據號
                    //取移交單最大單號
                    string sql_f = string.Format(@"SELECT dbo.fn_zz_sys_bill_max_jo07('{0}','{1}','{2}') as id", headData.out_dept, headData.in_dept, "T");
                    DataTable dt = sh.ExecuteSqlReturnDataTable(sql_f);
                    max_handover_id = dt.Rows[0]["id"].ToString(); //max_handover_id value is DT10560134510
                    headData.id = max_handover_id;
                }
                //更新系統表移交單最大單號
                id= headData.id;
                string billCode = id.Substring(1, 12);   //biiCode value is T10560134510
                string strSql_i = string.Format(
                @" INSERT INTO sys_bill_max_jo07(within_code,bill_id,year_month,bill_code,bill_text1,bill_text2,bill_text3,bill_text4,bill_text5) 
                VALUES('0000','JO07','','{0}','T','{1}','{2}','','')", billCode, headData.out_dept, headData.in_dept);
                string strSql_u = string.Format(
                @" UPDATE sys_bill_max_jo07 SET bill_code='{0}' WHERE within_code='0000' AND bill_id='JO07' AND bill_text1='T' AND bill_text2='{1}' and bill_text3='{2}'",
                billCode, headData.out_dept, headData.in_dept);
                string sql_sys_update2 = "";
                if (billCode.Substring(7, 5) != "00001")//**
                    sql_sys_update2 = strSql_u;
                else
                    sql_sys_update2 = strSql_i;
                str = sql_sys_update2;
                sbSql.Append(str);
                                
                //生成移交單
                GenNumberDAL objNum = new GenNumberDAL();
                string update_count = "1", transfers_state = "0", bill_type_no = "T", con_type = "0", stock_type = "0", bill_origin = "2";
                string sequence_id = "", aim_jo_id = "", aim_jo_sequence = "";
                decimal jo_qty = 0, c_qty = 0, package_num = 0;
                int index;
                if (lstDetailData1.Count > 0)
                {
                    //移交單表頭                    
                    str = string.Format(
                    @" Insert Into jo_materiel_con_mostly(within_code,id,con_date,handler, update_count,transfers_state,remark,state,bill_type_no,out_dept,in_dept,
                    con_type,stock_type,bill_origin,create_by,create_date,servername,update_date,update_by) VALUES
                    ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',getdate(),'{15}',getdate(),'{16}')",
                    within_code, headData.id, headData.con_date, headData.handler, update_count, transfers_state, headData.remark, headData.state,
                    bill_type_no, headData.out_dept, headData.in_dept, con_type, stock_type, bill_origin, headData.create_by, ls_servername, headData.create_by);
                    sbSql.Append(str);
                    //移交單明細
                    index = 0;
                    foreach (var item in lstDetailData1)
                    {
                        index += 1;
                        sequence_id = objNum.GetSequenceID(index);// index.ToString().PadLeft(4, '0')+"h"; //移交單的序號
                        aim_jo_id = null;// item.id;//組裝單號
                        aim_jo_sequence = null;// item.sequence_id;//組裝單序號
                        str = string.Format(
                        @" Insert Into jo_materiel_con_details
                        (within_code,id,sequence_id,jo_id,jo_sequence_id,goods_id,jo_qty,c_qty,con_qty,unit_code,sec_qty,sec_unit,remark,mo_id,package_num,
                        location,carton_code,lot_no,aim_jo_id,aim_jo_sequence,qc_result,app_supply_side) Values
                        ('{0}','{1}','{2}','{3}','{4}','{5}', {6},{7},{8}, '{9}',{10},'{11}','{12}','{13}',{14},'{15}','{16}','{17}','{18}','{19}','{20}','{21}')",
                        within_code, headData.id, sequence_id, item.jo_id, item.jo_sequence_id, item.goods_id, jo_qty,c_qty,item.con_qty, item.unit_code,
                        item.sec_qty, item.sec_unit, item.remark, item.mo_id, package_num, headData.out_dept, headData.out_dept, item.lot_no, aim_jo_id,aim_jo_sequence,
                        item.qc_result,item.app_supply_side);
                        sbSql.Append(str);
                    }
                }                
            }
            else //已保存移交單的基礎上進行增刪改
            {
                //首先處理刪除(表格一) 
                //注意加選項with(ROWLOCK),只有用到主鍵時才會用到行級鎖
                if (lstDelData1 != null)
                {
                    foreach (var item in lstDelData1)
                    {
                        str = string.Format(
                            @" DELETE FROM jo_materiel_con_details with(ROWLOCK) Where within_code='0000' AND id='{0}' AND sequence_id='{1}'",
                            item.id, item.sequence_id);
                        sbSql.Append(str);                       
                    }
                }               
                //更新組裝轉換表頭
                str = string.Format(
                @" UPDATE jo_materiel_con_mostly with(ROWLOCK) 
                   SET con_date='{2}',handler='{3}',remark='{4}',update_by='{5}', update_date=getdate(),out_dept='{6}',in_dept='{7}',
                       update_count = Convert(nvarchar(5),Convert(int,update_count)+1)
                   WHERE within_code ='{0}' AND id ='{1}'",
                within_code, headData.id, headData.con_date, headData.handler, headData.remark, headData.update_by, headData.out_dept, headData.in_dept);
                sbSql.Append(str);
                //更新明細表一 
                decimal ldc_con_qty = 0, ldc_sec_qty = 0;              
                foreach (var item in lstDetailData1)
                {
                    if (!string.IsNullOrEmpty(item.row_status))
                    {
                        //item.row_status非空,數據有新增或修改
                        if (item.qc_qty > 0 && item.con_qty > item.qc_qty)
                        {
                            ldc_con_qty = item.con_qty - item.qc_qty;
                            ldc_sec_qty = item.sec_qty - decimal.Parse("0.01");
                        }
                        else
                        {
                            ldc_con_qty = item.con_qty;
                            ldc_sec_qty = item.sec_qty;
                        }
                        if (item.row_status == "EDIT")
                        {
                            lot_no = item.lot_no; //編輯時原記錄已有批號
                            if (string.IsNullOrEmpty(lot_no))
                            {
                                lot_no = CommonDAL.GetDeptLotNo(headData.out_dept, headData.in_dept); //重新獲取批號
                            }                            
                            //更新移交單明細
                            str = string.Format(
                            @" UPDATE jo_materiel_con_details 
                            SET mo_id='{3}', goods_id='{4}',jo_id='{5}',jo_sequence_id='{6}',con_qty={7},unit_code='{8}',sec_qty={9},sec_unit='{10}',remark='{11}',
                            package_num={12},location='{13}',carton_code='{14}',lot_no='{15}',aim_jo_id='{16}',aim_jo_sequence='{17}', qc_result='{18}',
                            app_supply_side='{19}'
                            WHERE within_code='{0}' And id='{1}' And sequence_id='{2}'", within_code, item.id, item.sequence_id,
                            item.mo_id, item.goods_id,item.jo_id, item.jo_sequence_id, ldc_con_qty, item.unit_code, ldc_sec_qty, item.sec_unit, item.remark,
                            item.package_num,item.location,item.carton_code,lot_no,item.aim_jo_id,item.aim_jo_sequence,item.qc_result,item.app_supply_side);
                            sbSql.Append(str);                            
                        }
                        else //INSERT ITEM//有項目新增
                        {
                            //插入新增的記錄
                            decimal jo_qty = 0, c_qty = 0;
                            string aim_jo_id = null, aim_jo_sequence = null;
                            //新增的記錄插入已存在的移交單中
                            str = string.Format(
                            @" Insert Into jo_materiel_con_details
                            (within_code,id,sequence_id,jo_id,jo_sequence_id,goods_id,jo_qty,c_qty,con_qty,unit_code,sec_qty,sec_unit,remark,mo_id,package_num,
                            location,carton_code,lot_no,aim_jo_id,aim_jo_sequence,qc_result,app_supply_side) Values
                            ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},'{9}',{10},'{11}','{12}','{13}',{14},'{15}','{16}','{17}','{18}','{19}','{20}')",
                            within_code, headData.id, item.sequence_id, item.jo_id, item.jo_sequence_id, item.goods_id, jo_qty, c_qty, ldc_con_qty, item.unit_code,
                            ldc_sec_qty,item.sec_unit,item.remark,item.mo_id,item.package_num,headData.out_dept,headData.out_dept,item.lot_no,aim_jo_id,aim_jo_sequence,
                            item.qc_result,item.app_supply_side);
                            sbSql.Append(str);
                        } //end of item add

                    } //end is edit
                } //end for
                
                //更新組裝轉換單中的移交單號字段
                string ls_aim_jo_id = "";
                foreach (var item in lstDetailData1)
                {
                    //理论上要求同一张移交单需要使用同一张组装转换单的数据,所以只以第一笔数据的组装单号为准
                    ls_aim_jo_id = item.aim_jo_id;
                    break;
                }
                if (!string.IsNullOrEmpty(ls_aim_jo_id))
                {
                    str = string.Format(
                        @" Update A WITH(ROWLOCK) Set A.handover_id ='{2}'
		                   From jo_assembly_mostly A 
                           Where A.within_code='{0}' And A.id='{1}' And Isnull(A.handover_id,'')=''",
                        within_code, ls_aim_jo_id, headData.id);
                    sbSql.Append(str);
                }
            }
            sbSql.Append(@" COMMIT TRANSACTION ");
            string result = sh.ExecuteSqlUpdate(sbSql.ToString());
            sbSql.Clear();
            return result;
        }// --end save

        public static string Approve(jo_assembly_mostly head, string user_id, string approve_type)
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
            result = ApproveRechange(head, ldt_check_date, active_name, user_id);          
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
        public static string ApproveRechange(jo_assembly_mostly headData, string check_date, string active_name, string user_id)
        {
            //生成移交單交易、更新移交單相關庫存
            string window_id = "";
            string result = ProduceAssemblyDAL.SetRechangeStBusiness(headData.id, check_date, active_name, "JO", user_id, window_id); //active_name:"pfc_ok","pfc_unok"
            if (result.Substring(0, 2) == "00") // SetRechangeStBusiness()執行成功
            {
                string strSql = "", checkDate = "", checkBy = "", state = "0";
                if (active_name == "pfc_ok")
                {
                    //更新移交單批準狀態 
                    checkDate = check_date;
                    checkBy = user_id;
                    state = "1";
                }
                else
                {
                    checkDate = "";
                    checkBy = "";
                    state = "0";
                }
                strSql = string.Format(
                        @" Update jo_materiel_con_mostly with(Rowlock) 
                           SET check_date=(Case When '{2}'<>'' Then '{2}' Else null End), check_by='{3}',
                               update_date=(Case When '{2}'<>'' Then '{2}' Else getdate() End),update_by='{4}',state='{5}' 
                           WHERE within_code='{0}' And id='{1}'", within_code, headData.id, checkDate, checkBy, user_id, state);
                result = sh.ExecuteSqlUpdate(strSql);
                result = (result == "") ? "00" : "-1" + result;
            }
            return result;
        }

        //檢查庫存,返回庫存差異數
        public static List<check_part_stock> CheckStorage(string id)
        {
            string strSql = string.Format(
                @"SELECT TOP 1 S.out_dept,S.sequence_id,S.mo_id,S.goods_id,S.lot_no,(ISNULL(C.qty,0)-S.con_qty) AS qty,(ISNULL(C.sec_qty,0)-S.sec_qty) AS sec_qty 
                FROM (SELECT A.within_code,A.out_dept,B.sequence_id,B.mo_id,B.goods_id,B.lot_no,B.con_qty,B.sec_qty
	                    FROM jo_materiel_con_mostly A With(nolock),jo_materiel_con_details B With(nolock) 
	                    WHERE A.within_code=B.within_code AND A.id=B.id AND A.within_code='{0}' AND A.id='{1}' AND A.state<>'2'
	                 ) S  
	                LEFT JOIN st_details_lot C with(nolock)
		                ON S.within_code=C.within_code AND S.out_dept=C.location_id AND S.out_dept=C.carton_code AND
		                   S.goods_id=C.goods_id AND S.lot_no=C.lot_no AND S.mo_id=C.mo_id AND ISNULL(C.sec_qty,0)>0
                WHERE S.con_qty>ISNULL(C.qty,0) OR S.sec_qty>ISNULL(C.sec_qty,0)", within_code, id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            List<check_part_stock> lstDetail = new List<check_part_stock>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                check_part_stock lst = new check_part_stock();
                lst.out_dept = dt.Rows[i]["out_dept"].ToString();
                //lst.upper_sequence = dt.Rows[i]["upper_sequence"].ToString();
                lst.sequence_id = dt.Rows[i]["sequence_id"].ToString();
                lst.mo_id = dt.Rows[i]["mo_id"].ToString();
                lst.goods_id = dt.Rows[i]["goods_id"].ToString();
                lst.lot_no = dt.Rows[i]["lot_no"].ToString();
                lst.con_qty = decimal.Parse(dt.Rows[i]["qty"].ToString());
                lst.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                lstDetail.Add(lst);
            }
            return lstDetail;
        }

        //注銷
        public static string DeleteHead(jo_assembly_mostly head,string user_id)
        {
            string result = "";
            string strSql = string.Format(
                @"UPDATE jo_materiel_con_mostly with(Rowlock) 
                SET state='2',update_by='{0}',update_date=getdate() 
                WHERE within_code='0000' AND id='{1}'", user_id, head.id);
            result = sh.ExecuteSqlUpdate(strSql);
            return result;//成功返回空格
        }




    }
}
