using CF.Core.Config;
using CF.SQLServer.DAL;
using CF2025.Base.DAL;
using CF2025.Prod.Contract;
using CF2025.Prod.Contract.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Prod.DAL
{
    public static class IssueAppriseListDAL
    {
        public static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        public static PubFunDAL pubFunction = new PubFunDAL();
        public static string within_code = "0000";
        public static string ls_servername = "hkserver.cferp.dbo";
        public static string ldt_check_date = "";
        public static StringBuilder sbSql = new StringBuilder();

        public static List<query_data> QueryData(query_condition query)
        {
            StringBuilder strSb = new StringBuilder(
            @"SELECT a.within_code,a.id,a.ver,a.materiel_id,a.goods_id,a.name,a.location,a.carton_code,a.unit,a.sec_unit,
		        SUM(Isnull(a.fl_qty,0)) As 'fl_qty',SUM(Isnull(a.issues_qty,0)) As 'issues_qty',SUM(Isnull(a.sec_qty,0)) As 'sec_qty',
		        SUM(Isnull(a.i_sec_qty,0)) As 'already_sec_qty',a.color,a.mo_id,a.dept_id,a.upper_sequence,a.plate_effect,a.outer_layer,
		        a.color_effect,MAX(a.mrp_id) as mrp_id,MAX(T2.name) AS location_name,MAX(T3.name) AS carton_name,MAX(T4.name) AS dept_name,
		        MAX(T4.name) AS dept_name_en,MAX(T5.name) AS color_name,MAX(T6.name) AS unit_name,MAX(T7.name) AS unit_name,MAX(T8.name) AS plate_name,
		        MAX(T9.name) AS outer_name,MAX(T10.name) AS color_effect_name, @@SPID AS SPID
            FROM v_qry_bill_goods_noissues a
	            LEFT JOIN cd_productline T2 ON a.within_code = T2.within_code AND a.location = T2.id
	            LEFT JOIN cd_carton_code T3 ON a.within_code = T3.within_code AND a.location = T3.location_id AND a.carton_code = T3.ID
	            LEFT JOIN cd_department T4 ON a.within_code = T4.within_code AND a.dept_id = T4.id
	            LEFT JOIN cd_color T5 ON a.within_code = T5.within_code AND a.color = T5.id
	            LEFT JOIN cd_units T6 ON a.within_code = T6.within_code AND a.unit = T6.id
	            LEFT JOIN cd_units T7 ON a.within_code = T7.within_code AND a.sec_unit = T7.id
	            LEFT JOIN cd_plate_effect T8 ON a.within_code = T8.within_code AND a.plate_effect = T8.id
	            LEFT JOIN cd_outer T9 ON a.within_code = T9.within_code AND a.outer_layer = T9.id
	            LEFT JOIN cd_color_effect T10 ON a.within_code = T10.within_code AND a.color_effect = T10.id
            Where Not ( (IsNull(a.fl_qty,0)>0 And (IsNull(a.fl_qty,0)-IsNull(a.issues_qty,0)<=0)) Or 
                        (IsNull(a.sec_qty,0)>0 And (IsNull(a.sec_qty,0)-IsNull(a.i_sec_qty,0)<=0)) ) 
                  And (IsNull(a.fl_qty,0)>0 Or IsNull(a.sec_qty,0)>0) 
                  And Isnull(a.dept_id,'')<>'702' And (a.within_code='0000') "
            );
            //生產單編號
            if (!string.IsNullOrEmpty(query.jo_id_s))
            {
                strSb.Append(string.Format(@" And a.id >='{0}'", query.jo_id_s));
            }
            if (!string.IsNullOrEmpty(query.jo_id_e))
            {
                strSb.Append(string.Format(@" And a.id <='{0}'", query.jo_id_e));
            }
            //頁數
            if (!string.IsNullOrEmpty(query.mo_id_s))
            {
                strSb.Append(string.Format(@" And a.mo_id >='{0}'", query.mo_id_s));
            }
            if (!string.IsNullOrEmpty(query.mo_id_e))
            {
                strSb.Append(string.Format(@" And a.mo_id <='{0}'", query.mo_id_e));
            }
            //貨品編碼
            if (!string.IsNullOrEmpty(query.goods_id_s))
            {
                strSb.Append(string.Format(@" And a.materiel_id >='{0}'", query.goods_id_s));
            }
            if (!string.IsNullOrEmpty(query.goods_id_e))
            {
                strSb.Append(string.Format(@" And a.materiel_id <='{0}'", query.goods_id_e));
            }
            //負責部門
            if (!string.IsNullOrEmpty(query.charge_dept_s))
            {
                strSb.Append(string.Format(@" And a.dept_id >='{0}'", query.charge_dept_s));
            }
            if (!string.IsNullOrEmpty(query.charge_dept_e))
            {
                strSb.Append(string.Format(@" And a.dept_id <='{0}'", query.charge_dept_e));
            }
            //倉庫
            if (!string.IsNullOrEmpty(query.location_id))
            {
                strSb.Append(string.Format(@" And a.location ='{0}'", query.location_id));
            }
            //理論完成日期
            if (!string.IsNullOrEmpty(query.production_date_s))
            {
                strSb.Append(string.Format(@" And Convert(Char(19),a.production_date,120)>='{0}'", query.production_date_s));
            }
            if (!string.IsNullOrEmpty(query.production_date_e))
            {
                strSb.Append(string.Format(@" And Convert(Char(19),a.production_date,120)<='{0}'", query.production_date_e));
            }
            //批準日期
            if (!string.IsNullOrEmpty(query.check_date_s))
            {
                strSb.Append(string.Format(@" And Convert(Char(19),a.check_date,120)>='{0}'", query.check_date_s));
            }
            if (!string.IsNullOrEmpty(query.check_date_e))
            {
                strSb.Append(string.Format(@" And Convert(Char(19),a.check_date,120)<='{0}'", query.check_date_e));
            }
            strSb.Append(
             @" Group By a.within_code,a.id,a.ver,a.materiel_id,a.goods_id,a.name,a.location,a.carton_code,a.unit,
                a.sec_unit,a.color,a.mo_id,a.dept_id,a.upper_sequence,a.plate_effect,a.outer_layer,a.color_effect");
            //string strSql = string.Format(@"SELECT dbo.fn_zz_sys_bill_max_separate_st('{0}',{1}) as id", "ST03", 4);
            //DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSb.ToString());
            List<query_data> lstDetail = new List<query_data>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                query_data mdj = new query_data();
                mdj.id = dt.Rows[i]["id"].ToString();
                mdj.ver = dt.Rows[i]["ver"].ToString();
                mdj.materiel_id = dt.Rows[i]["materiel_id"].ToString();
                mdj.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdj.name = dt.Rows[i]["name"].ToString();
                mdj.location = dt.Rows[i]["location"].ToString();
                mdj.carton_code = dt.Rows[i]["carton_code"].ToString();
                mdj.unit = dt.Rows[i]["unit"].ToString();
                mdj.sec_unit = dt.Rows[i]["sec_unit"].ToString();
                mdj.fl_qty = decimal.Parse(dt.Rows[i]["fl_qty"].ToString());
                mdj.issues_qty = decimal.Parse(dt.Rows[i]["issues_qty"].ToString());
                mdj.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                mdj.already_sec_qty = decimal.Parse(dt.Rows[i]["already_sec_qty"].ToString());
                mdj.color = dt.Rows[i]["color"].ToString();
                mdj.mo_id = dt.Rows[i]["mo_id"].ToString();
                mdj.dept_id = dt.Rows[i]["dept_id"].ToString();
                mdj.upper_sequence = dt.Rows[i]["upper_sequence"].ToString();
                mdj.plate_effect = dt.Rows[i]["plate_effect"].ToString();
                mdj.outer_layer = dt.Rows[i]["outer_layer"].ToString();
                mdj.color_effect = dt.Rows[i]["color_effect"].ToString();
                mdj.mrp_id = dt.Rows[i]["mrp_id"].ToString();
                mdj.location_name = dt.Rows[i]["location_name"].ToString();
                mdj.carton_name = dt.Rows[i]["carton_name"].ToString();
                mdj.dept_name = dt.Rows[i]["dept_name"].ToString();
                mdj.dept_name_en = dt.Rows[i]["dept_name_en"].ToString();
                mdj.color_name = dt.Rows[i]["color_name"].ToString();
                mdj.unit_name = dt.Rows[i]["unit_name"].ToString();
                mdj.plate_name = dt.Rows[i]["plate_name"].ToString();
                mdj.outer_name = dt.Rows[i]["outer_name"].ToString();
                mdj.color_effect_name = dt.Rows[i]["color_effect_name"].ToString();
                mdj.SPID = dt.Rows[i]["spid"].ToString();
                mdj.key_id = i.ToString() + "h";                
                lstDetail.Add(mdj);
            }
            return lstDetail;
        }

        public static List<query_data_sub> QueryDataSub(string location_id, string carton_code, string mo_id, string materiel_id, string upper_sequence,string key_id)
        {
            StringBuilder strSb = new StringBuilder(
            @"SELECT a.within_code,a.id,a.upper_sequence,a.sequence_id,a.materiel_id,a.goods_id,a.name,a.location,a.carton_code,
            a.basic_unit,a.unit,isnull(a.fl_qty,0) As fl_qty,isnull(a.issues_qty,0) As issues_qty,isnull(a.r_qty,0) As r_qty,
            isnull(a.ir_qty,0) As ir_qty,a.remark,a.sec_unit,isnull(a.sec_qty,0) As sec_qty,a.color,isnull(a.wastage_percent,0) As wastage_percent,
            a.ver,Isnull(a.base_qty,0) As base_qty,a.mo_id,a.dept_id,a.production_date,a.mrp_id,a.contract_cid,a.so_order_id,a.so_sequence_id,   
            a.obligate_mo_id,a.lot_no,Isnull(a.i_sec_qty,0) As 'already_sec_qty'
            FROM v_qry_bill_goods_noissues a  
            Where Not ( (IsNull(a.fl_qty,0)>0 And (IsNull(a.fl_qty,0)-IsNull(a.issues_qty,0)<=0)) OR
	              (IsNull(a.sec_qty,0)>0 And (IsNull(a.sec_qty,0) -IsNull(a.i_sec_qty,0)<=0)) ) And
	              (IsNull(a.fl_qty,0)>0 Or IsNull(a.sec_qty,0)>0) And a.dept_id<>'702' And a.within_code='0000' "
            );
            //倉庫
            if (!string.IsNullOrEmpty(location_id))
            {
                strSb.Append(string.Format(@" And a.location ='{0}'", location_id));
            }
            //倉位
            if (!string.IsNullOrEmpty(carton_code))
            {
                strSb.Append(string.Format(@" And a.carton_code ='{0}'", carton_code));
            }
            //頁數
            if (!string.IsNullOrEmpty(mo_id))
            {
                strSb.Append(string.Format(@" And a.mo_id ='{0}'", mo_id));
            }
            //貨品編碼
            if (!string.IsNullOrEmpty(materiel_id))
            {
                strSb.Append(string.Format(@" And a.materiel_id ='{0}'", materiel_id));
            }
            //序號
            if (!string.IsNullOrEmpty(upper_sequence))
            {
                strSb.Append(string.Format(@" And a.upper_sequence ='{0}'", upper_sequence));
            }
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSb.ToString());
            List<query_data_sub> lstDetail = new List<query_data_sub>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                query_data_sub mdj = new query_data_sub();
                mdj.production_date = dt.Rows[i]["production_date"].ToString();
                mdj.fl_qty = decimal.Parse(dt.Rows[i]["fl_qty"].ToString());
                mdj.issues_qty = decimal.Parse(dt.Rows[i]["issues_qty"].ToString());
                mdj.unit = dt.Rows[i]["unit"].ToString();
                mdj.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                mdj.already_sec_qty = decimal.Parse(dt.Rows[i]["already_sec_qty"].ToString());
                mdj.sec_unit = dt.Rows[i]["sec_unit"].ToString();
                mdj.remark = dt.Rows[i]["remark"].ToString();
                mdj.color = dt.Rows[i]["color"].ToString();
                mdj.mrp_id = dt.Rows[i]["mrp_id"].ToString();
                mdj.contract_cid = dt.Rows[i]["contract_cid"].ToString();
                mdj.so_order_id = dt.Rows[i]["so_order_id"].ToString();
                mdj.obligate_mo_id = dt.Rows[i]["obligate_mo_id"].ToString();
                mdj.lot_no = dt.Rows[i]["lot_no"].ToString();
                mdj.id = dt.Rows[i]["id"].ToString();
                mdj.ver = dt.Rows[i]["ver"].ToString();
                mdj.upper_sequence = dt.Rows[i]["upper_sequence"].ToString();
                mdj.sequence_id = dt.Rows[i]["sequence_id"].ToString();
                mdj.so_sequence_id = dt.Rows[i]["so_sequence_id"].ToString();
                mdj.materiel_id = dt.Rows[i]["materiel_id"].ToString();
                mdj.name = dt.Rows[i]["name"].ToString();
                mdj.basic_unit = dt.Rows[i]["basic_unit"].ToString();
                mdj.r_qty = decimal.Parse(dt.Rows[i]["r_qty"].ToString());
                mdj.ir_qty = decimal.Parse(dt.Rows[i]["ir_qty"].ToString());
                mdj.base_qty = decimal.Parse(dt.Rows[i]["base_qty"].ToString());
                mdj.wastage_percent = decimal.Parse(dt.Rows[i]["wastage_percent"].ToString());
                mdj.mo_id = dt.Rows[i]["mo_id"].ToString();
                mdj.dept_id = dt.Rows[i]["dept_id"].ToString();
                mdj.location = dt.Rows[i]["location"].ToString();
                mdj.carton_code = dt.Rows[i]["carton_code"].ToString();
                mdj.key_id = key_id;
                lstDetail.Add(mdj);
            }
            return lstDetail;
        }

        /// <summary>
        /// 批準/反批準,生成組裝單,移交單的交易,庫存更新等(失敗返回的字串前兩位是"-1")
        /// </summary>
        /// <param name="id">組裝單號</param>        
        /// <param name="user_id">當前用戶</param>
        /// <param name="approve_type">approve_type:1--批準;0--反批準</param>
        /// <returns>返回字串為空,表示成功</returns>        
        public static string Approve(st_inventory_mostly head, string user_id, string approve_type)
        {
            string result = "", return_value = "", strSql = "";
            string ls_id = string.Empty, ls_origin="", ls_dept_id="", ls_servername="", ldt_date="", ldt_check_date="";
            ls_id = head.id;
            ls_origin = head.origin;
            ldt_date = head.inventory_date;
            ls_dept_id = head.department_id;
            
            string active_name = approve_type == "1" ? "批準" : "反批準";
            DataTable dt = new DataTable();

            //--start 批準
            if (approve_type == "1")
            {
                //設置全局的批準日期
                ldt_check_date = CommonDAL.GetDbDateTime("L");//批準日期(長日期時間)

                // --start action_id:31
                //插入库存交易表//扣除原料库存
                strSql = string.Format(
                @" INSERT INTO st_business_record(within_code, id, goods_id, goods_name, unit, action_time,
                                                action_id,
                                                ii_qty, ii_location_id, ii_code, rate, sequence_id,operator, check_date,
                                                ib_qty, qty, mo_id, lot_no, sec_qty, sec_unit, dept_id, servername)
                SELECT b.within_code, b.id, b.goods_id, b.goods_name, b.unit_code, a.con_date,
                         '31',--//组装转换原料出库
                         Abs(b.con_qty), C.location, C.location, 0 as swit_rate, b.sequence_id,'{2}','{3}',
					     Abs(dbo.FN_CHANGE_UNITBYCV(b.within_code, b.goods_id, b.unit_code, 1, '', '', '*')),
					     Abs(round(dbo.FN_CHANGE_UNITBYCV(b.within_code, b.goods_id, b.unit_code, b.con_qty, '', '', '/'), 4)),
					     b.mo_id,b.lot_no,Abs(b.sec_qty),b.sec_unit,A.out_dept,'{4}'
                FROM     jo_assembly_mostly a,
                         jo_assembly_details_part b,
					     cd_department C
                Where a.within_code = b.within_code and a.id = b.id and a.within_code ='{0}' and
                        A.within_code = C.within_code And A.out_dept = C.id And a.id ='{1}' and isNull(C.location, '') <> ''",
                within_code, head.id, user_id, ldt_check_date, ls_servername);
                strSql = RetunSqlString(strSql);
                result = sh.ExecuteSqlUpdate(strSql);                
                if (result == "")
                {
                    result = "00";//成功
                }
                else
                {
                    result = "-1" + result + "\r\n" + "交易数据保存失败!" + "\r\n" + "<" + active_name + ">(st_business_record)" + "\r\n";
                    return result;
                }
                //更新库存
                return_value = pubFunction.of_update_st_details("I", "31", head.id, "*", ldt_check_date, "");
                if (return_value.Substring(0, 2) == "-1")
                {
                    result = return_value + "\r\n" + "庫存數據保存失败![扣除原料库存]+" + "\r\n" + "<" + active_name + ">(st_details)" + "\r\n";
                    return result;
                }
                // --end action_id:31

                //start action_id:51
                //--添加废料的库存增加记录
                int ll_cnt = 0;
                strSql = string.Format(
                @"Select Count(1) as cnt
                FROM jo_assembly_mostly a,
                     jo_assembly_details_scrap b,
                     cd_department C
                Where a.within_code=b.within_code and a.id=b.id and a.within_code ='{0}' and
                      A.within_code=C.within_code And A.out_dept=C.id And a.id='{1}' and isNull(C.location,'')<>'' ", within_code, head.id);
                dt = sh.ExecuteSqlReturnDataTable(strSql);
                ll_cnt = int.Parse(dt.Rows[0]["cnt"].ToString());
                if (ll_cnt > 0)
                {
                    strSql = string.Format(
                    @"INSERT INTO st_business_record(within_code, id, goods_id, goods_name, unit, action_time,
                                                    action_id,
                                                    ii_qty, ii_location_id, ii_code, rate, sequence_id,operator, check_date,
                                                    ib_qty, qty, mo_id, lot_no, sec_qty, sec_unit, dept_id, servername)
                    SELECT b.within_code, b.id, b.goods_id, b.goods_name, b.unit_code, a.con_date,
                                 '51',--//组装转换废料入库
                                 Abs(b.con_qty), C.location, C.location, 0 as swit_rate, b.sequence_id,'{2}','{3}',
						 Abs(dbo.FN_CHANGE_UNITBYCV(b.within_code, b.goods_id, b.unit_code, 1, '', '', '*')),
						 Abs(round(dbo.FN_CHANGE_UNITBYCV(b.within_code, b.goods_id, b.unit_code, b.con_qty, '', '', '/'), 4)),
						 b.mo_id,b.lot_no,Abs(b.sec_qty),b.sec_unit,a.out_dept,'{4}'
                    FROM  jo_assembly_mostly a, jo_assembly_details_scrap b, cd_department C
                    Where a.within_code = b.within_code and a.id = b.id and a.within_code ='{0}' and
                          a.within_code=C.within_code And a.out_dept=C.id And a.id='{1}' and isNull(C.location,'') <> ''",
                    within_code, head.id, user_id, ldt_check_date, ls_servername);
                    strSql = RetunSqlString(strSql);                   
                    result = sh.ExecuteSqlUpdate(strSql);
                   
                    if (result == "")
                    {
                        result = "00";
                    }
                    else
                    {
                        result = "-1" + result + "\r\n" + "添加废料交易数据保存失败!" + "\r\n" + "<" + "批準" + ">(po_receipt_details)" + "\r\n";
                        return result;
                    }
                    //更新库存
                    return_value = pubFunction.of_update_st_details("I", "51", head.id, "*", ldt_check_date, "");
                    if (return_value.Substring(0, 2) == "-1")
                    {
                        result = return_value + "\r\n" + "庫存數據保存失败![废料]" + "\r\n" + "<" + active_name + ">(st_details)" + "\r\n";
                        return result;
                    }
                    // --end action_id:51
                }

                //--start action_id:29
                //--添加成品的库存增加记录
                strSql = string.Format(
                @" INSERT INTO st_business_record(within_code, id, goods_id, goods_name, unit, action_time,
                                                action_id,
                                                ii_qty, ii_location_id,
                                                ii_code, rate, sequence_id,operator, check_date,
                                                ib_qty, qty, mo_id, lot_no, sec_qty, sec_unit, dept_id, servername)
                SELECT b.within_code,b.id,b.goods_id,b.goods_name,b.unit_code,a.con_date,
                    '29',--//组装转换入库
                    Abs(b.con_qty),C.location,C.location,0 as swit_rate,b.sequence_id,'{2}','{3}',
                    Abs(dbo.FN_CHANGE_UNITBYCV(b.within_code, b.goods_id, b.unit_code, 1, '', '', '*')),
                    Abs(round(dbo.FN_CHANGE_UNITBYCV(b.within_code, b.goods_id, b.unit_code, b.con_qty, '', '', '/'), 4)),
                    b.mo_id,b.lot_no,Abs(b.sec_qty),b.sec_unit,A.out_dept,'{4}'
                FROM  jo_assembly_mostly a,
                      jo_assembly_details b,
                      cd_department C
                Where a.within_code = b.within_code and a.id = b.id and a.within_code ='{0}' and
                      a.within_code=C.within_code And a.out_dept=C.id And a.id='{1}' and IsNull(C.location,'')<>'' ",
                within_code, head.id, user_id, ldt_check_date, ls_servername);
                strSql = RetunSqlString(strSql);                
                result = sh.ExecuteSqlUpdate(strSql);                
                if (result == "")
                {
                    result = "00";
                }
                else
                {
                    result = "-1" + result + "\r\n" + "交易数据保存失败![成品组装转换入库]" + "\r\n" + "<" + active_name + ">(jo_assembly_details)" + "\r\n";
                    return result;
                }
                //更新库存                                  
                return_value = pubFunction.of_update_st_details("I", "29", head.id, "*", ldt_check_date, "");
                if (return_value.Substring(0, 2) == "-1")
                {
                    //数据保存失败
                    result = return_value + "\r\n" + "数据保存失败!<批準>((st_details)";
                    return result;
                }
                //--end action_id:29               
               

                //批準移交單(其中包含有調用生成移交單交易、更新移交單相關庫存的方法)
                //result = ApproveRechange(head, head.handover_id, ldt_check_date, user_id);

            } //--end 批準

            //--start 反批準組裝轉換單
            if (approve_type == "0")
            {
                ldt_check_date = head.check_date;
                int ll_cnt = 0;
                strSql = string.Format(
                @"Select Count(1) as cnt FROM st_business_record WITH (NOLOCK) Where within_code='{0}' and id='{1}' And action_id In('29','31','51')",
                within_code, head.id);
                dt = sh.ExecuteSqlReturnDataTable(strSql);
                ll_cnt = int.Parse(dt.Rows[0]["cnt"].ToString());

                if (ll_cnt > 0)
                {
                    //還原庫存
                    //更新库存
                    return_value = pubFunction.of_update_st_details("D", "29", head.id, "*", ldt_check_date, "");
                    if (return_value.Substring(0, 2) == "-1")
                    {
                        result = return_value + "数据保存失败![D,29]" + "\r\n" + "<" + active_name + ">(st_details)" + "\r\n";
                        return result;
                    }
                    //更新库存
                    return_value = pubFunction.of_update_st_details("D", "31", head.id, "*", ldt_check_date, "");
                    if (return_value.Substring(0, 2) == "-1")
                    {
                        result = return_value + "\r\n" + "数据保存失败![D,31]" + "\r\n" + "<" + active_name + ">(st_details)" + "\r\n";
                        return result;
                    }
                    //---
                    ll_cnt = 0;
                    strSql = string.Format(
                    @"Select Count(1) as cnt FROM st_business_record WITH(NOLOCK) Where within_code='{0}' and id='{1}' And action_id='51'", within_code, head.id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    ll_cnt = int.Parse(dt.Rows[0]["cnt"].ToString());
                    if (ll_cnt > 0)
                    {
                        //更新库存
                        return_value = pubFunction.of_update_st_details("D", "51", head.id, "*", ldt_check_date, "");
                        if (return_value.Substring(0, 2) == "-1")
                        {
                            result = return_value + "\r\n" + "数据保存失败![D,51]" + "\r\n" + "<" + active_name + ">(st_details)" + "\r\n";
                            return result;
                        }
                    }
                    //---
                    //刪除相并交易
                    strSql = string.Format(
                    @" Delete FROM st_business_record WITH(ROWLOCK) Where within_code='{0}' and id='{1}' And action_id In('31','51','29')", within_code, head.id);
                    strSql = RetunSqlString(strSql);
                    result = sh.ExecuteSqlUpdate(strSql);                   
                    if (result == "")
                    {
                        result = "00";
                    }
                    else
                    {
                        result = "-1" + result + "\r\n" + "数据保存失败![刪除交易數據:'31','51','29]" + "\r\n" + "<" + active_name + ">(st_business_record)" + "\r\n";
                        return result;
                    }
                }// if ll_cnt>0
                //无条件控制负数的出现2012-05-22??               
                //dt = pubFunction.wf_check_inventory_qty(id, "w_produce_assembly");
                //if (dt.Rows[0]["id"].ToString() == "-1")
                //{                    
                //    result = dt.Rows[0]["errtext"].ToString();                    
                //    return result;
                //}
            } //--end 反批準
            return result;
        }//--end 批準/反批準
        
        public static string RetunSqlString(string strSql)
        {
            string str = string.Empty;                     
            sbSql.Clear();
            sbSql.Append(" SET XACT_ABORT ON ");
            sbSql.Append(" BEGIN TRANSACTION ");
            sbSql.Append(strSql);
            sbSql.Append(@" COMMIT TRANSACTION ");
            str = sbSql.ToString();
            sbSql.Clear();
            return str;
        }
        

    }
}
