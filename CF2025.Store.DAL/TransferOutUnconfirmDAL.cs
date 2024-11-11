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
        public static PubFunDAL pubFunction = new PubFunDAL();
        public static string within_code = "0000";
        public static string ls_servername = "hkserver.cferp.dbo";
        public static string ldt_check_date = "";
        public static StringBuilder sbSql = new StringBuilder();

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

        ////最大單據號
        //public static string GetMaxID(string bill_id ,string dept_id, int serial_len)
        //{            
        //    string strSql = string.Format("Select dbo.fn_zz_sys_bill_max_separate('{0}','{1}',{2}) as max_id", bill_id, dept_id, serial_len);
        //    DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
        //    string id = dt.Rows.Count > 0 ? dt.Rows[0]["max_id"].ToString() : "";
        //    return id;
        //}

        //更新轉入單表頭
        public static string SaveAll(TransferInHead model, List<TransferInDetails> lstMdl)
        {
            string result = "";
            string strSql = "";
            //string user_id = AdminUserContext.Current.LoginInfo.LoginName;
            strSql += string.Format(@" SET XACT_ABORT ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");
            string id = model.id;
            string head_insert_status = model.head_status;

            if (head_insert_status == "NEW")
            {
                if (CheckHeadID(id))
                {                   
                    //已存在此單據號,重新取最大單據號
                    model.id = CommonDAL.GetMaxID("ST05", model.department_id, 3);//DDI1052201001                  
                }
                //***begin 更新系統表最大單據編號
                string dept_id = model.department_id;//105
                string year_month = "";// = model.id.Substring(6, 4);//2201
                string bill_code = "";// model.id.Substring(1, 12);
                if (model.id.Length > 12)
                {
                    year_month = model.id.Substring(6, 4);//2201
                    bill_code = model.id.Substring(1, 12);
                } else
                {
                    year_month = model.id.Substring(5, 4);//2201
                    bill_code = model.id.Substring(0, 11);
                }
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
            string strSql =
                @"Select id,Convert(varchar(10),transfer_date,121) AS transfer_date,department_id,handler,remark,create_by,update_by,check_by,
                update_count,Convert(varchar(19),create_date,121) AS create_date,Convert(varchar(19),update_date,121) AS update_date,
                Convert(varchar(19),check_date,121) AS check_date,state
                FROM st_transfer_mostly with(nolock) Where id='" + id + "'" + " AND within_code='0000'";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                mdjHead.id = dr["id"].ToString();
                mdjHead.transfer_date = dr["transfer_date"].ToString();
                mdjHead.department_id = dr["department_id"].ToString();
                mdjHead.handler = dr["handler"].ToString();
                mdjHead.remark = dr["remark"].ToString();
                mdjHead.create_by = dr["create_by"].ToString();
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
                mdjDetail.package_num = decimal.Parse(string.IsNullOrEmpty(dt.Rows[i]["package_num"].ToString()) ? "0.00" : dt.Rows[i]["package_num"].ToString());
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
            string strSql = "Select id FROM st_transfer_mostly with(nolock) Where id= '" + id + "'" + " AND within_code='0000'";
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
            strSql += string.Format(@" UPDATE st_transfer_mostly SET state='2',update_by='{0}',update_date=getdate() WHERE id='{1}' AND within_code='0000'", head.update_by, head.id);
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

        //**====================
        //**以下代碼為包裝部轉出單
        //**====================
        public static List<ItemLotNo> GetItemLotNo(string moId, string locationId)
        {
            string sql = string.Format(
            @"Select a.mo_id,a.goods_id,b.name as goods_name,a.lot_no,a.qty,a.sec_qty,a.location_id,b.do_color
            From st_details_lot a with(nolock),it_goods b
            Where a.within_code=b.within_code and a.goods_id=b.id and a.within_code='0000' and a.mo_id='{0}' and 
                  a.location_id='{1}' and a.carton_code='{1}' and a.qty>0 ", moId, locationId);
            DataTable dt = sh.ExecuteSqlReturnDataTable(sql);
            List<ItemLotNo> lstDetail = new List<ItemLotNo>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ItemLotNo mdjDetail = new ItemLotNo();
                mdjDetail.mo_id = dt.Rows[i]["mo_id"].ToString();
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();
                mdjDetail.lot_no = dt.Rows[i]["lot_no"].ToString();
                mdjDetail.qty = decimal.Parse(dt.Rows[i]["qty"].ToString());
                mdjDetail.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                mdjDetail.location_id = dt.Rows[i]["location_id"].ToString();
                mdjDetail.do_color = dt.Rows[i]["do_color"].ToString();
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }

        public static List<ItemLotNo> GetProductIdByMo(string moId,bool suit)
        {
            string sql =string.Empty;
            if (suit)
            {
                //套件
                sql=string.Format(
                @"Select b.goods_id,c.name as goods_name,c.do_color,'' as lot_no,b.prod_qty,0.00 as sec_qty
                From jo_bill_mostly a with(nolock), jo_bill_goods_details b with(nolock),it_goods c 
                Where a.within_code=b.within_code and a.id=b.id and a.ver=b.ver and b.within_code=c.within_code and b.goods_id=c.id and 
                    a.within_code='0000' and a.mo_id='{0}' and b.goods_id LIKE 'F0-%'", moId);
            }
            else
            {
               //非套件時，默認只取出面件
               sql = string.Format(
               @"SELECT TOP 1 S.goods_id,S.goods_name,S.do_color,T.lot_no,T.qty as prod_qty,T.sec_qty
                FROM 
                    (Select a.within_code,d.goods_id,e.name as goods_name,e.do_color
                    from jo_bill_mostly a with(nolock) 
                    inner join jo_bill_goods_details b with(nolock) on a.within_code=b.within_code and a.id=b.id and a.ver=b.ver
                    inner join it_bom_mostly c on b.within_code=c.within_code and b.goods_id=c.goods_id
                    inner join it_bom d on c.within_code=d.within_code and c.id=d.id and c.exp_id=d.exp_id 
                    inner join it_goods e on d.within_code=e.within_code and d.goods_id=e.id
                    where a.within_code='0000' and a.mo_id='{0}' and b.goods_id like 'F0-%' and d.primary_key='1') S,
                    st_details_lot T with(nolock)
                WHERE S.within_code=T.within_code and T.location_id='601' and t.carton_code='601' and S.goods_id=T.goods_id and T.qty>0", moId);
            }
            DataTable dt = sh.ExecuteSqlReturnDataTable(sql);
            List<ItemLotNo> lstDetail = new List<ItemLotNo>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ItemLotNo mdjDetail = new ItemLotNo();               
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();               
                mdjDetail.do_color = dt.Rows[i]["do_color"].ToString();
                mdjDetail.lot_no = dt.Rows[i]["lot_no"].ToString();
                mdjDetail.qty = decimal.Parse(dt.Rows[i]["prod_qty"].ToString());
                mdjDetail.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }

        public static List<ItemLotNo> GetPackingInfoByMo(string moId)
        {
            SqlParameter[] paras = new SqlParameter[] {
                new SqlParameter("@mo_id",moId)
            };
            DataTable dt = sh.ExecuteProcedureRetrunDataTable("zz_jo_get_plan_order_qty", paras);            
            List<ItemLotNo> lstDetail = new List<ItemLotNo>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ItemLotNo mdjDetail = new ItemLotNo();
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();                
                mdjDetail.lot_no = dt.Rows[i]["lot_no"].ToString();
                mdjDetail.qty = decimal.Parse(dt.Rows[i]["qty"].ToString());
                mdjDetail.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                mdjDetail.order_qty = decimal.Parse(dt.Rows[i]["order_qty"].ToString());
                mdjDetail.qty_rate = decimal.Parse(dt.Rows[i]["qty_rate"].ToString());
                mdjDetail.dosage = decimal.Parse(dt.Rows[i]["dosage"].ToString());
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }

        //保存轉出單
        public static string Save(TransferInHead headData, List<TransferInDetails> lstDetailData1, List<TransferDetailPart> lstDetailData2,
                                  List<TransferInDetails> lstDelData1, List<TransferDetailPart> lstDelData2,string user_id)
        {
            string result = "";
            string str = "";
            string lot_no = "";                   
            string id = headData.id;
            string head_insert_status = headData.head_status;
            string within_code = "0000";           
            string type = "0", state = "0", updateCount = "1";
            string transfersState = "0";
            string origin = "1";
            string baseUnit = "PCS";
            string strSuit = "0";
            decimal rate = 1;
            decimal packageNum = 0;
            decimal inventoryQty = 0, inventorySecQty = 0;
            decimal accountWeight = 0;

            sbSql.Clear();
            sbSql.Append(" SET XACT_ABORT ON ");
            sbSql.Append(" BEGIN TRANSACTION ");

            if (head_insert_status == "NEW")//全新的單據
            {
                bool id_exists = CommonDAL.CheckIdIsExists("st_transfer_mostly", id);
                string bill_id = "ST04";
                if (id_exists)
                {
                    //已存在此單據號,重新取最大單據號
                    headData.id = CommonDAL.GetMaxID(bill_id, headData.bill_type_no, headData.group_no, 2);//DNV024101802
                }
                //***begin 更新系統表組裝轉換的最大單據編號
                string dept_id = headData.department_id;//105
                string year_month = ""; //model.id.Substring(6, 4);//
                string bill_code = "";  //model.id.Substring(0, 11);DNV024101802
                string bill_type_no = headData.bill_type_no;
                string group_no = headData.group_no;

                year_month = headData.id.Substring(4, 6);   //241018
                bill_code = headData.id.Trim();   //DNV024101802
                string sqlSysUpdate = "";
                string sql_sys_i = string.Format(
                @" INSERT INTO sys_bill_max_separate(within_code, bill_code, bill_id,year_month, bill_text1,bill_text2, bill_text3,bill_text4,bill_text5) 
                VALUES('{0}','{1}','{2}','{3}','{4}','{5}','','','')",
                within_code, bill_code, bill_id, year_month, bill_type_no, group_no);
                string sql_sys_u = string.Format(
                @" UPDATE sys_bill_max_separate SET bill_code='{0}' WHERE within_code='{1}' And bill_id='{2}' And year_month='{3}' And bill_text1='{4}' And bill_text2='{5}'",
                bill_code, within_code, bill_id, year_month, bill_type_no, group_no);
                if (bill_code.Substring(10, 2) != "01")
                    sqlSysUpdate = sql_sys_u;
                else
                    sqlSysUpdate = sql_sys_i;
                str = sqlSysUpdate;
                sbSql.Append(str);
                //***end 更新系統表組裝轉換的最大單據編號 

                //插入主表
                str = string.Format(
                @" Insert Into st_transfer_mostly
                (within_code,id,transfer_date,type,transfers_state,handler,remark,department_id,origin,bill_type_no,group_no,state,servername,update_count,create_by,create_date) 
                Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',getdate())",
                within_code,id,headData.transfer_date, type, transfersState, headData.handler, headData.remark,headData.department_id, origin,
                headData.bill_type_no,headData.group_no, state,ls_servername, updateCount, headData.create_by );
                sbSql.Append(str);

                //插入明細表一                
                foreach (var item in lstDetailData1)
                {
                    lot_no = !string.IsNullOrEmpty(item.lot_no) ? item.lot_no : CommonDAL.GetDeptLotNo(item.location_id, item.location_id);//自動生成批號
                    //改变list中某个元素值 (例子: var model = list.Where(c => c.ID == ).FirstOrDefault(); model.Value1 = ;)
                    //历遍移交單臨時數組,更改批號與組裝單批號一致
                    //lstTurnOver.ForEach(i =>
                    //{
                    //    if (i.id == item.id && i.sequence_id == item.sequence_id)
                    //    {
                    //        i.lot_no = lot_no;
                    //    }
                    //});        
                    strSuit = (item.shipment_suit) ? "1" : "0";
                    str = string.Format(
                    @" Insert Into st_transfer_detail
                    (within_code,id,sequence_id,goods_id,goods_name,base_unit,unit,rate,inventory_qty,inventory_sec_qty,transfer_amount,location_id,carton_code,lot_no,state,remark,
                    transfers_state, account_weight, sec_unit, sec_qty, mo_id, nwt, gross_wt, package_num, shipment_suit, move_location_id, move_carton_code) Values 
                    ('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8},{9},{10},'{11}','{12}','{13}','{14}','{15}','{16}',{17},'{18}',{19},'{20}',{21},{22},{23},{24},'{25}','{26}')",
                    within_code,headData.id,item.sequence_id,item.goods_id,item.goods_name,baseUnit,item.unit,rate,inventoryQty,inventorySecQty,item.transfer_amount,item.location_id,item.carton_code,lot_no,state,item.remark,
                    transfersState,accountWeight,item.sec_unit,item.sec_qty,item.mo_id,item.nwt,item.gross_wt,packageNum, strSuit, item.move_location_id,item.move_carton_code);
                    sbSql.Append(str);
                }
              
                //插入明細表二
                decimal joQty = 0, cQty = 0;
                foreach (var item in lstDetailData2)
                {
                    str = string.Format(
                    @" Insert Into st_transfer_detail_part(
                    within_code,id,upper_sequence,sequence_id,mo_id,goods_id,jo_qty,c_qty,con_qty,unit_code,remark,sec_unit,sec_qty,location,carton_code,bom_qty,lot_no,inventory_qty,inventory_sec_qty)
                    Values('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},'{9}','{10}','{11}',{12},'{13}','{14}',{15},'{16}',{17},{18})",
                    within_code,headData.id,item.upper_sequence,item.sequence_id,item.mo_id,item.goods_id,joQty,cQty,item.con_qty,item.unit_code,item.remark,item.sec_unit,item.sec_qty,
                    item.location, item.carton_code,item.bom_qty,item.lot_no, item.inventory_qty, item.inventory_sec_qty);
                    sbSql.Append(str);
                }
            }
            else //已保存組裝單的基礎上進行增刪改
            {
                //首先處理刪除(表格一) 
                //注意加選項with(ROWLOCK),只有用到主鍵時才會用到行級鎖
                if (lstDelData1 != null)
                {                    
                    foreach (var item in lstDelData1)
                    {
                        str = string.Format(
                            @" DELETE FROM st_transfer_detail with(ROWLOCK) Where within_code='0000' AND id='{0}' AND sequence_id='{1}'",
                            item.id, item.sequence_id);
                        sbSql.Append(str);
                    }
                }
                //首先處理刪除(表格二)
                if (lstDelData2 != null)
                {
                    foreach (var item in lstDelData2)
                    {
                        str = string.Format(
                        @" DELETE FROM st_transfer_detail_part with(ROWLOCK) WHERE within_code='0000' AND id='{0}' AND upper_sequence='{1}' AND sequence_id='{2}'",
                        item.id, item.upper_sequence, item.sequence_id);
                        sbSql.Append(str);
                    }
                }
                
                //更新轉出單主檔               
                str = string.Format(
                @" UPDATE st_transfer_mostly with(ROWLOCK) 
                SET transfer_date='{2}',handler='{3}',remark='{4}', department_id='{5}',bill_type_no='{6}',group_no='{7}',
                state='{8}',update_by='{9}',update_date=getdate(),update_count= Convert(nvarchar(5),Convert(int,update_count)+1)
                WHERE within_code='{0}' AND id='{1}'",
                within_code, headData.id, headData.transfer_date, headData.handler, headData.remark,headData.department_id,headData.bill_type_no, headData.group_no,
                headData.state, headData.update_by);
                sbSql.Append(str);
                //更新明細表一               
                foreach (var item in lstDetailData1)
                {
                    if (!string.IsNullOrEmpty(item.row_status))
                    {
                        strSuit = (item.shipment_suit) ? "1" : "0";
                        //item.row_status非空,數據有新增或修改                        
                        if (item.row_status == "EDIT")
                        {
                            lot_no = !string.IsNullOrEmpty(item.lot_no) ? item.lot_no : CommonDAL.GetDeptLotNo(item.location_id, item.location_id);//自動生成批號
                            //更新轉出單明細 少了base_unit不用更新
                            str = string.Format(
                            @" UPDATE st_transfer_detail with(Rowlock) 
                            Set goods_id='{3}',goods_name='{4}',unit='{5}',rate={6},inventory_qty={7},inventory_sec_qty={8},transfer_amount={9},location_id='{10}',carton_code='{11}',
                            lot_no='{12}',state='{13}',remark='{14}',transfers_state='{15}',account_weight={16},sec_unit='{17}',sec_qty={18},mo_id='{19}',nwt={20},gross_wt={21},package_num={22},
                            shipment_suit={23},move_location_id='{24}',move_carton_code='{25}'
                            WHERE within_code='{0}' AND id='{1}' AND sequence_id='{2}'", within_code, headData.id, item.sequence_id, 
                            item.goods_id,item.goods_name,item.unit, rate, inventoryQty, inventorySecQty, item.transfer_amount,item.location_id, item.carton_code,lot_no, state,item.remark,                             
                            transfersState,accountWeight,item.sec_unit,item.sec_qty,item.mo_id,item.nwt,item.gross_wt, packageNum, strSuit, item.move_location_id,item.move_carton_code);
                            sbSql.Append(str);
                        }
                        else //INSERT ITEM//有項目新增
                        {
                            //組裝轉換明細,插入新增的記錄
                            lot_no = !string.IsNullOrEmpty(item.lot_no) ? item.lot_no : CommonDAL.GetDeptLotNo(item.location_id, item.location_id);//自動生成批號
                            str = string.Format(
                            @" Insert Into st_transfer_detail
                            (within_code,id,sequence_id,goods_id,goods_name,base_unit,unit,rate,inventory_qty,inventory_sec_qty,transfer_amount,location_id,carton_code,lot_no,state,remark,
                            transfers_state, account_weight, sec_unit, sec_qty, mo_id, nwt, gross_wt, package_num, shipment_suit, move_location_id, move_carton_code) Values 
                            ('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8},{9},{10},'{11}','{12}','{13}','{14}','{15}','{16}',{17},'{18}',{19},'{20}',{21},{22},{23},{24},'{25}','{26}')",
                            within_code, headData.id, item.sequence_id, item.goods_id, item.goods_name, baseUnit, item.unit, rate, inventoryQty, inventorySecQty, item.transfer_amount, item.location_id, item.carton_code, lot_no, state, item.remark,
                            transfersState, accountWeight, item.sec_unit, item.sec_qty, item.mo_id, item.nwt, item.gross_wt, packageNum, strSuit, item.move_location_id, item.move_carton_code);
                            sbSql.Append(str); 
                        } //end of item add
                    } //end is edit
                } //end for

                //更新明細表二
                decimal joQty = 0, cQty = 0;
                foreach (var item in lstDetailData2)
                {
                    if (!string.IsNullOrEmpty(item.row_status))
                    {
                        if (item.row_status == "EDIT")
                        {                            
                            str = string.Format(
                            @" UPDATE st_transfer_detail_part with(Rowlock) 
                            SET mo_id='{4}',goods_id='{5}',jo_qty={6}, c_qty={7},con_qty={8},unit_code='{9}',sec_qty={10},sec_unit='{11}',remark='{12}',
                            location='{13}',carton_code='{14}',bom_qty={15},lot_no='{16}',inventory_qty={17},inventory_sec_qty={18}
                            WHERE within_code='{0}' AND id='{1}' AND upper_sequence='{2}' AND sequence_id='{3}'", within_code, headData.id, item.upper_sequence, item.sequence_id,
                            item.mo_id,item.goods_id, joQty, cQty, item.con_qty, item.unit_code,item.sec_qty,item.sec_unit,item.remark, 
                            item.location,item.carton_code,item.bom_qty,item.lot_no, item.inventory_qty, item.inventory_sec_qty);
                        }
                        else
                        {
                            //插入明細表二
                            str = string.Format(
                            @" Insert Into st_transfer_detail_part(
                            within_code,id,upper_sequence,sequence_id,mo_id,goods_id,jo_qty,c_qty,con_qty,unit_code,remark,sec_unit,sec_qty,location,carton_code,bom_qty,lot_no,inventory_qty,inventory_sec_qty)
                            Values('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},'{9}','{10}','{11}',{12},'{13}','{14}',{15},'{16}',{17},{18})",
                            within_code, headData.id, item.upper_sequence, item.sequence_id, item.mo_id, item.goods_id, joQty, cQty, item.con_qty, item.unit_code, item.remark, item.sec_unit, item.sec_qty,
                            item.location, item.carton_code, item.bom_qty, item.lot_no, item.inventory_qty, item.inventory_sec_qty);
                            sbSql.Append(str);
                        }
                        sbSql.Append(str);
                    }
                }
            }

            sbSql.Append(@" COMMIT TRANSACTION ");
            result = sh.ExecuteSqlUpdate(sbSql.ToString());
            sbSql.Clear();

            return result;           
        }// -- end of SAVE

        //轉出單主檔
        public static TransferInHead GetHeadOut(string id)
        {
            TransferInHead mdjHead = new TransferInHead();
            string strSql = string.Format(
            @"Select id,Convert(nvarchar(10),transfer_date,121) As transfer_date,bill_type_no,group_no,department_id,handler,remark,
            create_by,create_date,update_by,update_date,check_by,Convert(nvarchar(19),check_date,121) As check_date,update_count,state
            FROM st_transfer_mostly with(nolock) Where within_code='{0}' And id='{1}'", within_code, id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                mdjHead.id = dr["id"].ToString();
                mdjHead.transfer_date = dr["transfer_date"].ToString();
                mdjHead.bill_type_no = dr["bill_type_no"].ToString();
                mdjHead.group_no = dr["group_no"].ToString();
                mdjHead.department_id = dr["department_id"].ToString();
                mdjHead.handler = dr["handler"].ToString();
                mdjHead.remark = dr["remark"].ToString();
                mdjHead.create_by = dr["create_by"].ToString();
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
        //轉出單明細一
        public static List<TransferInDetails> GetDetailsOutByID(string id)
        {
            string strSql = string.Format(
            @"SELECT A.id,A.sequence_id,A.mo_id,CONVERT(bit,A.shipment_suit) as shipment_suit,A.goods_id,A.goods_name,A.unit,A.transfer_amount,A.sec_unit,A.sec_qty,A.package_num,
            A.position_first,A.nwt,A.gross_wt,A.location_id,A.carton_code,A.lot_no,A.remark,B.do_color,A.move_location_id,A.move_carton_code
            FROM st_transfer_detail A with(nolock),it_goods B
            Where A.within_code=B.within_code and A.goods_id=B.id And A.within_code='{0}' And A.id='{1}' Order by A.id,A.sequence_id", within_code, id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);           
            List<TransferInDetails> lstDetail = new List<TransferInDetails>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TransferInDetails mdjDetail = new TransferInDetails();
                mdjDetail.id = dt.Rows[i]["id"].ToString();
                mdjDetail.sequence_id = dt.Rows[i]["sequence_id"].ToString();
                mdjDetail.mo_id = dt.Rows[i]["mo_id"].ToString();
                mdjDetail.shipment_suit = (dt.Rows[i]["shipment_suit"].ToString() == "True") ? true : false;
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();
                mdjDetail.unit = dt.Rows[i]["unit"].ToString();
                mdjDetail.transfer_amount = decimal.Parse(dt.Rows[i]["transfer_amount"].ToString());
                mdjDetail.sec_unit = dt.Rows[i]["sec_unit"].ToString();
                mdjDetail.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                mdjDetail.package_num = decimal.Parse(string.IsNullOrEmpty(dt.Rows[i]["package_num"].ToString()) ? "0.00" : dt.Rows[i]["package_num"].ToString());
                mdjDetail.position_first = dt.Rows[i]["position_first"].ToString();
                mdjDetail.nwt = decimal.Parse(dt.Rows[i]["nwt"].ToString());
                mdjDetail.gross_wt = decimal.Parse(dt.Rows[i]["gross_wt"].ToString());
                mdjDetail.location_id = dt.Rows[i]["location_id"].ToString();
                mdjDetail.carton_code = dt.Rows[i]["carton_code"].ToString();
                //mdjDetail.inventory_qty = decimal.Parse(dt.Rows[i]["inventory_qty"].ToString());
                //mdjDetail.inventory_sec_qty = decimal.Parse(dt.Rows[i]["inventory_sec_qty"].ToString());
                mdjDetail.lot_no = dt.Rows[i]["lot_no"].ToString();
                mdjDetail.remark = dt.Rows[i]["remark"].ToString();
                mdjDetail.do_color = dt.Rows[i]["do_color"].ToString();
                mdjDetail.move_location_id = dt.Rows[i]["move_location_id"].ToString();
                mdjDetail.move_carton_code = dt.Rows[i]["move_carton_code"].ToString();
                //mdjDetail.tran_id = dt.Rows[i]["tran_id"].ToString();               
                mdjDetail.row_status = "";
               
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }
        //轉出單明細二
        public static List<TransferDetailPart> GetDetailsPartByID(string id)
        {
            string strSql = string.Format(
            @"SELECT A.id,A.upper_sequence,A.sequence_id,A.mo_id,A.goods_id,B.name AS goods_name,A.con_qty,A.unit_code,
            A.sec_qty,A.sec_unit,A.location,A.carton_code,0 AS order_qty,0 as transfer_amount,0 AS nostorage_qty,
            'SET' as goods_unit, A.remark,A.mrp_id,Isnull(A.lot_no,'') As lot_no,0 as obligate_qty, A.bom_qty
            FROM dbo.st_transfer_detail_part A with(nolock)
            INNER JOIN it_goods B ON A.within_code=B.within_code AND A.goods_id=B.id
            Where A.within_code='{0}' AND A.id='{1}' Order by A.sequence_id", within_code, id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            List<TransferDetailPart> lstDetail = new List<TransferDetailPart>();
           
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TransferDetailPart mdjDetail = new TransferDetailPart();
                mdjDetail.id = dt.Rows[i]["id"].ToString();
                mdjDetail.upper_sequence = dt.Rows[i]["upper_sequence"].ToString();
                mdjDetail.sequence_id = dt.Rows[i]["sequence_id"].ToString();
                mdjDetail.mo_id = dt.Rows[i]["mo_id"].ToString();
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();
                mdjDetail.con_qty = decimal.Parse(dt.Rows[i]["con_qty"].ToString());
                mdjDetail.unit_code = dt.Rows[i]["unit_code"].ToString();
                mdjDetail.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                mdjDetail.sec_unit = dt.Rows[i]["sec_unit"].ToString();
                mdjDetail.location = dt.Rows[i]["location"].ToString();
                mdjDetail.carton_code = dt.Rows[i]["carton_code"].ToString();
                mdjDetail.order_qty = decimal.Parse(dt.Rows[i]["order_qty"].ToString());
                mdjDetail.transfer_amount = decimal.Parse(dt.Rows[i]["transfer_amount"].ToString());
                mdjDetail.nostorage_qty = decimal.Parse(dt.Rows[i]["nostorage_qty"].ToString());
                mdjDetail.obligate_qty = decimal.Parse(dt.Rows[i]["obligate_qty"].ToString());                
                mdjDetail.goods_unit = dt.Rows[i]["goods_unit"].ToString();
                mdjDetail.remark = dt.Rows[i]["remark"].ToString();
                mdjDetail.lot_no = dt.Rows[i]["lot_no"].ToString();
                mdjDetail.mrp_id = dt.Rows[i]["mrp_id"].ToString();
                mdjDetail.bom_qty = decimal.Parse(string.IsNullOrEmpty(dt.Rows[i]["bom_qty"].ToString()) ? "0.00" : dt.Rows[i]["bom_qty"].ToString());                
                mdjDetail.row_status = "";

                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }

        /// <summary>
        /// 批準/反批準,生成組裝單,移交單的交易,庫存更新等(失敗返回的字串前兩位是"-1")
        /// </summary>
        /// <param name="id">轉出單號</param>        
        /// <param name="user_id">當前用戶</param>
        /// <param name="approve_type">approve_type:1--批準;0--反批準</param>
        /// <returns>返回字串為空,表示成功</returns> 
        public static string Approve(TransferInHead head, string user_id, string approve_type)
        {
            string result = "", sqlUpdate = "", gs_company = "0000", sql_f = "";
            string active_name = (approve_type == "1") ? "批準" : "反批準";
            string is_active_name = (approve_type == "1") ? "pfc_ok" : "pfc_unok";
            string ls_id, ls_location_id, ls_move_location, ls_move_carton_code, ls_sequence_id, ls_mo_id, ls_shipment_suit, ls_error = "";
            string ls_goods_id, ls_unit_code, ls_department_id, ls_servername, ls_F0_goods, ls_upper_sequence, ls_carton_code, ls_obligate_mo_id;
            string ldt_mo_max_date = "", ldt_check_date = "", ldt_transfer_date = "", ldt_actual_bto_hk_date = "";
            int ll_count = 0, ll_count_order = 0;
            decimal ldc_qty, ldc_sec_qty, ldc_gross_wt, ldc_package_num, ldc_wt_sec_qty; 
            DataTable dt = new DataTable();
            DataTable dtFind = new DataTable();

            //--
            ls_id = head.id;
            ldt_check_date = CommonDAL.GetDbDateTime("L");//批準日期(長日期時間)
            ldt_check_date = (is_active_name == "pfc_ok") ? ldt_check_date : head.check_date;          
            ldt_transfer_date = head.transfer_date;
            ls_department_id = head.department_id;
            ls_servername = "hkserver.cferp.dbo";
            //--

            //--start 批準
            if (is_active_name == "pfc_ok" || is_active_name == "pfc_unok")
            {
                //設置全局的批準日期
                sql_f = string.Format(
                @"Select a.location_id,a.move_location_id,a.move_carton_code,a.sequence_id,a.mo_id,IsNull(a.shipment_suit,'0') as shipment_suit,a.goods_id,a.transfer_amount,a.sec_qty,a.unit
                From st_transfer_detail a With(nolock) Where a.within_code='{0}' And a.id='{1}'", gs_company, ls_id);
                dt = sh.ExecuteSqlReturnDataTable(sql_f);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ls_location_id = dt.Rows[i]["location_id"].ToString();
                    ls_move_location = dt.Rows[i]["move_location_id"].ToString();
                    ls_move_carton_code = dt.Rows[i]["move_carton_code"].ToString();
                    ls_sequence_id = dt.Rows[i]["sequence_id"].ToString();
                    ls_mo_id = dt.Rows[i]["mo_id"].ToString();
                    ls_shipment_suit = dt.Rows[i]["shipment_suit"].ToString();
                    ls_goods_id = dt.Rows[i]["goods_id"].ToString();
                    ldc_qty = decimal.Parse(dt.Rows[i]["transfer_amount"].ToString());
                    ldc_sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                    ls_unit_code = dt.Rows[i]["unit"].ToString();
                    ls_F0_goods = ls_goods_id;
                    if (is_active_name == "pfc_unok")
                    {
                        //先清空销售明细的数据
                        sqlUpdate = string.Format(
                        @"Update B WITH(ROWLOCK)
                        Set B.actual_bto_hk_date = Null, B.actual_bto_hk_qty = Null
                        From so_order_manage A, so_order_details B
                        Where A.within_code=B.within_code And A.id=B.id And A.ver=B.ver And A.state not in('2','V')
						      And A.within_code='{0}' And B.mo_id='{1}' And B.goods_id='{2}'", gs_company, ls_mo_id, ls_goods_id);
                        result = sh.ExecuteSqlUpdate(sqlUpdate);
                        if (result == "")
                            result = "00";
                        else
                        {
                            result = RetrunResult(result, active_name, "(so_order_details)");
                            return result;
                        }
                        // SetNull(ldt_mo_max_date)
                        sql_f = string.Format(
                        @"Select Max(Isnull(check_date, create_date)) as max_date                        
                        From st_transfer_mostly A, st_transfer_detail B
                        Where A.within_code = B.within_code And A.id = B.id  And A.state = '1'
                              And B.goods_id ='{0}' And B.mo_id ='{1}' And A.id<>'{2}'", ls_goods_id, ls_mo_id, ls_id);
                        dtFind = sh.ExecuteSqlReturnDataTable(sql_f);
                        ldt_mo_max_date = dtFind.Rows[0]["max_date"].ToString();
                    }
                    //---
                    ldt_actual_bto_hk_date = (is_active_name == "pfc_ok") ? ldt_check_date : ldt_mo_max_date;
                    sql_f = string.Format(
                    @"SELECT Count(1) as cnt FROM st_transfer_detail_part WITH(NOLOCK) WHERE within_code='{0}' AND id='{1}' And upper_sequence='{2}'", gs_company, ls_id, ls_sequence_id);
                    ll_count = int.Parse(sh.ExecuteSqlReturnObject(sql_f));
                    //--start 20130412 wangwei 增加回写销售订单中，实际回港日期、实际回港数量
                    //--20130419 wangwei 此处是更新非套件的实际回港数据，将此段代码提前到找FO编号前面
                    //--非套件
                    if (ll_count < 1 || ls_goods_id.Substring(0, 3) != "F0-")
                    {
                        ll_count_order = 0;
                        sql_f = string.Format(
                        @"Select Count(1) as ll_cnt                        
                        From so_order_manage A WITH(NOLOCK),so_order_details B WITH(NOLOCK),so_order_bom C WITH(NOLOCK)
                        Where A.within_code=B.within_code And A.id=B.id And A.ver=B.ver And A.state not in('2','V')
					        And B.within_code=C.within_code And B.id=C.id And B.ver=C.ver And B.sequence_id=C.upper_sequence
                            And A.within_code='{0}' And B.mo_id='{1}' And C.goods_id='{2}'", gs_company, ls_mo_id, ls_goods_id);
                        ll_count_order = int.Parse(sh.ExecuteSqlReturnObject(sql_f));
                        if (ll_count_order > 0)
                        {
                            //----非套件，批准、反批准时写销售订单实际回港数据
                            sqlUpdate = string.Format(
                            @"Update C WITH(ROWLOCK)
                            Set C.actual_bto_hk_date= (Case When('{3}'='pfc_ok') Then '{4}' Else '{5}' End),
								C.actual_bto_hk_qty=ISNULL(C.actual_bto_hk_qty,0)+dbo.FN_CHANGE_UNITS(C.within_code,C.goods_id,'{6}',B.goods_unit,Isnull({7},0)) * (Case When('{3}'='pfc_ok') Then 1 When('{3}'='pfc_unok') Then -1 Else 0 End),
								C.is_backhk = (Case When('{3}'='pfc_ok') Then '1' Else '0' End) --//--将散件的回港标志加入到此处
					        From so_order_manage A,so_order_details B,so_order_bom C
                            Where A.within_code = B.within_code And A.id = B.id And A.ver = B.ver And A.state not in('2','V')
						        And B.within_code = C.within_code And B.id = C.id And B.ver = C.ver And B.sequence_id = C.upper_sequence
                                And A.within_code='{0}' And B.mo_id='{1}' And C.goods_id='{2}'", gs_company, ls_mo_id, ls_goods_id, is_active_name, ldt_check_date, ldt_mo_max_date, ls_unit_code, ldc_qty);
                            result = sh.ExecuteSqlUpdate(sqlUpdate);
                            if (result == "")
                                result = "00";
                            else
                            {
                                result = RetrunResult(result, active_name, "(so_order_bom)");
                                return result;
                            }
                        }
                        //--得到F0编号
                        sql_f = string.Format(
                        @"Select B.goods_id                       
                        From so_order_manage A WITH(ROWLOCK),
                            so_order_details B WITH(ROWLOCK),
						    so_order_bom C WITH(ROWLOCK)
                        Where A.within_code=B.within_code And A.id=B.id And A.ver=B.ver
                              And B.within_code=C.within_code And B.id=C.id And B.ver=C.ver And B.sequence_id=C.upper_sequence
                              And A.state Not In('2','V') And Isnull(C.primary_key,'0')='1'
                              And A.within_code='{0}' And B.mo_id='{1}' And C.goods_id='{2}'", gs_company, ls_mo_id, ls_goods_id);
                        ls_F0_goods = sh.ExecuteSqlReturnObject(sql_f);

                        //--如果是非套件，先扣除转出仓的库存，判断方法改成判断明细是否有资料2011-03-01
                        if (is_active_name == "pfc_ok")
                        {
                            sqlUpdate = string.Format(
                            @"Insert Into st_business_record(within_code,id,goods_id,goods_name,unit,base_unit,rate,action_time,action_id,ii_qty,
                                     ii_location_id, ii_code, check_date, sequence_id, lot_no, mo_id, ib_qty, qty, dept_id, sec_unit, sec_qty,
                                     wt_sec_qty, pack_qty, servername, shelf)
                            Select within_code, id, goods_id, goods_name, unit, base_unit, rate,'{3}',
								        '48',--//转出单(-)
								        transfer_amount,location_id,carton_code,'{4}',sequence_id,lot_no,mo_id,
								        dbo.FN_CHANGE_UNITBYCV(within_code, goods_id, unit, 1,'','','*'),
								        Round(dbo.FN_CHANGE_UNITBYCV(within_code, goods_id,unit,transfer_amount,'','','/'), 4),'{5}',sec_unit,sec_qty,
								        gross_wt,package_num,'{6}',shelf
                            From st_transfer_detail 
                            Where within_code='{0}' And id='{1}' And sequence_id='{2}'", gs_company, ls_id, ls_sequence_id, ldt_transfer_date, ldt_check_date, ls_department_id, ls_servername);
                            result = sh.ExecuteSqlUpdate(sqlUpdate);
                            if (result == "")
                                result = "00";
                            else
                            {
                                result = RetrunResult(result, active_name, "(st_business_record)");
                                return result;
                            }
                            //--更新库存
                            result = pubFunction.of_update_st_details("I", "48", ls_id, ls_sequence_id, ldt_check_date, ls_error);
                            if (result.Substring(0, 2) == "-1")
                            {
                                result = RetrunResult(result, active_name, "(st_details)");
                                return result;
                            }
                            else
                            {
                                result = "00";
                            }

                            //start 20131018 将交易表中的批号回写过来,因为流动仓的批号要与现在的实际交易中的批号一样
                            sqlUpdate = string.Format(
                            @"Update A WITH(ROWLOCK)
                            Set A.lot_no = B.lot_no
                            From st_transfer_detail A, st_business_record B
                            Where A.within_code = B.within_code And A.id = B.id And A.sequence_id = B.sequence_id And B.action_id ='48'
                                  And A.within_code='{0}' And A.id='{1}' And A.sequence_id='{2}' And Isnull(A.lot_no,'')=''", gs_company, ls_id, ls_sequence_id);
                            result = sh.ExecuteSqlUpdate(sqlUpdate);
                            if (result == "")
                                result = "00";
                            else
                            {
                                result = RetrunResult(result, active_name, "(st_transfer_detail)");
                                return result;
                            }
                            //end 20131018
                        }// --end if(is_active_name=="pfc_ok")
                    } //--end of IF --非套件

                    //--套件，F0出货
                    if (ll_count > 0 && ls_goods_id.Substring(0, 3) == "F0-")
                    {
                        //--start 20130412 wangwei 增加回写销售订单中，实际回港日期、实际回港数量
                        ll_count_order = 0;
                        sql_f = string.Format(
                        @"Select Count(1) as cnt                        
                        From so_order_manage A WITH(NOLOCK),so_order_details B WITH(NOLOCK)
                        Where A.within_code = B.within_code And A.id = B.id And A.ver = B.ver And A.state not in('2','V')
						      And A.within_code ='{0}' And B.mo_id ='{1}' And B.goods_id ='{2}'", gs_company, ls_mo_id, ls_goods_id);
                        ll_count_order = int.Parse(sh.ExecuteSqlReturnObject(sql_f));
                        if (ll_count_order > 0)
                        {
                            //--F0在写sales bom时，将批准和反批准写到一起
                            sqlUpdate = string.Format(
                            @"Update C WITH(ROWLOCK)
                            Set C.actual_bto_hk_date=(Case When('{3}'='pfc_ok') Then '{4}' Else '{5}' End),
							    C.actual_bto_hk_qty=Isnull(C.actual_bto_hk_qty,0) + Isnull(C.dosage,0) * Dbo.FN_CHANGE_UNITS(B.within_code,B.goods_id,'{6}',B.goods_unit,Isnull({7},0)) * (Case When('{3}'='pfc_ok') Then 1 When('{3}'='pfc_unok') Then -1 Else 0 End)
					        From so_order_manage A,
								 so_order_details B,
                                 so_order_bom C
                            Where A.within_code=B.within_code And A.id=B.id And A.ver=B.ver And A.state not in('2','V')
							        And B.within_code=C.within_code And B.id=C.id And B.ver=C.ver And B.sequence_id=C.upper_sequence
                                    And B.within_code='{0}' And B.mo_id='{1}' And B.goods_id='{2}'", gs_company, ls_mo_id, ls_goods_id, is_active_name, ldt_check_date, ldt_mo_max_date, ls_unit_code, ldc_qty);
                            result = sh.ExecuteSqlUpdate(sqlUpdate);
                            if (result == "")
                                result = "00";
                            else
                            {
                                result = RetrunResult(result, active_name, "(so_order_bom)(2)");
                                return result;
                            }
                        }
                    }
                    //--start更新生产计划单的数量
                    if (ls_goods_id.Substring(0, 3) == "F0-")
                    {
                        ll_count_order = 0;
                        sql_f = string.Format(
                        @"Select Count(1) as cnt                        
                         From jo_bill_mostly A WITH(NOLOCK),jo_bill_goods_details B WITH(NOLOCK)
                         Where A.within_code=B.within_code And A.id=B.id And A.ver=B.ver And A.state Not In('2','V')
                               And A.within_code='{0}' And A.mo_id='{1}' And B.goods_id='{2}'", gs_company, ls_mo_id, ls_goods_id);
                        ll_count_order = int.Parse(sh.ExecuteSqlReturnObject(sql_f));
                        if (ll_count_order > 0)
                        {
                            sqlUpdate = string.Format(
                            @"Update B WITH(ROWLOCK)
                            Set B.c_qty=Isnull(B.c_qty,0) + Dbo.FN_CHANGE_UNITS(B.within_code,B.goods_id,'{3}',B.goods_unit,Isnull({4},0))*(Case When('{5}'='pfc_ok') Then 1 When('{5}'='pfc_unok') Then -1 Else 0 End),
							    B.c_qty_ok =Isnull(B.c_qty_ok,0) + Dbo.FN_CHANGE_UNITS(B.within_code, B.goods_id,'{6}', B.goods_unit,Isnull({4},0)) * (Case When('{5}'='pfc_ok') Then 1 When('{5}'='pfc_unok') Then -1 Else 0 End),
							    B.c_sec_qty =Isnull(B.c_sec_qty,0) + Isnull({7},0) * (Case When('{5}'='pfc_ok') Then 1 When('{5}'='pfc_unok') Then -1 Else 0 End),
							    B.c_sec_qty_ok = Isnull(B.c_sec_qty_ok,0) + Isnull({7},0) * (Case When('{5}'='pfc_ok') Then 1 When('{5}'='pfc_unok') Then -1 Else 0 End),
							    B.f_complete_date = (Case When('{5}'='pfc_ok') Then '{8}' Else null End)
					        From jo_bill_mostly A, jo_bill_goods_details B
                            Where A.within_code=B.within_code And A.id=B.id And A.ver=B.ver And A.state not in('2','V')
							      And A.within_code='{0}' And A.mo_id='{1}' And B.goods_id='{2}'", gs_company, ls_mo_id, ls_goods_id, ls_unit_code, ldc_qty, is_active_name, ls_unit_code, ldc_sec_qty, ldt_transfer_date);
                            result = sh.ExecuteSqlUpdate(sqlUpdate);
                            if (result == "")
                                result = "00";
                            else
                            {
                                result = RetrunResult(result, active_name, "(jo_bill_goods_details)(1)");
                                return result;
                            }
                        }
                    }
                    //--end 更新生产计划单的数量

                    if (is_active_name == "pfc_ok")
                    {
                        //--start 20130419 wangwei 当批准时，要判断销此页数是否全部回港了，如果是，就更新销售明细表中的回港
                        //--要先判断是否存在这样的数据，才更新
                        ll_count = 0;
                        sql_f = string.Format(
                        @"Select Count(1) as cnt                        
                        From so_order_manage A WITH(NOLOCK),so_order_details B WITH(NOLOCK)
                        Where A.within_code=B.within_code And A.id=B.id And A.ver=B.ver And A.state Not In('2','V') And B.within_code='{0}' And B.mo_id='{1}'
                             And (Not Exists(Select 1 From so_order_bom C Where B.within_code=C.within_code And B.id=C.id And B.ver=C.ver And B.sequence_id=C.upper_sequence
                                             And B.order_qty * Isnull(C.dosage,0) >isnull(C.actual_bto_hk_qty,0)))", gs_company, ls_mo_id);
                        ll_count = int.Parse(sh.ExecuteSqlReturnObject(sql_f));
                        if (ll_count > 0)
                        {
                            sqlUpdate = string.Format(
                            @"Update B WITH(ROWLOCK)
                            Set B.actual_bto_hk_date =(Select Max(C1.actual_bto_hk_date) From so_order_bom C1 Where B.within_code=C1.within_code And B.id=C1.id And B.ver=C1.ver And B.sequence_id=C1.upper_sequence),
							    B.actual_bto_hk_qty =(Select C2.actual_bto_hk_qty/Nullif(C2.dosage, 0) From so_order_bom C2
                                                      Where B.within_code=C2.within_code And B.id=C2.id And B.ver=C2.ver And B.sequence_id=C2.upper_sequence And Isnull(C2.primary_key,'')='1' )                                                     
					        From so_order_manage A, so_order_details B
                            Where A.within_code=B.within_code And A.id=B.id And A.ver=B.ver And A.state Not In('2','V') And B.within_code='{0}' And B.mo_id='{1}'", gs_company, ls_mo_id);
                            result = sh.ExecuteSqlUpdate(sqlUpdate);
                            if (result == "")
                                result = "00";
                            else
                            {
                                result = RetrunResult(result, active_name, "(so_order_details)(2)");
                                return result;
                            }
                        }
                        //--
                        sqlUpdate = string.Format(
                        @"INSERT INTO ST_BUSINESS_RECORD(WITHIN_CODE, ID, GOODS_ID, GOODS_NAME, UNIT, BASE_UNIT, RATE, ACTION_TIME, ACTION_ID, II_QTY,
                                II_LOCATION_ID, II_CODE, CHECK_DATE, SEQUENCE_ID, LOT_NO, mo_id, IB_QTY, QTY, dept_id, sec_unit, sec_qty,
                                wt_sec_qty, pack_qty, servername, shelf)
                        SELECT WITHIN_CODE, ID, GOODS_ID, GOODS_NAME, UNIT, BASE_UNIT, RATE,'{3}',
								'47',--//转出单(+)
								TRANSFER_AMOUNT,Isnull(move_location_id,'ZZZ'),Isnull(move_location_id,'ZZZ'),
								'{4}',SEQUENCE_ID,LOT_NO,mo_id,
							    DBO.FN_CHANGE_UNITBYCV(WITHIN_CODE, GOODS_ID,UNIT, 1,'','','*'),
							    ROUND(DBO.FN_CHANGE_UNITBYCV(WITHIN_CODE,GOODS_ID,UNIT,TRANSFER_AMOUNT,'','','/'),4),'{5}',sec_unit,sec_qty,
								gross_wt,package_num,'{6}',shelf
                        FROM  ST_TRANSFER_DETAIL
                        WHERE WITHIN_CODE='{0}' AND ID='{1}' AND SEQUENCE_ID='{2}'", gs_company, ls_id, ls_sequence_id, ldt_transfer_date, ldt_check_date, ls_department_id, ls_servername);
                        result = sh.ExecuteSqlUpdate(sqlUpdate);
                        if (result == "")
                            result = "00";
                        else
                        {
                            result = RetrunResult(result, active_name, "(st_business_record)(2)");
                            return result;
                        }
                        //--更新库存
                        result = pubFunction.of_update_st_details("I", "47", ls_id, ls_sequence_id, ldt_check_date, ls_error);
                        if (result.Substring(0, 2) == "-1")
                        {
                            result = RetrunResult(result, active_name, "(st_details)");
                            return result;
                        }
                        else
                        {
                            result = "00";
                        }
                    }// enf of pfc_ok

                    if (is_active_name == "pfc_unok")
                    {
                        ll_count_order = 0;
                        sql_f = string.Format(
                        @"Select Count(1) as cnt                        
                        From st_business_record WITH(NOLOCK)
                        Where within_code ='{0}' And id='{1}' And sequence_id='{2}' And action_id In('47','48')", gs_company, ls_id, ls_sequence_id);
                        ll_count_order = int.Parse(sh.ExecuteSqlReturnObject(sql_f));
                        if (ll_count_order > 0)
                        {
                            //先更新库存,再删除交易数据
                            result = pubFunction.of_update_st_details("D", "48", ls_id, ls_sequence_id, ldt_check_date, ls_error);
                            if (result.Substring(0, 2) == "-1")
                            {
                                result = RetrunResult(result, active_name, "(st_details)");
                                return result;
                            }
                            else
                            {
                                result = "00";
                            }

                            //先更新库存,再删除交易数据
                            result = pubFunction.of_update_st_details("D", "47", ls_id, ls_sequence_id, ldt_check_date, ls_error);
                            if (result.Substring(0, 2) == "-1")
                            {
                                result = RetrunResult(result, active_name, "(st_details)");
                                return result;
                            }
                            else
                            {
                                result = "00";
                            }
                            sqlUpdate = string.Format(
                            @"Delete From st_business_record WITH(ROWLOCK)
                              Where within_code='{0}' And id='{1}' And sequence_id='{2}' And action_id In('47','48')", gs_company, ls_id, ls_sequence_id);
                            result = sh.ExecuteSqlUpdate(sqlUpdate);
                            if (result == "")
                                result = "00";
                            else
                            {
                                result = RetrunResult(result, active_name, "(st_business_record)");
                                return result;
                            }
                        }
                    }// end of pfc_unok
                } // end of for Loop

                //--如果是套件出货，扣除Sales Bom的库存  2010-05-19
                sql_f = string.Format(
                @"Select b.sequence_id,b.upper_sequence,b.con_qty,b.goods_id,b.location,b.carton_code,b.mo_id as obligate_mo_id,a.mo_id,
                (Case When a.gross_wt-a.sec_qty>0 Then a.gross_wt-a.sec_qty Else 0 End) as gross_wt,Isnull(a.package_num,0) as package_num
                From st_transfer_detail a with(nolock),st_transfer_detail_part b with(nolock)
                Where a.id=b.id And a.sequence_id=b.upper_sequence And b.within_code='{0}' And b.id='{1}'", gs_company, ls_id);
                dt = sh.GetDataTable(sql_f);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ls_sequence_id = dt.Rows[i]["sequence_id"].ToString();
                    ls_upper_sequence = dt.Rows[i]["upper_sequence"].ToString();
                    ldc_qty = decimal.Parse(dt.Rows[i]["con_qty"].ToString());
                    ls_goods_id = dt.Rows[i]["goods_id"].ToString();
                    ls_location_id = dt.Rows[i]["location"].ToString();
                    ls_carton_code = dt.Rows[i]["carton_code"].ToString();
                    ls_obligate_mo_id = dt.Rows[i]["obligate_mo_id"].ToString();
                    ls_mo_id = dt.Rows[i]["mo_id"].ToString();
                    ldc_gross_wt = CommonDAL.ReturnFloat2(dt.Rows[i]["gross_wt"].ToString());
                    ldc_package_num = CommonDAL.ReturnFloat2(dt.Rows[i]["package_num"].ToString());
                    if (is_active_name == "pfc_ok")
                    {
                        //--只有不存在才增加，防同步冲突jeff 2011-03-21
                        sql_f = string.Format(
                        @"Select Count(1) as cnt FROM st_business_record WITH(NOLOCK) 
                        Where within_code='{0}' And id ='{1}' And action_id='36' And sequence_id='{2}' And goods_id='{3}'", gs_company, ls_id, ls_sequence_id, ls_goods_id);
                        ll_count = int.Parse(sh.ExecuteSqlReturnObject(sql_f));
                        if (ll_count == 0)
                        {
                            ll_count_order = 0;
                            sql_f = string.Format(
                            @"Select count(1) as cnt					        
					        From so_order_manage a WITH (NOLOCK),
								 so_order_details b WITH (NOLOCK),
								 so_order_bom c
					        Where a.within_code=b.within_code And a.id=b.id And a.ver=b.ver 
								  And b.within_code=c.within_code And b.id=c.id And b.ver=c.ver And b.sequence_id=c.upper_sequence
								  And a.state Not In ('2','V')  And b.state Not In('2','V') And c.primary_key='1'
								  And a.within_code='{0}' And b.mo_id='{1}' And c.goods_id='{2}'", gs_company, ls_mo_id, ls_goods_id);
                            ll_count_order = int.Parse(sh.ExecuteSqlReturnObject(sql_f));
                            //--暂时采用的方法是，毛重：全部放在主件上，包数：主件和配件都一样	
                            ldc_wt_sec_qty = (ll_count_order > 0) ? ldc_gross_wt : ldc_gross_wt;
                            //--插入库存交易表
                            sqlUpdate = string.Format(
                            @"INSERT INTO st_business_record(within_code,id,goods_id,goods_name,unit,action_time,action_id,ii_qty,ii_location_id,
									ii_code,rate,sequence_id,check_date,ib_qty,qty,mo_id,lot_no,sec_qty,sec_unit,pack_qty,wt_sec_qty,servername)
					        SELECT b.within_code,b.id,b.goods_id,b.goods_name,b.unit_code,a.transfer_date,
							        '36',--//转出单(Sales Bom)
							        Abs(b.con_qty),b.location,b.carton_code,0 as swit_rate,b.sequence_id,'{4}',
							        Abs(dbo.FN_CHANGE_UNITBYCV(b.within_code,b.goods_id,b.unit_code,1,'','','*')),
							        Abs(round(dbo.FN_CHANGE_UNITBYCV(b.within_code,b.goods_id,b.unit_code,b.con_qty,'','','/'),4)),
							        b.mo_id,b.lot_no,Abs(b.sec_qty),b.sec_unit,{5},abs(b.sec_qty) +{6},'{7}'
					        FROM 	st_transfer_mostly a,
								    st_transfer_detail_part b
					        Where 	a.within_code=b.within_code And a.id=b.id And Isnull(b.location,'')<>'' And a.within_code='{0}'
								    And a.id ='{1}' And b.upper_sequence ='{2}' And b.sequence_id='{3}'",
                            gs_company, ls_id, ls_upper_sequence, ls_sequence_id, ldt_check_date, ldc_package_num, ldc_wt_sec_qty, ls_servername);
                            result = sh.ExecuteSqlUpdate(sqlUpdate);
                            if (result == "")
                                result = "00";
                            else
                            {
                                result = RetrunResult(result, active_name, "(st_business_record)(4)");
                                return result;
                            }
                            //--更新库存
                            result = pubFunction.of_update_st_details("I", "36", ls_id, ls_sequence_id, ldt_check_date, ls_error);
                            if (result.Substring(0, 2) == "-1")
                            {
                                result = RetrunResult(result, active_name, "(st_details)");
                                return result;
                            }
                            else
                            {
                                result = "00";
                            }
                            //start 将交易表中的批号回写过来,因为流动仓的批号要与现在的实际交易中的批号一样
                            sqlUpdate = string.Format(
                            @"Update A WITH(ROWLOCK)
					        Set	A.lot_no = B.lot_no
					        From st_transfer_detail_part A,st_business_record B
					        Where A.within_code = B.within_code And A.id = B.id And A.sequence_id = B.sequence_id And B.action_id ='36'
								  And A.within_code='{0}' And A.id='{1}' And A.upper_sequence='{2}' And A.sequence_id='{3}'  And Isnull(A.lot_no,'')=''",
                            gs_company, ls_id, ls_upper_sequence, ls_sequence_id);
                            result = sh.ExecuteSqlUpdate(sqlUpdate);
                            if (result == "")
                                result = "00";
                            else
                            {
                                result = RetrunResult(result, active_name, "(st_transfer_detail_part)(lot_no)");
                                return result;
                            }
                            //end 
                        }//--enf if (ll_count == 0)

                    }// end if(is_active_name == "pfc_ok")  

                    //--
                    if (is_active_name == "pfc_unok")
                    {
                        ll_count = 0;
                        sql_f = string.Format(
                        @"Select Count(1) as cnt                        
                        FROM st_business_record WITH(NOLOCK)
                        Where within_code='{0}' And id='{1}' And action_id='36' And sequence_id='{2}' And goods_id='{3}'", gs_company, ls_id, ls_sequence_id, ls_goods_id);
                        ll_count = int.Parse(sh.ExecuteSqlReturnObject(sql_f));
                        if (ll_count > 0)
                        {
                            //先更新库存,再删除交易数据
                            result = pubFunction.of_update_st_details("D", "36", ls_id, ls_sequence_id, ldt_check_date, ls_error);
                            if (result.Substring(0, 2) == "-1")
                            {
                                result = RetrunResult(result, active_name, "(st_details)");
                                return result;
                            }
                            else
                            {
                                result = "00";
                            }
                            //--
                            sqlUpdate = string.Format(
                            @"Delete FROM st_business_record WITH(ROWLOCK)
                            Where within_code ='{0}' And id ='{1}' And action_id ='36' And sequence_id='{2}' And goods_id ='{3}'",
                            gs_company, ls_id, ls_sequence_id, ls_goods_id);
                            result = sh.ExecuteSqlUpdate(sqlUpdate);
                            if (result == "")
                                result = "00";
                            else
                            {
                                result = RetrunResult(result, active_name, "(st_business_record)(5)");
                                return result;
                            }
                        }
                    }
                    //--
                    sqlUpdate = string.Format(
                    @"Update C WITH(ROWLOCK)
                    Set C.is_backhk = (Case When('{3}'='pfc_ok') Then '1' Else '0' End)
			        From so_order_manage a ,
						so_order_details b,
                        so_order_bom C
                    Where a.within_code = b.within_code And a.id = b.id And a.ver = b.ver And A.state Not In('2','V')
                        And b.within_code =C.within_code And b.id=C.id And b.ver=C.ver And b.sequence_id=C.upper_sequence
                        And a.within_code ='{0}' And b.mo_id='{1}' And C.goods_id='{2}'", gs_company, ls_mo_id, ls_goods_id, is_active_name);
                    result = sh.ExecuteSqlUpdate(sqlUpdate);
                    if (result == "")
                        result = "00";
                    else
                    {
                        result = RetrunResult(result, active_name, "(so_order_bom)(3)");
                        return result;
                    }

                    //--更新库存预留表2010-04-22
                    sql_f = string.Format(@"Select mo_id From st_transfer_detail Where within_code='{0}' And id='{1}' And sequence_id='{2}'", gs_company, ls_id, ls_upper_sequence);
                    ls_mo_id = sh.ExecuteSqlReturnObject(sql_f);
                    ll_count = 0;
                    sql_f = string.Format(
                    @"Select Count(1) as cnt From mrp_st_details_lot WITH(NOLOCK)
                    Where within_code='{0}' And mo_id='{1}' And obligate_mo_id='{2}' And location_id='{3}' And carton_code='{4}' And goods_id='{5}'",
                    gs_company, ls_mo_id, ls_obligate_mo_id, ls_location_id, ls_carton_code, ls_goods_id);
                    ll_count = int.Parse(sh.ExecuteSqlReturnObject(sql_f));
                    //******
                    if (ll_count > 0)
                    {
                        sqlUpdate = string.Format(
                        @"Update mrp_st_details_lot WITH(ROWLOCK)
                        Set issue_qty=IsNull(issue_qty, 0) +{6}*(Case When('{7}'='pfc_ok') Then 1 When('{7}'='pfc_unok') Then -1 Else 0 End)
				        Where within_code='{0}' And mo_id='{1}' And obligate_mo_id='{2}' And location_id='{3}' And carton_code='{4}' And goods_id='{5}'",
                        gs_company, ls_mo_id, ls_obligate_mo_id, ls_location_id, ls_carton_code, ls_goods_id, ldc_qty, is_active_name);
                        result = sh.ExecuteSqlUpdate(sqlUpdate);
                        if (result == "")
                            result = "00";
                        else
                        {
                            result = RetrunResult(result, active_name, "(mrp_st_details_lot)(1)");
                            return result;
                        }
                    }
                } //--end for 如果是套件出货

                //更新批準/反批準狀態
                if (result.Substring(0, 2) == "00")
                {
                    if (is_active_name == "pfc_ok")
                    {
                        sqlUpdate = string.Format(
                        @"Update st_transfer_mostly with(Rowlock) SET check_date='{2}',check_by='{3}',update_date='{2}',update_by='{3}',state='1'
                        WHERE within_code='{0}' and id='{1}'", gs_company, ls_id, ldt_check_date, head.update_by);
                    }
                    else
                    {
                        sqlUpdate = string.Format(
                        @"Update st_transfer_mostly with(Rowlock) SET check_date=null,check_by=null,state='0',update_by='{2}',update_date=getdate()
                         WHERE within_code='{0}' and id='{1}'", gs_company, ls_id, head.update_by);
                    }
                    result = sh.ExecuteSqlUpdate(sqlUpdate);
                    if (result == "")
                        result = "00";
                    else
                    {
                        result = RetrunResult(result, active_name, "(st_transfer_mostly)(Approve/UnApprove)");
                        return result;
                    }
                }

            } //--end if (is_active_name=="pfc_ok" || is_active_name=="pfc_unok")

            return result;
        }

        public static string RetrunResult(string strResult,string active_name, string strTable)
        {
            string result = "-1" + strResult + "\r\n" + "更新库存失败!+" + "\r\n" +string.Format(@"<{0}>{1}", active_name, strTable)+ "\r\n";
            return result;
        }
    }
}
