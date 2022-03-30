using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CF.Framework.Contract;
using CF.SQLServer.DAL;
using CF.Core.Config;
using CF2025.Store.Contract;
using CF2025.Base.DAL;
using System.Data.SqlClient;

namespace CF2025.Store.DAL
{
    public static class TransferOutUnconfirmDAL
    {
        private static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        //private static string strRemoteDB = "DGERP2.cferp.dbo.";//SQLHelper.RemoteDB;
        //查詢轉出待確認列表
        public static List<TransferOutDetails> GetSearchDataList(TransferOutFind model)
        {           
            string strSql =
             @"SELECT id,Convert(varchar(10),transfer_date,120) AS transfer_date,mo_id,sequence_id,goods_id,goods_name,color,unit,transfer_amount,
             al_transfer_amount,sec_unit,sec_qty,package_num,nwt,gross_wt,location_id,move_location_id,inventory_qty,lot_no,remark
             FROM v_rpt_transfer_out_list
             WHERE within_code = '0000'";
            if (!string.IsNullOrEmpty(model.transfer_date))
            {
                strSql += string.Format(" AND transfer_date >= '{0}'", model.transfer_date);
            }
            if (!string.IsNullOrEmpty(model.transfer_date_end))
            {
                strSql += string.Format(" AND transfer_date <= '{0}'", model.transfer_date_end);
            }
            if (!string.IsNullOrEmpty(model.id))
            {
                strSql += string.Format(" AND id >= '{0}'", model.id);
            }
            if (!string.IsNullOrEmpty(model.id_end))
            {
                strSql += string.Format(" AND id <= '{0}'", model.id_end);
            }
            if (!string.IsNullOrEmpty(model.mo_id))
            {
                strSql += string.Format(" AND mo_id >= '{0}'", model.mo_id);
            }
            if (!string.IsNullOrEmpty(model.mo_id_end))
            {
                strSql += string.Format(" AND mo_id <= '{0}'", model.mo_id_end);
            }
            if (!string.IsNullOrEmpty(model.goods_id))
            {
                strSql += string.Format(" AND goods_id >= '{0}'", model.goods_id);
            }
            if (!string.IsNullOrEmpty(model.goods_id_end))
            {
                strSql += string.Format(" AND goods_id <= '{0}'", model.goods_id_end);
            }
            strSql += " AND type = '0' AND state = '1'";
            strSql += " AND (transfer_amount - al_transfer_amount) > 0 Order By id,sequence_id";
            DataTable dtOut = sh.ExecuteSqlReturnDataTable(strSql);
            List<TransferOutDetails> lstModel = CommonDAL.DataTableToList<TransferOutDetails>(dtOut);
            return lstModel;
        }

