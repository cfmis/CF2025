﻿using CF.Core.Config;
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
        /// 保存倉庫發料
        /// </summary>
        /// <param name="headData"></param>
        /// <param name="lstDetailData1"></param>
        /// <param name="lstDetailData2"></param>
        /// <param name="lstDelData1"></param>
        /// <param name="lstDelData2"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public static string Save(st_inventory_mostly headData, List<st_i_subordination> lstDetailData1, List<st_cc_details_schedule> lstDetailData2,
            List<st_i_subordination> lstDelData1, List<st_cc_details_schedule> lstDelData2, string user_id)
        {
            string result = "00";
            string str = "" ;
            StringBuilder sbSql = new StringBuilder(" SET XACT_ABORT ON ");
            sbSql.Append(" BEGIN TRANSACTION ");
            string id = headData.id;
            string head_insert_status = headData.head_status;

            if (head_insert_status == "NEW")//全新的單據
            {
                bool id_exists = CommonDAL.CheckIdIsExists("st_inventory_mostly", id);
                if (id_exists)
                {
                    //已存在此單據號,重新取最大單據號
                    headData.id = CommonDAL.GetMaxID("ST03", 4); //DAA24010132,轉倉是ST10
                }
                //***begin 更新系統表倉庫發料最大單據編號
                //string dept_id = headData.out_dept;//105
                string year_month = headData.id.Substring(3, 4); //2401
                string bill_code = headData.id.Substring(1, 10); //AA24010132
               
                string sql_sys_update1 = string.Empty;
                string sql_sys_id_insert = string.Format(
                    @" INSERT INTO sys_bill_max_separate(within_code,bill_code,bill_id,year_month,bill_text2,bill_text1,bill_text3,bill_text4,bill_text5) 
                    VALUES('0000','{0}','{1}','{2}','','','','','')",
                    bill_code, "ST03", year_month);
                string sql_sys_id_udate = string.Format(
                    @" UPDATE sys_bill_max_separate SET bill_code='{0}' WHERE within_code='0000' AND bill_id='{1}' AND year_month='{2}'",
                    bill_code, "ST03", year_month);
                if (bill_code.Substring(5, 4) != "0001")
                    sql_sys_update1 = sql_sys_id_udate;
                else
                    sql_sys_update1 = sql_sys_id_insert;
                str = sql_sys_update1;
                sbSql.Append(str);
                //***end save form max id to system tabe 

                //插入主表               
                str = string.Format(
                  @" Insert Into st_inventory_mostly(
                  within_code,id,inventory_date,origin,state,bill_type_no,department_id,linkman,handler,
                  remark,create_by,create_date,update_count,transfers_state,servername,turn_type) 
                  Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',getdate(),'1','0','{11}','A' )",
                  within_code, headData.id, headData.inventory_date, headData.origin, headData.state, headData.bill_type_no,headData.department_id,
                  headData.linkman,headData.handler, headData.remark, headData.create_by, ls_servername);
                sbSql.Append(str);

                //插入明細表一
                foreach (var item in lstDetailData1)
                {
                    str = string.Format(
                    @" Insert Into st_i_subordination(
                    within_code,id,sequence_id, goods_id, base_unit, unit,i_amount,inventory_issuance, ii_code, ii_lot_no,
                    inventory_receipt, ir_code, ir_lot_no, remark, i_weight, ref_id, ref_sequence_id, mo_id, mrp_id,obligate_mo_id,jo_sequence_id,
                    rate,ib_qty,state,transfers_state,ib_weight,only_detail,vendor_id) Values                    
                    ('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}','{8}','{9}','{10}','{11}','{12}','{13}',{14},'{15}','{16}','{17}','{18}','{19}','{20}',
                    1,0,'0','0',0,'0','P')",
                    within_code,headData.id,item.sequence_id,item.goods_id,item.base_unit,item.unit,item.i_amount,item.inventory_issuance, 
                    item.ii_code,item.ii_lot_no,item.inventory_receipt,item.ir_code,item.ir_lot_no,item.remark,item.i_weight,item.ref_id,
                    item.ref_sequence_id,item.mo_id,item.mrp_id,item.obligate_mo_id,item.jo_sequence_id );
                    sbSql.Append(str);
                }
                //插入明細表二
                foreach (var item in lstDetailData2)
                {
                    str = string.Format(
                    @" Insert Into st_cc_details_schedule(
                    within_code,id,upper_sequence,sequence_id,goods_id,i_amount,i_weight,order_qty,inventory_issuance,ii_code,inventory_receipt,ir_code,
                    ir_lot_no,remark,so_no,so_sequence_id,ref_id,ref_sequence_id,mo_id,obligate_mo_id,unit, state,transfers_state,only_detail) Values 
                    ('{0}','{1}','{2}','{3}','{4}',{5},{6},{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','0','0',0)",
                    within_code,headData.id,item.upper_sequence,item.sequence_id,item.goods_id,item.i_amount,item.i_weight,item.order_qty,
                    item.inventory_issuance, item.ii_code, item.inventory_receipt,item.ir_code,item.ir_lot_no,item.remark,item.so_no,item.so_sequence_id,
                    item.ref_id, item.ref_sequence_id,item.mo_id,item.obligate_mo_id,item.unit);
                    sbSql.Append(str);
                }
                //生成移交單
                //GenNumberDAL objNum = new GenNumberDAL();
                //index = 0;
                //index += 1;
                //sequence_id = objNum.GetSequenceID(index);// index.ToString().PadLeft(4, '0')+"h"; //移交單的序號
            }
            else
            {
                //首先處理刪除(表格一) 
                //注意加選項with(ROWLOCK),只有用到主鍵時才會用到行級鎖
                if (lstDelData1 != null)
                {                    
                    foreach (var item in lstDelData1)
                    {
                        str = string.Format(
                        @" DELETE FROM st_i_subordination With(ROWLOCK) Where id='{0}' And sequence_id='{1}' And within_code='{2}'",
                        item.id, item.sequence_id, within_code);
                        sbSql.Append(str);
                        
                    }
                }
                //首先處理刪除(表格二)
                if (lstDelData2 != null)
                {
                    foreach (var item in lstDelData2)
                    {
                        str = string.Format(
                        @" DELETE FROM st_cc_details_schedule with(ROWLOCK) 
                        WHERE within_code='{0}' And id='{1}' And upper_sequence='{2}' And sequence_id='{3}'",
                        within_code,item.id,item.upper_sequence,item.sequence_id);
                        sbSql.Append(str);
                    }
                }

                str = string.Format(
                @" Update st_inventory_mostly WITH(ROWLOCK)
                Set inventory_date='{2}',origin='{3}',state='{4}',bill_type_no='{5}',department_id='{6}',linkman='{7}',handler='{8}',
                remark='{9}',update_by='{10}',update_date=getdate()
                WHERE id='{0}' And within_code='{1}'",
                headData.id, within_code,headData.inventory_date,headData.origin,headData.state,headData.bill_type_no,headData.department_id,
                headData.linkman, headData.handler, headData.remark, headData.update_by);
                sbSql.Append(str);
                //插入明細表一                
                foreach (var item in lstDetailData1)
                {
                    if (!string.IsNullOrEmpty(item.row_status))
                    {
                        if (item.row_status == "NEW")
                        {
                            str = string.Format(
                            @" Insert Into st_i_subordination(
                            within_code,id,sequence_id, goods_id, base_unit, unit,i_amount,inventory_issuance, ii_code, ii_lot_no,
                            inventory_receipt, ir_code, ir_lot_no, remark, i_weight, ref_id, ref_sequence_id, mo_id, mrp_id,obligate_mo_id,jo_sequence_id,
                            rate,ib_qty,state,transfers_state,ib_weight,only_detail,vendor_id) Values                    
                            ('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}','{8}','{9}','{10}','{11}','{12}','{13}',{14},'{15}','{16}','{17}','{18}','{19}','{20}',
                            1,0,'0','0',0,'0','P')",
                            within_code, headData.id, item.sequence_id, item.goods_id, item.base_unit, item.unit, item.i_amount, item.inventory_issuance,
                            item.ii_code, item.ii_lot_no, item.inventory_receipt, item.ir_code, item.ir_lot_no, item.remark, item.i_weight, item.ref_id,
                            item.ref_sequence_id, item.mo_id, item.mrp_id, item.obligate_mo_id, item.jo_sequence_id);
                        }
                        if (item.row_status == "EDIT")
                        {
                            str = string.Format(
                            @" UPDATE st_i_subordination WITH(ROWLOCK)
                            SET goods_id='{3}',base_unit='{4}',unit='{5}',i_amount={6},inventory_issuance='{7}',ii_code='{8}',ii_lot_no='{9}',
                            inventory_receipt='{10}',ir_code='{11}',ir_lot_no='{12}',remark='{13}',i_weight={14},ref_id='{15}',ref_sequence_id='{16}',
                            mo_id='{17}', mrp_id='{18}',obligate_mo_id='{19}',jo_sequence_id='{20}'
                            WHERE id='{0}' And sequence_id='{1}' And within_code='{2}'",
                            headData.id, item.sequence_id, within_code,item.goods_id, item.base_unit, item.unit, item.i_amount, item.inventory_issuance,
                            item.ii_code, item.ii_lot_no, item.inventory_receipt, item.ir_code, item.ir_lot_no, item.remark, item.i_weight, item.ref_id,
                            item.ref_sequence_id, item.mo_id, item.mrp_id, item.obligate_mo_id, item.jo_sequence_id);
                        }
                        sbSql.Append(str);
                    }
                }
                //插入明細表二             
                foreach (var item in lstDetailData2)
                {
                    if (!string.IsNullOrEmpty(item.row_status))
                    {
                        if (item.row_status == "NEW")
                        {
                            str = string.Format(
                            @" Insert Into st_cc_details_schedule(
                            within_code,id,upper_sequence,sequence_id,goods_id,i_amount,i_weight,order_qty,inventory_issuance,ii_code,inventory_receipt,ir_code,
                            ir_lot_no,remark,so_no,so_sequence_id,ref_id,ref_sequence_id,mo_id,obligate_mo_id,unit, state,transfers_state,only_detail) Values 
                            ('{0}','{1}','{2}','{3}','{4}',{5},{6},{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','0','0',0)",
                            within_code, headData.id, item.upper_sequence, item.sequence_id, item.goods_id, item.i_amount, item.i_weight, item.order_qty,
                            item.inventory_issuance, item.ii_code, item.inventory_receipt, item.ir_code, item.ir_lot_no, item.remark, item.so_no, item.so_sequence_id,
                            item.ref_id, item.ref_sequence_id, item.mo_id, item.obligate_mo_id, item.unit);
                        }
                        if (item.row_status == "EDIT")
                        {
                            str = string.Format(
                            @" UPDATE st_cc_details_schedule With(ROWLOCK)
                            SET goods_id='{4}',i_amount={5},i_weight={6},order_qty={7},inventory_issuance='{8}',ii_code='{9}',
                            inventory_receipt='{10}',ir_code='{11}',ir_lot_no='{12}',remark='{13}',so_no='{14}',so_sequence_id='{15}',
                            ref_id='{16}',ref_sequence_id='{17}',mo_id='{18}',obligate_mo_id='{19}',unit='{20}' 
                            WHERE within_code='{0}' And id='{1}' And upper_sequence='{2}' And sequence_id='{3}'",
                            within_code,headData.id,item.upper_sequence,item.sequence_id,item.goods_id,item.i_amount,item.i_weight,item.order_qty,
                            item.inventory_issuance, item.ii_code, item.inventory_receipt, item.ir_code, item.ir_lot_no, item.remark, item.so_no,
                            item.so_sequence_id, item.ref_id, item.ref_sequence_id, item.mo_id, item.obligate_mo_id, item.unit);
                        }
                        sbSql.Append(str);
                    }
                }


            }
            sbSql.Append(@" COMMIT TRANSACTION ");
            result = sh.ExecuteSqlUpdate(sbSql.ToString());
            sbSql.Clear();
            result = (result == "") ? "00" : "-1";
            
            return result;
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
     //           result = ApproveRechange(head, head.handover_id, ldt_check_date, user_id);

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

        public static string ApproveChangeStore(st_inventory_mostly head, string user_id, string approve_type)
        {
            string result = "", is_active_name, is_turn_type = "A";//区分类型,A:倉庫發料;
            string ls_id, ls_origin, ls_mrp_id, ls_sequence_id, ls_unit_code, ls_servername, ls_error;
            string ls_charge_dept, ls_goods_id, ls_dept_id, ls_wp_id, ls_next_wp_id, ls_mo_id, ls_obligate_mo_id;
            int li_jo_ver;
            int ll_count, ll_count2, ll_cnt, ll_cnt_state1, ll_cnt_state3, ll_cnt_state4, ll_rtn;
            decimal ldec_qty, ldec_i_amount, ldec_i_weight;
            string ldt_date, ldt_check_date;
            ldt_check_date = CommonDAL.GetDbDateTime("L");//批準日期(長日期時間);
            is_active_name = approve_type == "1" ? "pfc_ok" : "pfc_unok";
            if (is_active_name == "pfc_ok" || is_active_name == "pfc_unok")
            {
                ls_id = head.id;
                ls_origin = head.origin;
                ldt_date = head.inventory_date;
                ls_dept_id = head.department_id;
                ls_servername = head.servername;                
                if (is_active_name == "pfc_unok")
                {
                    ldt_check_date = head.check_date;
                }
                string strSql = string.Format(
                @"Select sequence_id,unit,Isnull(i_amount,0) as i_amount,Isnull(i_weight,0) as i_weight,inventory_issuance,
                inventory_receipt,mo_id,goods_id,obligate_mo_id
				From st_i_subordination WITH(NOLOCK) Where id ='{0}' And within_code ='{1}'", head.id, within_code);
                DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
                DataTable dtFind = new DataTable();
                string sql_update = string.Empty;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ls_sequence_id = dt.Rows[i]["sequence_id"].ToString();
                    if (is_turn_type == "A")
                    {
                        //ls_sequence_id = dt.Rows[i]["sequence_id"].ToString();
                        ls_unit_code = dt.Rows[i]["unit"].ToString();
                        ldec_i_amount = decimal.Parse(dt.Rows[i]["i_amount"].ToString());
                        ldec_i_weight = decimal.Parse(dt.Rows[i]["i_weight"].ToString());
                        ls_wp_id = dt.Rows[i]["inventory_issuance"].ToString();
                        ls_next_wp_id = dt.Rows[i]["inventory_receipt"].ToString();
                        ls_mo_id = dt.Rows[i]["mo_id"].ToString();
                        ls_goods_id = dt.Rows[i]["goods_id"].ToString();
                        ls_obligate_mo_id = dt.Rows[i]["obligate_mo_id"].ToString();
                        strSql = string.Format(
                        @"Select count(1) as cnt                
                        From jo_bill_mostly A WITH(NOLOCK),jo_bill_goods_details B WITH(NOLOCK)
                        Where A.within_code = B.within_code And A.id = B.id And A.ver = B.ver
                          And A.state Not In('2','V') And A.within_code ='{0}' And A.mo_id ='{1}'
                          And B.goods_id ='{2}' And B.next_wp_id ='{3}' And (B.wp_id ='{4}' Or Left('{5}', 1)='R')",
                        within_code, ls_mo_id, ls_goods_id, ls_next_wp_id, ls_wp_id, ls_obligate_mo_id);
                        dtFind = sh.ExecuteSqlReturnDataTable(strSql);
                        ll_count = int.Parse(dtFind.Rows[0]["cnt"].ToString());
                        if (ll_count > 0)
                        {
                            sql_update = string.Format(
                            @" Update B WITH(ROWLOCK)
					        Set	B.c_qty=Isnull(B.c_qty,0) +
                            Case When '{6}'='pfc_ok' 
                                 Then dbo.FN_CHANGE_UNITS(B.within_code,B.goods_id,'{7}',B.goods_unit,Isnull({8},0)) 
                                 Else -dbo.FN_CHANGE_UNITS(B.within_code,B.goods_id,'{7}',B.goods_unit,Isnull({8},0)) 
                            End,
						    B.c_qty_ok = Isnull(B.c_qty_ok,0) +  
                            Case When '{6}'='pfc_ok' 
                                 Then dbo.FN_CHANGE_UNITS(B.within_code,B.goods_id,'{7}',B.goods_unit,Isnull({8},0)) 
                                 Else -dbo.FN_CHANGE_UNITS(B.within_code,B.goods_id,'{7}',B.goods_unit,Isnull({8},0)) 
                            End,
						    B.c_sec_qty = Isnull(B.c_sec_qty,0) + Case When '{6}'='pfc_ok' Then Isnull({9},0) Else -Isnull({9},0) End,
						    B.c_sec_qty_ok = Isnull(B.c_sec_qty_ok,0) + Case When '{6}'='pfc_ok' Then Isnull({9},0) Else -Isnull({9},0) End
					        From jo_bill_mostly A,jo_bill_goods_details B
					        Where A.within_code = B.within_code And A.id = B.id And A.ver = B.ver
						    And A.state Not In('2','V') And A.within_code ='{0}' And A.mo_id ='{1}'
						    And B.goods_id = '{2}' And B.next_wp_id ='{3}' And (b.wp_id ='{4}' Or Left('{5}',1)='R')",
                            within_code, ls_mo_id, ls_goods_id, ls_next_wp_id, ls_wp_id, ls_obligate_mo_id, is_active_name, ls_unit_code, ldec_i_amount, ldec_i_weight);
                            sql_update = RetunSqlString(sql_update);
                            result = sh.ExecuteSqlUpdate(sql_update);
                            if (result == "")
                            {
                                result = "00";
                            }
                            else
                            {
                                //保存失敗!
                                result += "-1数据保存失败" + is_active_name + result;
                                break;
                            }

                            //--
                            sql_update = string.Format(
                            @" Update B WITH(ROWLOCK)
					        Set B.goods_state=(Case When (Isnull(B.prod_qty,0)-Isnull(B.c_qty,0)<= 0) Then '4' Else (Case When Isnull(B.c_qty,0)>0 Then '3' Else '1' End) End)
					        From jo_bill_mostly A,jo_bill_goods_details B
					        Where A.within_code = B.within_code And A.id = B.id And A.ver = B.ver
						    And A.state Not In('2','V') And A.within_code='{0}' And A.mo_id='{1}'
						    And B.goods_id='{2}' And B.next_wp_id ='{3}' And (b.wp_id ='{4}' Or Left('{5}',1)='R')",
                            within_code, ls_mo_id, ls_goods_id, ls_next_wp_id, ls_wp_id, ls_obligate_mo_id);
                            sql_update = RetunSqlString(sql_update);
                            result = sh.ExecuteSqlUpdate(sql_update);
                            if (result == "")
                            {
                                result = "00";
                            }
                            else
                            {
                                //保存失敗!
                                result += "-1数据保存失败" + is_active_name + result;
                                break;
                            }
                            //--
                            ll_count = 0;
                            strSql = string.Format(
                            @"Select count(1) as cnt					
					        From jo_bill_mostly M WITH (NOLOCK) 
                            Inner Join jo_bill_goods_details D WITH (NOLOCK) On M.within_code=D.within_code And M.id=D.id And M.ver=D.ver
						    Inner Join jo_bill_materiel_details DD WITH (NOLOCK)
                                On D.within_code=DD.within_code And D.id=DD.id And D.ver=DD.ver And D.sequence_id=DD.upper_sequence
					        Where M.state Not In('2','V') And M.within_code='{0}' And M.mo_id='{1}' And D.wp_id='{2}' And DD.materiel_id='{3}'",
                            within_code, ls_mo_id, ls_next_wp_id, ls_goods_id);
                            dtFind = sh.ExecuteSqlReturnDataTable(strSql);
                            ll_count = int.Parse(dtFind.Rows[0]["cnt"].ToString());
                            if (ll_count > 0)
                            {
                                sql_update = string.Format(
                                @" Update D WITH(ROWLOCK) 
                                Set predept_rechange_qty=Isnull(D.predept_rechange_qty,0) + Case When '{4}'='pfc_ok' Then IsNull({5},0) else - IsNull({5},0) End,
							        predept_rechange_sec_qty=Isnull(D.predept_rechange_sec_qty,0) + Case When '{4}'='pfc_ok' Then IsNull({6},0) else -IsNull({6},0) End
						        From jo_bill_mostly M 
                                Inner Join jo_bill_goods_details D On M.within_code = D.within_code And M.id = D.id And M.ver = D.ver
						        Inner Join jo_bill_materiel_details DD On D.within_code=DD.within_code And D.id=DD.id And D.ver=DD.ver And D.sequence_id=DD.upper_sequence
						        Where M.within_code='{0}' And M.mo_id='{1}' And M.state Not In('2','V') And D.wp_id='{2}' And DD.materiel_id='{3}'",
                                within_code, ls_mo_id, ls_next_wp_id, ls_goods_id, is_active_name, ldec_i_amount, ldec_i_weight);
                                sql_update = RetunSqlString(sql_update);
                                result = sh.ExecuteSqlUpdate(sql_update);
                                if (result == "")
                                {
                                    result = "00";
                                }
                                else
                                {
                                    //保存失敗!
                                    result += "-1数据保存失败" + is_active_name + result;
                                    break;
                                }
                            }

                            //--start 20130813 huangyun 从触发器中移过来
                            ll_cnt = 0;
                            ll_cnt_state1 = 0;
                            ll_cnt_state3 = 0;
                            ll_cnt_state4 = 0;
                            //--
                            strSql = string.Format(
                            @"Select Sum(1) as cnt,
							Sum(Case When Isnull(B.goods_state,'0') = '1' Then 1 Else 0 End) as cnt_state1,
							Sum(Case When Isnull(B.goods_state,'0') = '3' Then 1 Else 0 End) as cnt_state3,
							Sum(Case When Isnull(B.goods_state,'0') = '4' Then 1 Else 0 End) as cnt_state4                    
                            From jo_bill_mostly A WITH (NOLOCK),jo_bill_goods_details B WITH (NOLOCK)
					        Where A.within_code = B.within_code And A.id = B.id And A.ver = B.ver And
						      A.within_code ='{0}' And A.mo_id ='{1}' And A.state Not In('2','V')", within_code, ls_mo_id);
                            dtFind = sh.ExecuteSqlReturnDataTable(strSql);
                            ll_cnt = int.Parse(dtFind.Rows[0]["cnt"].ToString());
                            ll_cnt_state1 = int.Parse(dtFind.Rows[0]["cnt_state1"].ToString());
                            ll_cnt_state3 = int.Parse(dtFind.Rows[0]["cnt_state3"].ToString());
                            ll_cnt_state4 = int.Parse(dtFind.Rows[0]["cnt_state4"].ToString());
                            if (ll_cnt > 0)//如果明细有生产中的数据,主表的状态也要为生产中
                            {
                                sql_update = string.Format(
                                @" Update M WITH(ROWLOCK) 
                                Set M.state =(Case When({2} > 0) Then '3' When({3}={4}) Then '4' When({3}={4}) Then '1' Else M.state End)
                                From jo_bill_mostly M
                                Where M.within_code='{0}' And M.mo_id='{1}' And M.state Not In('2','3','V')",
                                within_code, ls_mo_id, ll_cnt_state3, ll_cnt, ll_cnt_state4, ll_cnt_state1);
                                sql_update = RetunSqlString(sql_update);
                                result = sh.ExecuteSqlUpdate(sql_update);
                                if (result == "")
                                {
                                    result = "00";
                                }
                                else
                                {
                                    result += "-1数据保存失败" + is_active_name + result;
                                    break;
                                }
                            }
                        }// end of if (ll_count > 0)
                    } //-- end of if(is_turn_type == "A")
                    //更新交易表
                    ll_count = 0;
                    strSql = string.Format(
                    @"Select Count(1) as cnt            
                    From st_cc_details_schedule WITH(NOLOCK)
                    where within_code ='{0}' and id ='{1}' and upper_sequence ='{2}'", within_code, ls_id, ls_sequence_id);
                    dtFind = sh.ExecuteSqlReturnDataTable(strSql);
                    ll_count = int.Parse(dtFind.Rows[0]["cnt"].ToString());
                    //**star ll_count>0
                    if (ll_count > 0)
                    {
                        if (is_active_name == "pfc_ok")
                        {
                            ll_count = 0;
                            strSql = string.Format(
                            @"Select count(1) as cnt                        
                            From st_business_record  WITH(NOLOCK)
                            Where within_code='{0}' And id='{1}' And sequence_id='{2}' And action_id='28'", within_code, ls_id, ls_sequence_id);
                            dtFind = sh.ExecuteSqlReturnDataTable(strSql);
                            ll_count = int.Parse(dtFind.Rows[0]["cnt"].ToString());
                            if (ll_count == 0)
                            {
                                sql_update = string.Format(
                                @" Insert Into st_business_record(within_code,id,goods_id,goods_name,unit,base_unit,rate,action_time,action_id,ii_qty,
								    ii_location_id,ii_code,operator,check_date,sequence_id,ib_qty,qty,sec_unit,sec_qty,
								    lot_no,
								    mo_id,dept_id,servername)
					            Select	a.within_code,a.id,a.goods_id,a.goods_name,a.unit,a.base_unit,a.rate,'{3}','28',
								        a.i_amount ,
								        a.inventory_issuance,a.ii_code,'{4}','{5}',
								        a.sequence_id,
								        dbo.FN_CHANGE_UNITBYCV(a.within_code,a.goods_id,a.unit,1,'','','*'),
								        round(dbo.FN_CHANGE_UNITBYCV(a.within_code,a.goods_id,a.unit,a.i_amount,'','','/'),4),
								        (Select gross_unit From it_price_storage Where it_price_storage.within_code=a.within_code And it_price_storage.id=a.goods_id),
								        a.i_weight,
								        Isnull(a.ii_lot_no,a.ir_lot_no),--//库存出批号
								        a.obligate_mo_id,
								        '{6}','{7}'
					            From st_i_subordination A
					            Where A.within_code='{0}' And A.id='{1}' And A.sequence_id='{2}'",
                                within_code, ls_id, ls_sequence_id, ldt_date, user_id, ldt_check_date, ldt_date, ls_servername);
                                sql_update = RetunSqlString(sql_update);
                                result = sh.ExecuteSqlUpdate(sql_update);
                                if (result == "")
                                {
                                    result = "00";
                                }
                                else
                                {
                                    result = "-1数据保存失败<" + is_active_name + ">(" + "st_business_record)[28]" + result;
                                    break;
                                }
                                //--更新库存
                                result = pubFunction.of_update_st_details("I", "28", ls_id, ls_sequence_id, ldt_check_date, "");
                                if (result.Substring(0, 2) == "-1")
                                {
                                    result = "-1数据保存失败<" + is_active_name + ">(" + "st_details)" + result;
                                    break;
                                }
                            }
                            //--start 20131022 huangyun 因为'轉貨品編號'功能是同一个部门操作,所以不需要单独去进行签收操作

                        }// --end if is_active_name = 'pfc_ok' Then
                         //反批准时操作
                        if (is_active_name == "pfc_unok")
                        {
                            ll_count = 0;
                            strSql = string.Format(
                            @"Select count(1) as cnt
                            From st_business_record WITH(NOLOCK)
                            Where within_code='{0}' And id='{1}' And sequence_id='{2}' And action_id='28'",
                            within_code, ls_id, ls_sequence_id);
                            dtFind = sh.ExecuteSqlReturnDataTable(strSql);
                            ll_count = int.Parse(dtFind.Rows[0]["cnt"].ToString());
                            if (ll_count > 0)
                            {
                                //先更新库存,再删除交易数据
                                result = pubFunction.of_update_st_details("D", "28", ls_id, ls_sequence_id, ldt_check_date, "");
                                if (result.Substring(0, 2) == "-1")
                                {
                                    result = "-1数据保存失败<" + is_active_name + ">(" + "st_details)" + result;
                                    break;
                                }
                                sql_update = string.Format(
                                @" Delete From st_business_record Where within_code='{0}' And id='{1}' And sequence_id='{2}' And action_id='28'",
                                within_code, ls_id, ls_sequence_id);
                                sql_update = RetunSqlString(sql_update);
                                result = sh.ExecuteSqlUpdate(sql_update);
                                if (result == "")
                                {
                                    result = "00";
                                }
                                else
                                {
                                    result = "-1数据保存失败<" + is_active_name + ">(" + "st_business_record)" + result;
                                    break;
                                }
                            }
                        }
                    }//-- end of ll_count > 0
                }//--end for

                if (result.Substring(0, 2) == "-1")
                {
                    return result;
                }

                //物料更新
                strSql = string.Format(
                @"Select a.sequence_id,Isnull(a.i_amount,0) as i_amount,Isnull(a.i_weight,0) as i_weight,a.mrp_id,
                    (Case When c.kind='03' Then a.i_weight Else a.i_amount End) as qty
			    From st_i_subordination a0,st_cc_details_schedule a, it_goods b,cd_units c
                Where a0.within_code = a.within_code and a0.id = a.id and
                    a0.sequence_id = a.upper_sequence and a.within_code = b.within_code And a.goods_id = b.id And
                    b.within_code = c.within_code And b.unit_code = c.id And
                    a.within_code='{0}' And a.id='{1}'", within_code, ls_id);
                DataTable dtFind2= sh.ExecuteSqlReturnDataTable(strSql);               
                for (int i = 0; i < dtFind2.Rows.Count; i++)
                {
                    ls_sequence_id = dtFind2.Rows[i]["sequence_id"].ToString();
                    ldec_i_amount = decimal.Parse(dtFind2.Rows[i]["i_amount"].ToString());
                    ldec_i_weight = decimal.Parse(dtFind2.Rows[i]["i_weight"].ToString());
                    ls_mrp_id = dtFind2.Rows[i]["mrp_id"].ToString();
                    ldec_qty = decimal.Parse(dtFind2.Rows[i]["qty"].ToString());
                    if (is_turn_type== "A")
                    {
                        strSql = string.Format(
                        @"Select Count(1) as cnt 
                        From jo_bill_mostly a WITH(NOLOCK),
                             jo_bill_materiel_details b WITH(NOLOCK),
                             st_cc_details_schedule c WITH(NOLOCK)
                        Where a.within_code=b.within_code and a.id=b.id and a.ver=b.ver and a.state not in('2','V') and
                            b.within_code=c.within_code and b.id=c.ref_id and b.sequence_id=c.ref_sequence_id and IsNull(c.state,'0')<>'2' and
                            c.within_code='{0}' and c.id ='{1}' And c.sequence_id='{2}'",
                        within_code,ls_id, ls_sequence_id);
                        dtFind = sh.ExecuteSqlReturnDataTable(strSql);
                        ll_count = int.Parse(dtFind.Rows[0]["cnt"].ToString());
                        if (ll_count > 0)
                        {
                            sql_update = string.Format(
                            @" Update jo_bill_materiel_details WITH(ROWLOCK)
				            Set i_qty = IsNull(b.i_qty,0) + Case When '{3}'='pfc_ok' Then Isnull({4},0) Else -Isnull({4},0) end,
						        i_sec_qty = IsNull(b.i_sec_qty,0) +  Case When '{3}'='pfc_ok' Then Isnull({5},0) Else -Isnull({5},0) End
				            From jo_bill_mostly a,jo_bill_materiel_details b,st_cc_details_schedule c
				            Where a.within_code=b.within_code and a.id=b.id and a.ver=b.ver and a.state not in('2','V') and
						        b.within_code=c.within_code and b.id=c.ref_id and b.sequence_id=c.ref_sequence_id and IsNull(c.state,'0')<>'2' and
						        c.within_code='{0}' and c.id='{1}' And c.sequence_id='{2}'",
                            within_code,ls_id,ls_sequence_id, is_active_name, ldec_i_amount, ldec_i_weight);
                            sql_update = RetunSqlString(sql_update);
                            result = sh.ExecuteSqlUpdate(sql_update);
                            if (result == "")
                            {
                                result = "00";
                            }
                            else
                            {
                                result = "-1数据保存失败" + result;
                                break;
                            }
                            ll_count = 0;
                            //更新库存预留表
                            strSql = string.Format(@"Select Count(1) as cnt From mrp_st_details_lot WITH (NOLOCK) Where within_code='{0}' And id='{1}'",within_code,ls_mrp_id);
                            dtFind = sh.ExecuteSqlReturnDataTable(strSql);
                            ll_count = int.Parse(dtFind.Rows[0]["cnt"].ToString());
                            if (ll_count > 0)
                            {
                                sql_update = string.Format(
                                @" Update mrp_st_details_lot WITH(ROWLOCK) 
                                  Set issue_qty=IsNull(issue_qty, 0) + Case When '{2}'='pfc_ok' Then IsNull({3},0) Else -IsNull({3},0) End
                                  Where within_code='{0}' And id ='{1}'",
                                within_code, ls_mrp_id,is_active_name, ldec_qty);
                            }
                            sql_update = RetunSqlString(sql_update);
                            result = sh.ExecuteSqlUpdate(sql_update);
                            if (result == "")
                            {
                                result = "00";
                            }
                            else
                            {
                                result = "-1数据保存失败" + result;
                                break;
                            }                            
                        }
                    } //--enf If is_turn_type = 'A' Then
                    //更新交易表
                    strSql = string.Format(
                    @"Select count(1) as cnt
                    From st_business_record WITH(NOLOCK)
                    Where within_code='{0}' And id ='{1}' And sequence_id ='{2}' And action_id='28'", within_code, ls_id,ls_sequence_id);
                    dtFind = sh.ExecuteSqlReturnDataTable(strSql);
                    ll_count2 = int.Parse(dtFind.Rows[0]["cnt"].ToString());
                    if(is_active_name == "pfc_ok")//转出仓20131028 huangyun出库对应'库存页数'
                    {
                        if (ll_count2 == 0)
                        {
                            sql_update = string.Format(
                            @" Insert Into st_business_record(within_code,id,goods_id,goods_name,unit,base_unit,rate,action_time,action_id,ii_qty,
						            ii_location_id,ii_code,operator,check_date,sequence_id,lot_no,
						            ib_qty,qty,sec_unit,sec_qty,mo_id,dept_id,servername)
				            Select A.within_code,A.id,A.goods_id,A.goods_name,A.unit,A.base_unit,A.rate,'{3}','28',A.i_amount,
						            A.inventory_issuance,A.ii_code,'{4}','{5}',A.sequence_id,
						            IsNull(A.ref_lot_no,A.ir_lot_no),
						            dbo.FN_CHANGE_UNITBYCV(A.within_code,A.goods_id,A.unit,1,'','','*'),
						            round(dbo.FN_CHANGE_UNITBYCV(A.within_code,A.goods_id,A.unit,A.i_amount,'','','/'),4),
						            (Select gross_unit From it_price_storage Where it_price_storage.within_code=A.within_code And it_price_storage.id=A.goods_id),
						            A.i_weight,A.obligate_mo_id,'{6}','{7}'
				            From st_cc_details_schedule A
				            Where A.within_code='{0}' And A.id='{1}' And A.sequence_id='{2}'",
                            within_code, ls_id, ls_sequence_id, ldt_date,user_id, ldt_check_date, ls_dept_id, ls_servername);
                            sql_update = RetunSqlString(sql_update);
                            result = sh.ExecuteSqlUpdate(sql_update);
                            if (result == "")
                            {
                                result = "00";
                            }
                            else
                            {
                                result = "-1数据保存失败<" + is_active_name + ">(st_business_record)[28]" + result;
                                break;
                            }
                            //--更新库存
                            result = pubFunction.of_update_st_details("I", "28", ls_id, ls_sequence_id, ldt_check_date, "");//= -1 Then                           
                            if (result.Substring(0, 2) == "-1")
                            {
                                result = "-1数据保存失败<" + is_active_name + ">(st_details)" + result;
                                break;
                            }
                        }
                    }// -- end of If(is_active_name == 'pfc_ok')
                    
                    //反批准时处理
		            if(is_active_name == "pfc_unok")
                    {
			            if(ll_count2 > 0)
                        {
				            //先更新库存,再删除交易数据
				            result = pubFunction.of_update_st_details("D","28",ls_id,ls_sequence_id,ldt_check_date,"");
                            if (result.Substring(0, 2) == "-1")
                            {
                                result = "-1数据保存失败<" + is_active_name + ">(st_details)" + result;
                                break;
                            }				            
				            //--
                            sql_update=string.Format(
				            @" Delete From st_business_record 
				            Where within_code='{0}' And id='{1}' And sequence_id='{2}' And action_id='28'",within_code,ls_id,ls_sequence_id);
                            sql_update = RetunSqlString(sql_update);
                            result = sh.ExecuteSqlUpdate(sql_update);
                            if (result == "")
                            {
                                result = "00";
                            }
                            else
                            {
                                result = "-1数据保存失败<" + is_active_name + ">(st_business_record)" + result;
                                break;
                            }
			            }
		            }// -- end of If is_active_name=='pfc_unok'
                } // --end for loop

            } //--end of if (is_active_name == "pfc_ok" || is_active_name == "pfc_unok")

            //批準/反批準成功時需手動更新主表的狀態
            if (result.Substring(0, 2) == "00")
            {
                bool is_pfc_ok = (is_active_name == "pfc_ok") ? true : false;           
                string checkDate = is_pfc_ok ? ldt_check_date : "";
                string checkBy = is_pfc_ok ? user_id : "";
                string state = is_pfc_ok ? "1" : "0";
                string strSql = string.Format(
                @"Update st_inventory_mostly with(Rowlock) 
                   SET check_date=(Case When '{2}'<>'' Then '{2}' Else null End), check_by='{3}',
                       update_date=(Case When '{2}'<>'' Then '{2}' Else getdate() End),update_by='{4}',state='{5}' 
                   WHERE within_code='{0}' And id='{1}'", within_code, head.id, checkDate, checkBy, user_id, state);
                result = sh.ExecuteSqlUpdate(strSql);
                result = (result == "") ? "00" : "-1" + result;
            }

            return result;
        }
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

        //檢查庫存,返回庫存差異數
        public static List<check_part_stock> CheckStorage(string id)
        {
            string strSql = string.Format(
                @"SELECT TOP 1 S.out_dept,S.sequence_id,S.mo_id,S.goods_id,S.lot_no,(ISNULL(C.qty,0)-S.con_qty) AS qty,(ISNULL(C.sec_qty,0)-S.sec_qty) AS sec_qty 
                FROM (SELECT A.within_code,A1.inventory_issuance As out_dept,A1.sequence_id,B.obligate_mo_id As mo_id,B.goods_id,B.ir_lot_no AS lot_no,
                      B.i_amount As con_qty,B.i_weight as sec_qty
	                  FROM st_inventory_mostly A With(nolock) 
                      Inner Join st_i_subordination A1 With(nolock) On A.within_code=A1.within_code and A.id=A1.id
                      Inner Join st_cc_details_schedule B With(nolock) On A1.within_code=B.within_code AND A1.id=B.id And A1.sequence_id=B.upper_sequence
	                  WHERE A.id='{0}' AND A.within_code='{1}' AND A.state<>'2'
	                 ) S  
	                LEFT JOIN st_details_lot C with(nolock)
		                ON S.within_code=C.within_code AND S.out_dept=C.location_id AND S.out_dept=C.carton_code AND
		                   S.goods_id=C.goods_id AND S.lot_no=C.lot_no AND S.mo_id=C.mo_id AND ISNULL(C.sec_qty,0)>0
                WHERE S.con_qty>ISNULL(C.qty,0) OR S.sec_qty>ISNULL(C.sec_qty,0)", id, within_code);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            List<check_part_stock> lstDetail = new List<check_part_stock>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                check_part_stock lst = new check_part_stock();
                lst.out_dept = dt.Rows[i]["out_dept"].ToString();               
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
        public static string DeleteHead(st_inventory_mostly head)
        {
            string strSql = "";
            string result = "";
            strSql += string.Format(@" SET XACT_ABORT ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");
            strSql += string.Format(@" UPDATE st_inventory_mostly SET state='2',update_by='{0}',update_date=getdate() WHERE id='{1}' AND within_code='0000'", head.update_by, head.id);
            strSql += string.Format(@" COMMIT TRANSACTION ");
            result = sh.ExecuteSqlUpdate(strSql);
            return result;//成功返回空格
        }
        //public static List<st_lot_no> GetLotNoList(string location_id, string mo_id, string goods_id)
        //{
        //    string strSql = string.Format(
        //    @"Select lot_no,qty,sec_qty,mo_id 
        //    From st_details_lot 
        //    Where within_code='0000' And location_id='{0}' And carton_code='{1}' And goods_id='{3}' And qty>0 And sec_qty>0
        //    Order By lot_no", location_id, mo_id, goods_id);
        //    DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
        //    List<st_lot_no> lstDetail = new List<st_lot_no>();
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        st_lot_no mdj = new st_lot_no();
        //        mdj.lot_no = dt.Rows[i]["lot_no"].ToString();
        //        mdj.qty = decimal.Parse(dt.Rows[i]["qty"].ToString());
        //        mdj.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
        //        mdj.mo_id = dt.Rows[i]["mo_id"].ToString();
        //        lstDetail.Add(mdj);
        //    }
        //    return lstDetail;
        //}

        public static st_inventory_mostly GetHeadByID(string id)
        {
            /*批準日期需按這種格式轉換,否則反準比較對不上而出錯
            *Convert(nvarchar(19),check_date,121) As check_date
            */
            st_inventory_mostly mdjHead = new st_inventory_mostly();
            string strSql = string.Format(
            @"Select id,Convert(nvarchar(10),inventory_date,121) As inventory_date,origin,bill_type_no,department_id,linkman,
            handler, remark, create_by,create_date, update_by, update_date,check_by,Convert(nvarchar(19),check_date,121) As check_date,update_count,state
            FROM st_inventory_mostly with(nolock) Where id='{0}' And within_code='{1}'", id, within_code);
          
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)            {
                DataRow dr = dt.Rows[0];
                mdjHead.id = dr["id"].ToString();
                mdjHead.inventory_date = dr["inventory_date"].ToString();
                mdjHead.origin = dr["origin"].ToString();
                mdjHead.origin = dr["origin"].ToString();
                mdjHead.bill_type_no = dr["bill_type_no"].ToString();
                mdjHead.department_id = dr["department_id"].ToString();
                mdjHead.linkman = dr["linkman"].ToString();
                mdjHead.handler = dr["handler"].ToString();
                mdjHead.remark = dr["remark"].ToString();
                mdjHead.create_by = dr["create_by"].ToString();
                mdjHead.update_by = dr["update_by"].ToString();
                mdjHead.check_by = dr["check_by"].ToString();
                mdjHead.update_count = dr["update_count"].ToString();
                mdjHead.create_date = dr["create_date"].ToString();
                mdjHead.update_date = dr["update_date"].ToString();
                mdjHead.check_date = dr["check_date"].ToString();
                mdjHead.update_count = dr["update_count"].ToString();
                mdjHead.state = dr["state"].ToString();
            }
            return mdjHead;
        }
        public static List<st_i_subordination> GetDetailsByID(string id)
        {
            string strSql = string.Format(
            @"SELECT A.id,A.sequence_id,A.mo_id,goods_id,B.name as goods_name,A.inventory_issuance,A.ii_code,A.ir_lot_no,A.obligate_mo_id,
            A.i_amount,A.i_weight,A.inventory_receipt,A.ir_code,A.ii_lot_no,A.ref_lot_no,A.ib_qty,A.ib_weight,A.unit,A.remark,A.ref_id,
            A.jo_sequence_id,A.so_no,contract_cid,A.mrp_id,A.sign_by,A.sign_date,A.vendor_id,A.base_unit,A.rate,A.state,C.name as color,
            D.name AS vendor_name
            FROM st_i_subordination A with(nolock)               
            INNER JOIN it_goods B ON A.within_code = B.within_code AND A.goods_id = B.id
            LEFT JOIN cd_color C ON B.within_code = C.within_code AND B.color = C.id
            LEFT JOIN it_vendor D ON A.within_code = D.within_code AND Isnull(A.vendor_id,'')= C.id
            Where A.within_code='{0}' AND A.id='{1}' Order by A.id,A.sequence_id", within_code, id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
               
            List<st_i_subordination> lstDetail = new List<st_i_subordination>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                st_i_subordination mdjDetail = new st_i_subordination();
                mdjDetail.id = dt.Rows[i]["id"].ToString();
                mdjDetail.sequence_id = dt.Rows[i]["sequence_id"].ToString();
                mdjDetail.mo_id = dt.Rows[i]["mo_id"].ToString();
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();
                mdjDetail.inventory_issuance = dt.Rows[i]["inventory_issuance"].ToString();
                mdjDetail.ir_lot_no = dt.Rows[i]["ir_lot_no"].ToString();
                mdjDetail.obligate_mo_id = dt.Rows[i]["obligate_mo_id"].ToString(); 
                mdjDetail.i_amount = decimal.Parse(dt.Rows[i]["i_amount"].ToString());             
                mdjDetail.i_weight = decimal.Parse(dt.Rows[i]["i_weight"].ToString());
                mdjDetail.inventory_receipt = dt.Rows[i]["inventory_receipt"].ToString();
                mdjDetail.ir_code = dt.Rows[i]["ir_code"].ToString();
                mdjDetail.ii_lot_no = dt.Rows[i]["ii_lot_no"].ToString();
                mdjDetail.ref_lot_no = dt.Rows[i]["ref_lot_no"].ToString();
                mdjDetail.color = dt.Rows[i]["color"].ToString();
                mdjDetail.ib_qty = decimal.Parse(dt.Rows[i]["ib_qty"].ToString());
                mdjDetail.ib_weight = decimal.Parse(dt.Rows[i]["ib_weight"].ToString());
                mdjDetail.unit = dt.Rows[i]["unit"].ToString();
                mdjDetail.remark = dt.Rows[i]["remark"].ToString();
                mdjDetail.ref_id = dt.Rows[i]["ref_id"].ToString();
                mdjDetail.so_no = dt.Rows[i]["so_no"].ToString();
                mdjDetail.contract_cid = dt.Rows[i]["contract_cid"].ToString();
                mdjDetail.mrp_id = dt.Rows[i]["mrp_id"].ToString();
                mdjDetail.sign_by = dt.Rows[i]["sign_by"].ToString();
                mdjDetail.sign_date = string.IsNullOrEmpty(dt.Rows[i]["sign_date"].ToString()) ? null : dt.Rows[i]["sign_date"].ToString();
                mdjDetail.vendor_id = dt.Rows[i]["vendor_id"].ToString();
                mdjDetail.vendor_name = dt.Rows[i]["vendor_name"].ToString();
                mdjDetail.row_status = "";
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }

        public static List<st_cc_details_schedule> GetDetailsPartByID(string id)
        {
            string strSql = string.Format(
            @"SELECT A.mo_id,A.goods_id,B.name as goods_name,C.name AS color,A.unit,A.inventory_issuance,A.ii_code,
            A.i_amount,A.i_weight,A.ir_lot_no,A.obligate_mo_id,A.inventory_receipt,A.ir_code,A.remark,A.ref_id,A.ref_sequence_id,
            A.order_qty,A.ref_lot_no,A.so_no,A.contract_cid,A.mrp_id,A.id,A.upper_sequence,A.sequence_id,A.so_sequence_id
            FROM st_cc_details_schedule A
            INNER JOIN it_goods B ON A.within_code=B.within_code AND A.goods_id=B.id
            LEFT JOIN cd_color C ON B.within_code = C.within_code AND B.color = C.id
            Where A.within_code='{0}' AND A.id='{1}' Order by A.sequence_id", within_code, id);
            
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            List<st_cc_details_schedule> lstDetail = new List<st_cc_details_schedule>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {               
                st_cc_details_schedule mdjDetail = new st_cc_details_schedule();
                mdjDetail.id = dt.Rows[i]["id"].ToString();
                mdjDetail.upper_sequence = dt.Rows[i]["upper_sequence"].ToString();
                mdjDetail.sequence_id = dt.Rows[i]["sequence_id"].ToString();
                mdjDetail.mo_id = dt.Rows[i]["mo_id"].ToString();
                mdjDetail.obligate_mo_id = dt.Rows[i]["obligate_mo_id"].ToString();
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();
                mdjDetail.color = dt.Rows[i]["color"].ToString();
                mdjDetail.unit = dt.Rows[i]["unit"].ToString();
                mdjDetail.inventory_issuance = dt.Rows[i]["inventory_issuance"].ToString();
                mdjDetail.ii_code = dt.Rows[i]["ii_code"].ToString();
                mdjDetail.i_amount = decimal.Parse(dt.Rows[i]["i_amount"].ToString());
                mdjDetail.i_weight = decimal.Parse(dt.Rows[i]["i_weight"].ToString());
                mdjDetail.ir_lot_no = dt.Rows[i]["ir_lot_no"].ToString();                
                mdjDetail.inventory_receipt = dt.Rows[i]["inventory_receipt"].ToString();
                mdjDetail.ir_code = dt.Rows[i]["ir_code"].ToString();
                mdjDetail.remark = dt.Rows[i]["remark"].ToString();
                mdjDetail.ref_id = dt.Rows[i]["ref_id"].ToString();
                mdjDetail.ref_sequence_id = dt.Rows[i]["ref_sequence_id"].ToString();
                mdjDetail.ref_lot_no = dt.Rows[i]["ref_lot_no"].ToString();
                mdjDetail.contract_cid = dt.Rows[i]["contract_cid"].ToString();
                mdjDetail.mrp_id = dt.Rows[i]["mrp_id"].ToString();
                mdjDetail.order_qty = decimal.Parse(string.IsNullOrEmpty(dt.Rows[i]["order_qty"].ToString()) ? "0.00" : dt.Rows[i]["order_qty"].ToString());
                mdjDetail.so_no = dt.Rows[i]["so_no"].ToString();
                mdjDetail.so_sequence_id = dt.Rows[i]["so_sequence_id"].ToString();
                mdjDetail.row_status = "";
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }


    }
}
