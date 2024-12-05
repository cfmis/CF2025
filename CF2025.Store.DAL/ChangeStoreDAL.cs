using CF.Core.Config;
using CF.SQLServer.DAL;
using CF2025.Base.DAL;
using CF2025.Prod.Contract;
using CF2025.Prod.Contract.Model;
using CF2025.Store.Contract.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Store.DAL
{
    public static class ChangeStoreDAL
    {
        public static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        public static PubFunDAL pubFunction = new PubFunDAL();
        public static string within_code = "0000";
        public static string ls_servername = "hkserver.cferp.dbo";
        public static string ldt_check_date = "";
        public static StringBuilder sbSql = new StringBuilder();

        //倉庫發料，倉庫轉倉共用
        public static st_inventory_mostly GetHeadByID(string id,string turnType)
        {
            /*批準日期需按這種格式轉換,否則反準比較對不上而出錯
            *Convert(nvarchar(19),check_date,121) As check_date
            */
            st_inventory_mostly mdjHead = new st_inventory_mostly();
            string strSql = string.Format(
            @"Select id,Convert(nvarchar(10),inventory_date,121) As inventory_date,origin,bill_type_no,department_id,linkman,
            handler, remark, create_by,create_date, update_by, update_date,check_by,Convert(nvarchar(19),check_date,121) As check_date,update_count,state
            FROM st_inventory_mostly with(nolock) Where id='{0}' And within_code='{1}' And turn_type='{2}'", id, within_code, turnType);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
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

        //倉庫發料，倉庫轉倉共用
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
            Where A.id='{0}' And A.within_code='{1}' Order by A.id,A.sequence_id",id, within_code);
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

        //返回倉庫發料的的表格二明細
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

        //倉庫轉倉使用
        public static List<PlanGoods> GetPlanItemListByMo(string mo_id)
        {
            string strSql = string.Format(
            @"SELECT Distinct b.goods_id,it_goods.name As goods_name
            FROM jo_bill_mostly a With(nolock),jo_bill_goods_details b With(nolock)
              LEFT OUTER JOIN it_goods ON b.within_code=it_goods.within_code and b.goods_id= it_goods.id 
            WHERE a.within_code=b.within_code and a.id=b.id and a.ver=b.ver and a.state not in('2','V')
            And SubString(b.goods_id,1,3) <> 'F0-'
            And b.within_code='{0}' and a.mo_id='{1}'", within_code, mo_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            List<PlanGoods> lstDetail = new List<PlanGoods>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                PlanGoods mdjDetail = new PlanGoods();
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }

        //倉庫發料，倉庫轉倉共用
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
        public static string SaveCc(st_inventory_mostly headData, List<st_i_subordination> lstDetailData1, List<st_cc_details_schedule> lstDetailData2,
            List<st_i_subordination> lstDelData1, List<st_cc_details_schedule> lstDelData2, string user_id)
        {
            string result = "00";
            string str = "";
            StringBuilder sbSql = new StringBuilder(" SET XACT_ABORT ON ");
            sbSql.Append(" BEGIN TRANSACTION ");
            string id = headData.id;
            string head_insert_status = headData.head_status;


            if (head_insert_status == "NEW")//全新的單據
            {
                string billType = "ST03";//倉庫發料
                bool id_exists = CommonDAL.CheckIdIsExists("st_inventory_mostly", id);
                if (id_exists)
                {
                    //已存在此單據號,重新取最大單據號
                    headData.id = CommonDAL.GetMaxID(billType, 4); //DAA24010132
                }
                //***begin 更新系統表倉庫發料最大單據編號
                //string dept_id = headData.out_dept;//105
                string year_month = headData.id.Substring(3, 4); //2401
                string bill_code = headData.id.Substring(1, 10); //AA24010132

                string sql_sys_update1 = string.Empty;
                string sql_sys_id_insert = string.Format(
                    @" INSERT INTO sys_bill_max_separate(within_code,bill_code,bill_id,year_month,bill_text2,bill_text1,bill_text3,bill_text4,bill_text5) 
                    VALUES('0000','{0}','{1}','{2}','','','','','')", bill_code, billType, year_month);
                string sql_sys_id_udate = string.Format(
                    @" UPDATE sys_bill_max_separate SET bill_code='{0}' WHERE within_code='0000' AND bill_id='{1}' AND year_month='{2}'", bill_code, billType, year_month);
                if (bill_code.Substring(6, 4) != "0001")
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
                  within_code, headData.id, headData.inventory_date, headData.origin, headData.state, headData.bill_type_no, headData.department_id,
                  headData.linkman, headData.handler, headData.remark, headData.create_by, ls_servername);
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
                    within_code, headData.id, item.sequence_id, item.goods_id, item.base_unit, item.unit, item.i_amount, item.inventory_issuance,
                    item.ii_code, item.ii_lot_no, item.inventory_receipt, item.ir_code, item.ir_lot_no, item.remark, item.i_weight, item.ref_id,
                    item.ref_sequence_id, item.mo_id, item.mrp_id, item.obligate_mo_id, item.jo_sequence_id);
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
                    within_code, headData.id, item.upper_sequence, item.sequence_id, item.goods_id, item.i_amount, item.i_weight, item.order_qty,
                    item.inventory_issuance, item.ii_code, item.inventory_receipt, item.ir_code, item.ir_lot_no, item.remark, item.so_no, item.so_sequence_id,
                    item.ref_id, item.ref_sequence_id, item.mo_id, item.obligate_mo_id, item.unit);
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
                        within_code, item.id, item.upper_sequence, item.sequence_id);
                        sbSql.Append(str);
                    }
                }

                str = string.Format(
                @" Update st_inventory_mostly WITH(ROWLOCK)
                Set inventory_date='{2}',origin='{3}',state='{4}',bill_type_no='{5}',department_id='{6}',linkman='{7}',handler='{8}',
                remark='{9}',update_by='{10}',update_date=getdate()
                WHERE id='{0}' And within_code='{1}'",
                headData.id, within_code, headData.inventory_date, headData.origin, headData.state, headData.bill_type_no, headData.department_id,
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
                            headData.id, item.sequence_id, within_code, item.goods_id, item.base_unit, item.unit, item.i_amount, item.inventory_issuance,
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
                            within_code, headData.id, item.upper_sequence, item.sequence_id, item.goods_id, item.i_amount, item.i_weight, item.order_qty,
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
        }//--end SaveCc

        /// <summary>
        /// 保存倉庫轉倉
        /// </summary>
        /// <param name="headData"></param>
        /// <param name="lstDetailData1"></param>      
        /// <param name="lstDelData1"></param>        
        /// <param name="user_id"></param>
        /// <returns></returns>
        public static string SaveCe(st_inventory_mostly headData, List<st_i_subordination> lstDetailData1, List<st_i_subordination> lstDelData1, string user_id)
        {
            string result = "00", str = "";          
            StringBuilder sbSql = new StringBuilder(" SET XACT_ABORT ON ");
            sbSql.Append(" BEGIN TRANSACTION ");
            string id = headData.id;
            string head_insert_status = headData.head_status;


            if (head_insert_status == "NEW")//全新的單據
            {
                string billType = "ST10";//倉庫轉倉
                bool id_exists = CommonDAL.CheckIdIsExists("st_inventory_mostly", id);
                if (id_exists)
                {
                    //已存在此單據號,重新取最大單據號
                    headData.id = CommonDAL.GetMaxID(billType, 4); //DAB24010132
                }
                //***begin 更新系統表倉庫轉倉最大單據編號               
                string year_month = headData.id.Substring(3, 4); //2401
                string bill_code = headData.id.Substring(1, 10); //AB24010132
                string sql_sys_update1 = string.Empty;
                string sql_sys_id_insert = string.Format(
                @" INSERT INTO sys_bill_max_separate(within_code,bill_code,bill_id,year_month,bill_text2,bill_text1,bill_text3,bill_text4,bill_text5) 
                VALUES('0000','{0}','{1}','{2}','','','','','')", bill_code, billType, year_month);
                string sql_sys_id_udate = string.Format(
                @" UPDATE sys_bill_max_separate SET bill_code='{0}' WHERE within_code='0000' AND bill_id='{1}' AND year_month='{2}'", bill_code, billType, year_month);
                if (bill_code.Substring(6, 4) != "0001")
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
                  Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',getdate(),'1','0','{11}','B' )",
                  within_code, headData.id, headData.inventory_date, headData.origin, headData.state, headData.bill_type_no, headData.department_id,
                  headData.linkman, headData.handler, headData.remark, headData.create_by, ls_servername);
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
                    within_code, headData.id, item.sequence_id, item.goods_id, item.base_unit, item.unit, item.i_amount, item.inventory_issuance,
                    item.ii_code, item.ii_lot_no, item.inventory_receipt, item.ir_code, item.ir_lot_no, item.remark, item.i_weight, item.ref_id,
                    item.ref_sequence_id, item.mo_id, item.mrp_id, item.obligate_mo_id, item.jo_sequence_id);
                    sbSql.Append(str);
                }
                //生成移交單
                //GenNumberDAL objNum = new GenNumberDAL();
                //index = 0; //index += 1; //sequence_id = objNum.GetSequenceID(index);// index.ToString().PadLeft(4, '0')+"h"; //移交單的序號
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
                str = string.Format(
                @" Update st_inventory_mostly WITH(ROWLOCK)
                SET inventory_date='{2}',origin='{3}',state='{4}',bill_type_no='{5}',department_id='{6}',linkman='{7}',handler='{8}',
                remark='{9}',update_by='{10}',update_date=getdate()
                WHERE id='{0}' And within_code='{1}'",
                headData.id, within_code, headData.inventory_date, headData.origin, headData.state, headData.bill_type_no, headData.department_id,
                headData.linkman, headData.handler, headData.remark, headData.update_by);
                sbSql.Append(str);
                //插入明細表一
                if (lstDetailData1 != null)
                {
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
                                item.inventory_issuance, item.ii_lot_no, item.inventory_receipt, item.inventory_receipt, item.ir_lot_no, item.remark, item.i_weight, item.ref_id,
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
                                headData.id, item.sequence_id, within_code, item.goods_id, item.base_unit, item.unit, item.i_amount, item.inventory_issuance,
                                item.inventory_issuance, item.ii_lot_no, item.inventory_receipt, item.inventory_receipt, item.ir_lot_no, item.remark, item.i_weight, item.ref_id,
                                item.ref_sequence_id, item.mo_id, item.mrp_id, item.obligate_mo_id, item.jo_sequence_id);
                            }
                            sbSql.Append(str);
                        }
                    }
                }
            }
            sbSql.Append(@" COMMIT TRANSACTION ");
            result = sh.ExecuteSqlUpdate(sbSql.ToString());
            sbSql.Clear();
            result = (result == "") ? "00" : "-1";
            return result;
        } //--end SaveCe

        //批準、反批準：倉庫發料,倉庫轉倉共用
        public static string Approve(st_inventory_mostly head, string user_id, string approve_type,string turnType)
        {
            string result = "", is_active_name, is_turn_type = "";
            is_turn_type = turnType;  //区分类型,A:倉庫發料,:倉庫轉倉;
            string ls_id, ls_origin, ls_mrp_id, ls_sequence_id, ls_unit_code, ls_servername; //ls_error,ls_charge_dept;
            string ls_goods_id, ls_dept_id, ls_wp_id, ls_next_wp_id, ls_mo_id, ls_obligate_mo_id;
            int ll_count, ll_count2, ll_cnt, ll_cnt_state1, ll_cnt_state3, ll_cnt_state4; //, li_jo_ver,ll_rtn;
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
				From st_i_subordination WITH(nolock) Where id ='{0}' And within_code ='{1}'", head.id, within_code);
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
                        From jo_bill_mostly A WITH(nolock),jo_bill_goods_details B WITH(NOLOCK)
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
					        From jo_bill_mostly M WITH (nolock) 
                            Inner Join jo_bill_goods_details D WITH (nolock) On M.within_code=D.within_code And M.id=D.id And M.ver=D.ver
						    Inner Join jo_bill_materiel_details DD WITH (nolock)
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
                    if (is_turn_type == "A")
                    {
                        strSql = string.Format(
                        @"Select Count(1) as cnt            
                        From st_cc_details_schedule WITH(NOLOCK)
                        Where within_code ='{0}' And id ='{1}' And upper_sequence ='{2}'", within_code, ls_id, ls_sequence_id);
                    }
                    else
                    {
                        strSql = string.Format(
                        @"Select Count(1) as cnt            
                        From st_i_subordination WITH(nolock)
                        Where id ='{0}' And sequence_id ='{1}' And within_code ='{2}'", ls_id, ls_sequence_id, within_code);
                    }
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
                if (result == "")
                {
                    result = "00";
                }
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
                DataTable dtFind2 = sh.ExecuteSqlReturnDataTable(strSql);
                for (int i = 0; i < dtFind2.Rows.Count; i++)
                {
                    ls_sequence_id = dtFind2.Rows[i]["sequence_id"].ToString();
                    ldec_i_amount = decimal.Parse(dtFind2.Rows[i]["i_amount"].ToString());
                    ldec_i_weight = decimal.Parse(dtFind2.Rows[i]["i_weight"].ToString());
                    ls_mrp_id = dtFind2.Rows[i]["mrp_id"].ToString();
                    ldec_qty = decimal.Parse(dtFind2.Rows[i]["qty"].ToString());
                    if (is_turn_type == "A")
                    {
                        strSql = string.Format(
                        @"Select Count(1) as cnt 
                        From jo_bill_mostly a WITH(NOLOCK),
                             jo_bill_materiel_details b WITH(NOLOCK),
                             st_cc_details_schedule c WITH(NOLOCK)
                        Where a.within_code=b.within_code and a.id=b.id and a.ver=b.ver and a.state not in('2','V') and
                            b.within_code=c.within_code and b.id=c.ref_id and b.sequence_id=c.ref_sequence_id and IsNull(c.state,'0')<>'2' and
                            c.within_code='{0}' and c.id ='{1}' And c.sequence_id='{2}'",
                        within_code, ls_id, ls_sequence_id);
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
                            within_code, ls_id, ls_sequence_id, is_active_name, ldec_i_amount, ldec_i_weight);
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
                            strSql = string.Format(@"Select Count(1) as cnt From mrp_st_details_lot WITH (NOLOCK) Where within_code='{0}' And id='{1}'", within_code, ls_mrp_id);
                            dtFind = sh.ExecuteSqlReturnDataTable(strSql);
                            ll_count = int.Parse(dtFind.Rows[0]["cnt"].ToString());
                            if (ll_count > 0)
                            {
                                sql_update = string.Format(
                                @" Update mrp_st_details_lot WITH(ROWLOCK) 
                                  Set issue_qty=IsNull(issue_qty, 0) + Case When '{2}'='pfc_ok' Then IsNull({3},0) Else -IsNull({3},0) End
                                  Where within_code='{0}' And id ='{1}'",
                                within_code, ls_mrp_id, is_active_name, ldec_qty);
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
                    Where within_code='{0}' And id ='{1}' And sequence_id ='{2}' And action_id='28'", within_code, ls_id, ls_sequence_id);
                    dtFind = sh.ExecuteSqlReturnDataTable(strSql);
                    ll_count2 = int.Parse(dtFind.Rows[0]["cnt"].ToString());
                    if (is_active_name == "pfc_ok")//转出仓20131028 huangyun出库对应'库存页数'
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
                            within_code, ls_id, ls_sequence_id, ldt_date, user_id, ldt_check_date, ls_dept_id, ls_servername);
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
                    if (is_active_name == "pfc_unok")
                    {
                        if (ll_count2 > 0)
                        {
                            //先更新库存,再删除交易数据
                            result = pubFunction.of_update_st_details("D", "28", ls_id, ls_sequence_id, ldt_check_date, "");
                            if (result.Substring(0, 2) == "-1")
                            {
                                result = "-1数据保存失败<" + is_active_name + ">(st_details)" + result;
                                break;
                            }
                            //--
                            sql_update = string.Format(
                            @" Delete From st_business_record 
				            Where within_code='{0}' And id='{1}' And sequence_id='{2}' And action_id='28'", within_code, ls_id, ls_sequence_id);
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
        }//-- end 批準/反批準

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

        //倉庫發料：檢查庫存,返回庫存差異數
        public static List<check_part_stock> CheckStorageCc(string id)
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

        //倉庫轉倉：檢查庫存,返回庫存差異數
        public static List<check_part_stock> CheckStorageCe(string id)
        {
            string strSql = string.Format(
                @"SELECT TOP 1 S.out_dept,S.sequence_id,S.mo_id,S.goods_id,S.lot_no,(ISNULL(C.qty,0)-S.con_qty) AS qty,(ISNULL(C.sec_qty,0)-S.sec_qty) AS sec_qty 
                FROM (SELECT A.within_code,B.inventory_issuance As out_dept,B.sequence_id,B.mo_id,B.goods_id,B.ii_lot_no AS lot_no,
                      B.i_amount As con_qty,B.i_weight As sec_qty
	                  FROM st_inventory_mostly A With(nolock)
                      Inner Join st_i_subordination B With(nolock) On A.within_code=B.within_code and A.id=B.id
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


    }
}