        //最大單據號
        public static string GetMaxID(string bill_id ,string dept_id)
        {            
            string strSql = string.Format("Select dbo.fn_zz_sys_bill_max_separate('{0}','{1}') as max_id", bill_id, dept_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            string id = dt.Rows.Count > 0 ? dt.Rows[0]["max_id"].ToString() : "";
            return id;
        }

        //更新轉入單表頭
        public static string SaveAll(TransferInHead model,List<TransferInDetails> lstMdl)
        {
            string result = "";
            string strSql = "";            
            //string user_id = AdminUserContext.Current.LoginInfo.LoginName;
            strSql += string.Format(@" SET XACT_ABORT ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");
            string id = model.id;
            string head_insert_status = model.head_status;
            
            if (head_insert_status=="NEW")
            {
                if (CheckHeadID(id))
                {
                    //已存在此單據號,重新取最大單據號
                    model.id = GetMaxID("ST05", model.department_id);//DDI1052201001
                }
                //***begin 更新系統表最大單據編號
                string dept_id = model.department_id;//105
                string year_month = model.id.Substring(6, 4);//2201
                string bill_code = model.id.Substring(1, 12);
                string sql_sys_update = "";
                string sql_sys_id_find = string.Format(
                    @"SELECT bill_code FROM sys_bill_max_separate WHERE within_code='0000' AND bill_id='{0}' AND year_month='{1}' and bill_text2='{2}'",
                    "ST05", year_month, dept_id);
                string sql_sys_id_insert = string.Format(
                    @"INSERT INTO sys_bill_max_separate(within_code,bill_code,bill_id,year_month,bill_text2,bill_text1,bill_text3,bill_text4,bill_text5) 
                    VALUES('0000','{0}','{1}','{2}','{3}','','','','')",
                    bill_code, "ST05", year_month, dept_id);
                string sql_sys_id_udate = string.Format(
                    @"UPDATE sys_bill_max_separate SET bill_code='{0}' WHERE within_code='0000' AND bill_id='{1}' AND year_month='{2}' and bill_text2='{3}'",
                    bill_code, "ST05", year_month, dept_id);
                DataTable dt = sh.ExecuteSqlReturnDataTable(sql_sys_id_find);
                if (dt.Rows.Count > 0)
                {
                    sql_sys_update = sql_sys_id_udate;
                    //if (!string.IsNullOrEmpty(dt.Rows[0]["bill_code"].ToString()))
                    //    sql_sys_update = sql_sys_id_udate;
                    //else
                    //    sql_sys_update = sql_sys_id_insert;
                }
                else
                {
                    sql_sys_update = sql_sys_id_insert;
                }
                strSql += sql_sys_update;
                //***end
                strSql += string.Format(
                  @" Insert Into st_transfer_mostly(within_code,id,type,transfer_date,handler,remark,state,transfers_state,update_count,department_id,servername,create_by,create_date) 
                  Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',getdate())",
                  "0000", model.id, "1", model.transfer_date, model.handler, model.remark, "0", "0", "0", model.department_id, "hkserver.cferp.dbo", model.create_by);
            }
            else
            {
                strSql += string.Format(
                @" UPDATE st_transfer_mostly SET transfer_date='{0}',handler='{1}',remark='{2}',state='{3}',department_id='{4}',update_by='{5}',update_date=getdate()
                WHERE id='{6}' AND within_code='0000'", model.transfer_date, model.handler, model.remark, model.state, model.department_id, model.update_by, model.id);
            }
            //更新明細
            if (lstMdl != null)
            {
                string sql_details_i = "";
                for (int i = 0; i < lstMdl.Count; i++)
                {
                    //明細新增
                    sql_details_i = string.Format(
                        @" Insert Into st_transfer_detail(within_code,id,sequence_id,goods_id,goods_name,unit,rate,transfer_amount,location_id,carton_code,lot_no,
                    state,remark,transfers_state,sec_unit,sec_qty,transfer_out_id,transfer_out_sequence_id,mo_id,nwt,gross_wt,package_num,position_first,shelf,
                    move_location_id,move_carton_code) VALUES('0000','{0}','{1}','{2}','{3}','{4}',{5},{6},'{7}','{8}','{9}',
                    '{10}','{11}','{12}','{13}',{14},'{15}','{16}','{17}',{18},{19},{20},'{21}','{22}','{23}','{24}')", model.id, lstMdl[i].sequence_id, lstMdl[i].goods_id,
                        lstMdl[i].goods_name, lstMdl[i].unit, 1, lstMdl[i].transfer_amount, lstMdl[i].location_id, lstMdl[i].carton_code, lstMdl[i].lot_no, '0', lstMdl[i].remark,
                        '0', lstMdl[i].sec_unit, lstMdl[i].sec_qty, lstMdl[i].transfer_out_id, lstMdl[i].transfer_out_sequence_id, lstMdl[i].mo_id, lstMdl[i].nwt, lstMdl[i].gross_wt,
                        lstMdl[i].package_num, lstMdl[i].position_first, lstMdl[i].shelf, lstMdl[i].move_location_id, lstMdl[i].move_carton_code);
                    if (head_insert_status == "NEW")
                    {
                        //如主檔為新增新單狀態,明細則全為新增
                        strSql += sql_details_i;
                    }
                    else
                    {
                        //新增狀態
                        if (lstMdl[i].row_status == "NEW")
                        {
                            strSql += sql_details_i;
                        }
                        //編輯狀態
                        if (lstMdl[i].row_status == "EDIT")
                        {
                            strSql += string.Format(
                            @" UPDATE st_transfer_detail 
                        SET goods_id='{0}',goods_name='{1}',unit='{2}',rate=1,transfer_amount={3},location_id='{4}',carton_code='{5}',lot_no='{6}',remark='{7}',sec_unit='{8}',
                        sec_qty={9},transfer_out_id='{10}',transfer_out_sequence_id='{11}',mo_id='{12}',nwt={13},gross_wt={14}, package_num={15},position_first='{16}',shelf='{17}',
                        move_location_id='{18}',move_carton_code='{19}' WHERE id='{20}' AND sequence_id='{21}' AND within_code='0000'",
                            lstMdl[i].goods_id, lstMdl[i].goods_name, lstMdl[i].unit, lstMdl[i].transfer_amount, lstMdl[i].location_id, lstMdl[i].carton_code, lstMdl[i].lot_no, lstMdl[i].remark,
                            lstMdl[i].sec_unit, lstMdl[i].sec_qty, lstMdl[i].transfer_out_id, lstMdl[i].transfer_out_sequence_id, lstMdl[i].mo_id, lstMdl[i].nwt, lstMdl[i].gross_wt, lstMdl[i].package_num,
                            lstMdl[i].position_first, lstMdl[i].shelf, lstMdl[i].move_location_id, lstMdl[i].move_carton_code, lstMdl[i].id, lstMdl[i].sequence_id);
                        }
                        //刪除狀態
                        if (lstMdl[i].row_status == "DEL")
                        {
                            strSql += string.Format(@" DELETE FROM st_transfer_detail WHERE id='{0}' AND sequence_id='{1}' AND within_code='0000'", lstMdl[i].id, lstMdl[i].sequence_id);
                        }
                    }
                }
            }       
            strSql += string.Format(@" COMMIT TRANSACTION ");
            result = sh.ExecuteSqlUpdate(strSql);           
            return result;//成功返回空格
        }

        public static TransferInHead GetHeadByID(string id)
        {
            TransferInHead mdjHead = new TransferInHead();
            string strSql= "Select * FROM st_transfer_mostly with(nolock) Where id='" + id + "'" + " AND within_code='0000'";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                mdjHead.id = dr["id"].ToString();
                mdjHead.transfer_date =dr["transfer_date"].ToString();
                mdjHead.department_id = dr["department_id"].ToString();
                mdjHead.handler = dr["handler"].ToString();
                mdjHead.remark= dr["remark"].ToString();
                mdjHead.create_by= dr["create_by"].ToString();
                mdjHead.update_by = dr["update_by"].ToString();
                mdjHead.check_by = dr["check_by"].ToString();
                mdjHead.update_count = dr["update_count"].ToString();
                mdjHead.create_date = dr["create_date"].ToString();
                mdjHead.update_date = dr["update_date"].ToString();
                mdjHead.check_date = dr["check_date"].ToString();
                mdjHead.state = dr["state"].ToString();
            }
            return mdjHead;
        }
        public static List<TransferInDetails> GetDetailsByID(string id)
        {            
            string strSql =
            @"SELECT a.*,b.do_color 
            FROM st_transfer_detail a with(nolock),it_goods b 
            WHERE a.within_code=b.within_code AND a.goods_id=b.id AND a.within_code='0000' AND a.id='" + id + "'";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            List<TransferInDetails> lstDetail = new List<TransferInDetails>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TransferInDetails mdjDetail = new TransferInDetails();               
                mdjDetail.id = dt.Rows[i]["id"].ToString();
                mdjDetail.sequence_id = dt.Rows[i]["sequence_id"].ToString();                
                mdjDetail.mo_id = dt.Rows[i]["mo_id"].ToString();
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();
                mdjDetail.unit = dt.Rows[i]["unit"].ToString();
                mdjDetail.location_id = dt.Rows[i]["location_id"].ToString();
                mdjDetail.carton_code = dt.Rows[i]["carton_code"].ToString();
                mdjDetail.shelf = dt.Rows[i]["shelf"].ToString();
                mdjDetail.transfer_amount = decimal.Parse(dt.Rows[i]["transfer_amount"].ToString());
                mdjDetail.sec_unit = dt.Rows[i]["sec_unit"].ToString();
                mdjDetail.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                mdjDetail.nwt = decimal.Parse(dt.Rows[i]["nwt"].ToString());
                mdjDetail.gross_wt = decimal.Parse(dt.Rows[i]["gross_wt"].ToString());
                mdjDetail.package_num = decimal.Parse(string.IsNullOrEmpty(dt.Rows[i]["package_num"].ToString())?"0.00": dt.Rows[i]["package_num"].ToString());
                mdjDetail.position_first = dt.Rows[i]["position_first"].ToString();
                mdjDetail.transfer_out_id = dt.Rows[i]["transfer_out_id"].ToString();
                mdjDetail.transfer_out_sequence_id = dt.Rows[i]["transfer_out_sequence_id"].ToString();                
                mdjDetail.lot_no = dt.Rows[i]["lot_no"].ToString();
                mdjDetail.remark = dt.Rows[i]["remark"].ToString();
                mdjDetail.move_location_id = dt.Rows[i]["move_location_id"].ToString();
                mdjDetail.move_carton_code = dt.Rows[i]["move_carton_code"].ToString();
                mdjDetail.do_color = dt.Rows[i]["do_color"].ToString();                
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
           
        }
       
        private static bool CheckHeadID(string id)
        {
            bool result = true;
            string strSql = "Select id FROM st_transfer_mostly with(nolock) Where '" + id + "'" + " AND within_code='0000'";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
                result = true;
            else
                result = false;
            return result;
        }

        public static string DeleteHead(TransferInHead head)
        {
            string strSql = "";
            string result = "";            
            strSql += string.Format(@" SET XACT_ABORT ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");
            strSql += string.Format(@" UPDATE st_transfer_mostly SET state='2',update_by='{0}',update_date=getdate() WHERE id='{1}' AND within_code='0000'", head.update_by,head.id );
            strSql += string.Format(@" COMMIT TRANSACTION ");
            result = sh.ExecuteSqlUpdate(strSql);
            return result;//成功返回空格
        }

        //批準&反批準
        public static List<ApproveReturnData> Approve(TransferInHead head, string approve_type)
        {
            string strProcName = "zz_store_transfer_in_approve";           
            SqlParameter[] paras = {
                new SqlParameter("@id",head.id),
                new SqlParameter("@check_by",head.check_by),
                new SqlParameter("@approve_type",approve_type)
            };
            DataTable dt = sh.ExecuteProcedureRetrunDataTable(strProcName, paras);
            List<ApproveReturnData> lstDetail = new List<ApproveReturnData>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ApproveReturnData mdjDetail = new ApproveReturnData();
                mdjDetail.id = dt.Rows[i]["id"].ToString();
                mdjDetail.sequence_id = dt.Rows[i]["sequence_id"].ToString();
                mdjDetail.mo_id = dt.Rows[i]["mo_id"].ToString();
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.qty = decimal.Parse(dt.Rows[i]["transfer_amount"].ToString());
                mdjDetail.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                mdjDetail.qty_out = decimal.Parse(dt.Rows[i]["qty"].ToString());
                mdjDetail.sec_qty_out = decimal.Parse(dt.Rows[i]["sec_qty_out"].ToString());               
                mdjDetail.approve_status = dt.Rows[i]["approve_status"].ToString();
                mdjDetail.error_info = dt.Rows[i]["error_info"].ToString();
                mdjDetail.action_type = dt.Rows[i]["action_type"].ToString();
                mdjDetail.move_location_id = dt.Rows[i]["move_location_id"].ToString();
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;            
        }

    }
}
