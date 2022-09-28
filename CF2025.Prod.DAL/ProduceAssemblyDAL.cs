using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CF.Core.Config;
using CF.SQLServer.DAL;
using CF2025.Prod.Contract;
using System.Data;
using System.Data.SqlClient;
using CF2025.Base.DAL;

namespace CF2025.Prod.DAL
{
    public static class ProduceAssemblyDAL
    {
        private static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        private static string within_code = "0000";
        //private static string language_id = "1";
        //private static string sequence_id = "";
        public static jo_assembly_mostly GetHeadByID(string id)
        {
            jo_assembly_mostly mdjHead = new jo_assembly_mostly();
            string strSql = string.Format("Select * FROM jo_assembly_mostly with(nolock) Where within_code='{0}' AND id='{1}'", within_code,id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                mdjHead.id = dr["id"].ToString();
                mdjHead.con_date = dr["con_date"].ToString();
                mdjHead.bill_origin= dr["bill_origin"].ToString();
                mdjHead.out_dept = dr["out_dept"].ToString();
                mdjHead.in_dept = dr["in_dept"].ToString();
                mdjHead.handover_id = dr["handover_id"].ToString();
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
        public static List<jo_assembly_details> GetDetailsByID(string id)
        {           
            string strSql = string.Format(
                @"SELECT A.id,A.sequence_id,A.mo_id, A.goods_id, B.name AS goods_name, A.con_qty, A.unit_code,
                sec_qty, A.sec_unit, A.lot_no, A.package_num, A.app_supply_side, A.remark, A.return_qty_nonce,
                A.sign_by, A.sign_date, A.location, A.carton_code,ISNULL(A.prd_id,0) as prd_id, A.jo_id,
                ISNULL(A.qc_qty,0) as qc_qty,A.jo_sequence_id, C.name AS color_name
                FROM jo_assembly_details A with(nolock)
                INNER JOIN it_goods B ON A.within_code = B.within_code AND A.goods_id = B.id
                LEFT JOIN cd_color C ON B.within_code = C.within_code AND B.color = C.id
                Where A.within_code='{0}' AND A.id='{1}' Order by A.id,A.sequence_id", within_code, id );
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            //sequence_id = "";
            //if (dt.Rows.Count > 0)
            //{
            //    //默認查原料成份的第一筆
            //    sequence_id = dt.Rows[0]["sequence_id"].ToString();
            //}           
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
                mdjDetail.sign_by = dt.Rows[i]["sign_by"].ToString();
                mdjDetail.sign_date = string.IsNullOrEmpty(dt.Rows[i]["sign_date"].ToString()) ? null : dt.Rows[i]["sign_date"].ToString();
                mdjDetail.location = dt.Rows[i]["location"].ToString();
                mdjDetail.carton_code = dt.Rows[i]["carton_code"].ToString();               
                mdjDetail.prd_id = int.Parse(dt.Rows[i]["prd_id"].ToString());
                mdjDetail.jo_id = dt.Rows[i]["jo_id"].ToString();
                mdjDetail.jo_sequence_id = dt.Rows[i]["jo_sequence_id"].ToString();
                mdjDetail.color_name = dt.Rows[i]["color_name"].ToString();
                mdjDetail.row_status = "";
                mdjDetail.four_color = "";
                mdjDetail.qc_qty = decimal.Parse(dt.Rows[i]["qc_qty"].ToString()); 
                lstDetail.Add(mdjDetail);
            }            
            return lstDetail;
        }

        public static List<jo_assembly_details_part> GetDetailsPartByID(string id)
        {                 
            string strSql = string.Format(
                @"SELECT A.id,A.upper_sequence,A.sequence_id,A.mo_id,A.goods_id,B.name AS goods_name,A.con_qty,A.unit_code
                ,A.sec_qty,A.sec_unit,A.package_num,A.bom_qty,A.base_qty,isnull(A.lot_no,'') as lot_no
                FROM dbo.jo_assembly_details_part A
                INNER JOIN it_goods B ON A.within_code=B.within_code AND A.goods_id=B.id
                Where A.within_code='{0}' AND A.id='{1}' Order by A.sequence_id", within_code, id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            List<jo_assembly_details_part> lstDetail = new List<jo_assembly_details_part>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jo_assembly_details_part mdjDetail = new jo_assembly_details_part();
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
                mdjDetail.package_num = decimal.Parse(string.IsNullOrEmpty(dt.Rows[i]["package_num"].ToString()) ? "0.00" : dt.Rows[i]["package_num"].ToString());
                mdjDetail.bom_qty = decimal.Parse(string.IsNullOrEmpty(dt.Rows[i]["bom_qty"].ToString()) ? "0.00" : dt.Rows[i]["bom_qty"].ToString()); 
                mdjDetail.base_qty = decimal.Parse(string.IsNullOrEmpty(dt.Rows[i]["base_qty"].ToString()) ? "0.00" : dt.Rows[i]["base_qty"].ToString());
                mdjDetail.lot_no = dt.Rows[i]["lot_no"].ToString();
                mdjDetail.row_status = "";
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }

        public static List<goods_id_for_plan> GetGoodsId(string mo_id,string out_dept,string in_dept)
        {
            string strSql = string.Format(
            @"SELECT Distinct b.within_code,b.id,b.sequence_id,b.so_order_id,b.so_sequence_id,b.goods_id,it_goods.name as goods_name,b.wp_id,b.next_wp_id,b.ver,
			Isnull((select Sum(isnull(qty,0)) from st_details_lot c where c.within_code='{0}' and c.location_id=b.wp_id and c.carton_code<>'ZZZ' and c.goods_id=b.goods_id and c.mo_id=a.mo_id),0) as qty,
			Isnull((select Sum(isnull(sec_qty,0)) from st_details_lot c where c.within_code='{0}' and c.location_id=b.wp_id and c.carton_code<>'ZZZ' and c.goods_id=b.goods_id and c.mo_id=a.mo_id),0) as sec_qty,
			b.next_wp_id As 'location',b.prod_qty
            FROM jo_bill_mostly a with(nolock),jo_bill_goods_details b with(nolock)
   		        LEFT OUTER JOIN it_goods ON b.within_code=it_goods.within_code And b.goods_id=it_goods.id 
            WHERE a.within_code=b.within_code And a.id=b.id And a.ver=b.ver And a.within_code='{0}' And a.mo_id ='{1}' And a.state Not In('0','2','V') 
	          And SubString(b.goods_id,1,3) <> 'F0-' And Isnull(b.hold,'')='' And b.wp_id='{2}' And b.next_wp_id='{3}'", within_code, mo_id, out_dept,in_dept);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);            
            List<goods_id_for_plan> lst = new List<goods_id_for_plan>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                goods_id_for_plan mdjDetail = new goods_id_for_plan();
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();
                mdjDetail.location = dt.Rows[i]["location"].ToString();
                mdjDetail.prod_qty = decimal.Parse(dt.Rows[i]["prod_qty"].ToString());
                mdjDetail.qty = decimal.Parse(dt.Rows[i]["qty"].ToString());
                mdjDetail.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                mdjDetail.jo_id = dt.Rows[i]["id"].ToString();
                mdjDetail.jo_sequence_id = dt.Rows[i]["sequence_id"].ToString();
                lst.Add(mdjDetail);
            }
            return lst;
        }
        
        public static List<jo_assembly_details_part> GetAssembly(string mo_id,string goods_id,string out_dept,string in_dept)
        {
            string strSql = string.Format(
                @"SELECT S.mo_id,S.materiel_id AS goods_id,S.sequence_id,S.bom_qty,S.base_qty,S.name AS goods_name
                FROM(SELECT Distinct a.mo_id,c.materiel_id,c.bom_sequence as sequence_id,c.bom_qty,c.base_qty,it_goods.name		
                FROM jo_bill_mostly a with(nolock)
	                INNER JOIN jo_bill_goods_details b with(nolock) ON a.within_code=b.within_code AND a.id =b.id AND a.ver=b.ver
                    INNER JOIN jo_bill_materiel_details c with(nolock) ON b.within_code=c.within_code AND b.id=c.id AND b.ver=c.ver AND b.sequence_id=c.upper_sequence
	                LEFT OUTER JOIN it_goods ON b.within_code=it_goods.within_code AND b.goods_id=it_goods.id 
                WHERE a.within_code='{0}' AND a.mo_id='{1}' AND a.state not in('0','2','V') 
                  And b.goods_id='{2}' AND Isnull(b.hold,'')='' AND b.wp_id='{3}' AND next_wp_id='{4}') S
                ORDER BY S.sequence_id", within_code, mo_id,goods_id,out_dept,in_dept);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            List<jo_assembly_details_part> lstDetail = new List<jo_assembly_details_part>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jo_assembly_details_part mdjDetail = new jo_assembly_details_part();               
                mdjDetail.mo_id = dt.Rows[i]["mo_id"].ToString();
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();
                mdjDetail.sequence_id = dt.Rows[i]["sequence_id"].ToString();
                mdjDetail.bom_qty = decimal.Parse(dt.Rows[i]["bom_qty"].ToString());
                mdjDetail.base_qty = decimal.Parse(dt.Rows[i]["base_qty"].ToString());
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }
       
        public static List<details_mo_lot> GetLotNoList(string goods_id, string location_id, string mo_id)
        {
            SqlParameter[] paras = new SqlParameter[]{
                new SqlParameter("@within_code", within_code),
                new SqlParameter("@goods_id", goods_id),
                new SqlParameter("@location_id",location_id),
                new SqlParameter("@mo_id", mo_id)
            };            
            DataSet dts = sh.RunProcedure("p_details_mo_lot", paras,"mo_lot");
            List<details_mo_lot> lstDetail = new List<details_mo_lot>();
            for (int i = 0; i < dts.Tables[0].Rows.Count; i++)
            {
                details_mo_lot mdjDetail = new details_mo_lot();
                mdjDetail.lot_no = dts.Tables[0].Rows[i]["lot_no"].ToString();
                mdjDetail.qty = decimal.Parse(dts.Tables[0].Rows[i]["qty"].ToString());
                mdjDetail.sec_qty = decimal.Parse(dts.Tables[0].Rows[i]["sec_qty"].ToString());
                mdjDetail.mo_id = dts.Tables[0].Rows[i]["mo_id"].ToString();
                mdjDetail.vendor_name = dts.Tables[0].Rows[i]["vendor_name"].ToString();
                mdjDetail.is_qc = dts.Tables[0].Rows[i]["is_qc"].ToString();
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }

        //顯示成份
        public static List<jo_assembly_details_part> GetMaterialByItem(string mo_id, string goods_id, string out_dept, string in_dept)
        {
            string strSql = string.Format(
            @"SELECT Distinct c.materiel_id AS goods_id,c.unit AS unit_code,c.sec_unit,d.name AS goods_name
            FROM jo_bill_mostly a with(nolock)
            INNER JOIN jo_bill_goods_details b with(nolock) ON a.within_code=b.within_code and a.id= b.id and a.ver= b.ver
            INNER JOIN jo_bill_materiel_details c with(nolock) ON c.within_code=b.within_code and c.id=b.id and b.ver=b.ver and c.upper_sequence=b.sequence_id
            INNER JOIN it_goods d ON d.within_code=c.within_code and d.id=c.materiel_id
            WHERE a.within_code='{0}' and a.mo_id='{1}' and b.goods_id='{2}' and b.wp_id='{3}' and b.next_wp_id='{4}'
            ORDER BY c.materiel_id", within_code, mo_id, goods_id, out_dept, in_dept);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            List<jo_assembly_details_part> lstDetail = new List<jo_assembly_details_part>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jo_assembly_details_part mdjDetail = new jo_assembly_details_part();              
                mdjDetail.goods_id = dt.Rows[i]["goods_id"].ToString();
                mdjDetail.goods_name = dt.Rows[i]["goods_name"].ToString();
                mdjDetail.unit_code = dt.Rows[i]["unit_code"].ToString();
                mdjDetail.sec_unit = dt.Rows[i]["sec_unit"].ToString();                
                lstDetail.Add(mdjDetail);
            }
            return lstDetail;
        }
        private static bool CheckHeadId(string id)
        {
            bool result = true;
            string strSql = "Select id FROM jo_assembly_mostly with(nolock) Where within_code='0000' AND id= '" + id + "'" ;
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
                result = true;
            else
                result = false;
            return result;
        }

        //保存
        public static string Save(jo_assembly_mostly headData, List<jo_assembly_details> lstDetailData1, List<jo_assembly_details_part> lstDetailData2, 
            List<jo_assembly_details> lstDelData1, List<jo_assembly_details_part> lstDelData2, 
            List<jo_assembly_details> lstTurnOver, List<jo_assembly_details> lstTurnOverQc )
        {
            string str = "", lot_no = "";
            StringBuilder strSql = new StringBuilder(" SET XACT_ABORT ON ");
            strSql.Append(@" BEGIN TRANSACTION ");
            //string strSql = "", lot_no = "";            
            //strSql += string.Format(@" SET XACT_ABORT ON ");
            //strSql += string.Format(@" BEGIN TRANSACTION ");
            
            string id = headData.id;
            string head_insert_status = headData.head_status;
            string sql_lot_no = string.Format(
                    @"DECLARE @lot_no nvarchar(20)
                       EXEC usp_create_lot_no '{0}','{1}','{2}',@lot_no OUTPUT
                       SELECT @lot_no AS lot_no", within_code, headData.out_dept, headData.out_dept);
            DataTable dtLotNo = new DataTable();

            if (head_insert_status == "NEW")//全新的單據
            {
                if (CheckHeadId(id))
                {
                    //已存在此單據號,重新取最大單據號
                    headData.id = CommonDAL.GetMaxID("JO19", headData.out_dept, 5);//DDI1052201001
                }
                //***begin 更新系統表組裝轉換的最大單據編號
                string dept_id = headData.out_dept;//105
                string year_month = "";//model.id.Substring(6, 4);//2201
                string bill_code = "";//model.id.Substring(1, 13);DC105220800001
                year_month = headData.id.Substring(5, 4);//2201
                bill_code = headData.id.Substring(1, 13);//C105220800001
                string sql_sys_update1 = "";
                //string sql_sys_id_find = string.Format(
                //    @"SELECT bill_code FROM sys_bill_max_separate WHERE within_code='0000' AND bill_id='{0}' AND year_month='{1}' and bill_text2='{2}'",
                //    "JO19", year_month, dept_id);
                string sql_sys_id_insert = string.Format(
                    @" INSERT INTO sys_bill_max_separate(within_code,bill_code,bill_id,year_month,bill_text2,bill_text1,bill_text3,bill_text4,bill_text5) 
                    VALUES('0000','{0}','{1}','{2}','{3}','','','','')",
                    bill_code, "JO19", year_month, dept_id);
                string sql_sys_id_udate = string.Format(
                    @" UPDATE sys_bill_max_separate SET bill_code='{0}' WHERE within_code='0000' AND bill_id='{1}' AND year_month='{2}' and bill_text2='{3}'",
                    bill_code, "JO19", year_month, dept_id);
                //DataTable dt = sh.ExecuteSqlReturnDataTable(sql_sys_id_find);
                if (bill_code.Substring(7, 5) != "00001")
                    sql_sys_update1 = sql_sys_id_udate;
                else
                    sql_sys_update1 = sql_sys_id_insert;
               
                //更新系統表移交單最大單號
                //取移交單最大單號
                string sql_max_id = string.Format(@"SELECT dbo.fn_zz_sys_bill_max_jo07('{0}','{1}','{2}') as id", headData.out_dept, headData.in_dept, "T");
                DataTable dt = sh.ExecuteSqlReturnDataTable(sql_max_id);
                string max_id = dt.Rows[0]["id"].ToString(); //max_id value is DT10560134510
                string billCode = max_id.Substring(1, 12);//biiCode value is T10560134510
                //string strSql_f = string.Format(
                //    @"SELECT bill_code FROM sys_bill_max_jo07 WHERE within_code='0000' and bill_id='JO07' AND bill_text1='T' AND bill_text2='{0}' and bill_text3='{1}'",
                //    headData.out_dept, headData.in_dept);
                string strSql_i = string.Format(
                @" INSERT INTO sys_bill_max_jo07(within_code,bill_id,year_month,bill_code,bill_text1,bill_text2,bill_text3,bill_text4,bill_text5) 
                VALUES('0000','JO07','','{0}','T','{1}','{2}','','')",billCode, headData.out_dept, headData.in_dept);
                string strSql_u = string.Format(
                @" UPDATE sys_bill_max_jo07 SET bill_code='{0}' WHERE within_code='0000' AND bill_id='JO07' AND bill_text1='T' AND bill_text2='{1}' and bill_text3='{2}'",
                billCode, headData.out_dept, headData.in_dept);
                //dt = sh.ExecuteSqlReturnDataTable(strSql_f);
                string sql_sys_update2 = "";
                if (billCode.Substring(6,5) !="00001")
                    sql_sys_update2 = strSql_u;
                else
                    sql_sys_update2 = strSql_i;

                str = sql_sys_update1 + sql_sys_update2;
                strSql.Append(str);
                //***end save form max id to system tabe 
                
                //插入主表
                if (!string.IsNullOrEmpty(headData.in_dept))
                {
                    headData.handover_id = max_id;
                }
                str = string.Format(
                  @" Insert Into jo_assembly_mostly(within_code,id,con_date,handler,update_count,transfers_state,remark,state,create_by,create_date,bill_type,
                  out_dept,in_dept,con_type,stock_type,bill_origin,handover_id,servername) 
                  Values ('{0}','{1}','{2}','{3}','0','0','{4}','0','{5}',getdate(),'C',   '{6}','{7}','0','0','2','{8}','hkserver.cferp.dbo')",
                  within_code, headData.id, headData.con_date, headData.handler, headData.remark, headData.create_by, headData.out_dept, headData.in_dept, headData.handover_id);
                strSql.Append(str);
                //插入明細表一
                foreach (var item in lstDetailData1)
                {
                    //獲取批號
                    dtLotNo = sh.ExecuteSqlReturnDataTable(sql_lot_no);
                    lot_no = dtLotNo.Rows[0]["lot_no"].ToString();

                    //改变list中某个元素值 (例子: var model = list.Where(c => c.ID == ).FirstOrDefault(); model.Value1 = ;)
                    //更改移交單的批號與組裝單的批號一致
                    lstTurnOver.ForEach(i =>
                    {
                        if (i.id == item.id && i.sequence_id == item.sequence_id)
                        {
                            i.lot_no = lot_no;
                        }
                    });
                    //更改QC移交單的批號與組裝單的批號一致
                    lstTurnOverQc.ForEach(i =>
                    {
                        if (i.id == item.id && i.sequence_id == item.sequence_id)
                        {
                            i.lot_no = lot_no;
                        }
                    });
                    str = string.Format(
                    @" Insert Into jo_assembly_details(within_code,id,sequence_id,jo_id,jo_sequence_id,goods_id,con_qty,unit_code,sec_qty,sec_unit,mo_id,
                    package_num,signfor,location,carton_code,sign_by,sign_date,lot_no,prd_id,qc_qty) Values 
                    ('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}',{8},'{9}','{10}','{11}',1,'{12}','{13}','{14}',getdate(),'{15}',{16},{17})",
                    within_code,headData.id,item.sequence_id,item.jo_id,item.jo_sequence_id,item.goods_id,item.con_qty,item.unit_code,item.sec_qty,item.sec_unit,
                    item.mo_id,item.package_num,headData.out_dept,headData.out_dept,headData.create_by,lot_no,item.prd_id,item.qc_qty);
                    strSql.Append(str);
                }
                //插入明細表二
                foreach (var item in lstDetailData2)
                {                    
                    str = string.Format(
                    @" Insert Into jo_assembly_details_part(within_code,id,upper_sequence,sequence_id,mo_id,goods_id,con_qty,unit_code,sec_qty,sec_unit,
                    package_num,remark,bom_qty,base_qty,lot_no) Values ('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}',{8},'{9}',{10},{11},{12},'{13}')",
                    within_code, headData.id,item.upper_sequence,item.sequence_id,item.mo_id,item.goods_id,item.con_qty,item.unit_code,item.sec_qty,
                    item.sec_unit,item.package_num,item.remark,item.bom_qty,item.base_qty,item.lot_no);
                    strSql.Append(str);
                }
                //生成移交單
                GenNumberDAL objNum = new GenNumberDAL();
                string update_count = "0", transfers_state = "0", bill_type_no = "T", con_type = "0", stock_type = "0", bill_origin = "2";
                string sequence_id = "", aim_jo_id = "", aim_jo_sequence = "";
                decimal jo_qty = 0, c_qty = 0, package_num = 0;
                int index ;
                if (lstTurnOver.Count > 0)
                {
                    //移交單表頭                    
                    str = string.Format(
                    @" Insert Into jo_materiel_con_mostly(within_code,id,con_date,handler, update_count,transfers_state,remark,state,bill_type_no,out_dept,in_dept,
                    con_type,stock_type,bill_origin,create_by,create_date,servername) VALUES
                    ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',getdate(),'hkserver.cferp.dbo')",
                    within_code, headData.handover_id, headData.con_date, headData.handler, update_count,transfers_state, headData.remark, headData.state, 
                    bill_type_no, headData.out_dept,headData.in_dept, con_type, stock_type, bill_origin,headData.create_by);
                    strSql.Append(str);
                    //移交單明細
                    index = 0;
                    foreach (var item in lstTurnOver)
                    {
                        index += 1;
                        sequence_id = objNum.GetSequenceID(index);// index.ToString().PadLeft(4, '0')+"h"; //移交單的序號
                        aim_jo_id = item.id;//組裝單號
                        aim_jo_sequence = item.sequence_id;//組裝單序號
                        str = string.Format(
                        @" Insert Into jo_materiel_con_details
                        (within_code,id,sequence_id,jo_id,jo_sequence_id,goods_id,jo_qty,c_qty,con_qty,unit_code,sec_qty,sec_unit,remark,mo_id,package_num,
                        location,carton_code,lot_no,aim_jo_id,aim_jo_sequence) Values
                        ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},'{9}',{10},'{11}','{12}','{13}',{14},'{15}','{16}','{17}','{18}','{19}')",
                        within_code,headData.handover_id, sequence_id,item.jo_id,item.jo_sequence_id,item.goods_id,jo_qty,c_qty,item.con_qty,item.unit_code,
                        item.sec_qty,item.sec_unit,item.remark,item.mo_id,package_num,item.location,item.carton_code,item.lot_no,aim_jo_id,aim_jo_sequence);
                        strSql.Append(str);
                    }
                }
                //生成QC移交單
                if (lstTurnOverQc.Count > 0)
                {
                    string in_dept = "702";
                    sql_max_id = string.Format(@"SELECT dbo.fn_zz_sys_bill_max_jo07('{0}','{1}','{2}') as id", headData.out_dept, in_dept, "T");
                    DataTable dtQc = sh.ExecuteSqlReturnDataTable(sql_max_id);
                    max_id = dtQc.Rows[0]["id"].ToString(); //max_id value is DT10570234510
                    billCode = max_id.Substring(1, 12);//biiCode value is T10570234510
                    strSql_i = string.Format(
                    @" INSERT INTO sys_bill_max_jo07(within_code,bill_id,year_month,bill_code,bill_text1,bill_text2,bill_text3,bill_text4,bill_text5) 
                    VALUES('0000','JO07','','{0}','T','{1}','{2}','','')", billCode, headData.out_dept, in_dept);
                    strSql_u = string.Format(
                    @" UPDATE sys_bill_max_jo07 SET bill_code='{0}'
                    WHERE within_code='0000' AND bill_id='JO07' AND bill_text1='T' AND bill_text2='{1}' and bill_text3='{2}'",billCode,headData.out_dept,in_dept);
                    sql_sys_update2 = billCode.Substring(6, 5) != "00001" ? strSql_u : strSql_i;                    
                    strSql.Append(sql_sys_update2);
                    //QC移交單表頭
                    str = string.Format(
                    @" Insert Into jo_materiel_con_mostly(within_code,id,con_date,handler, update_count,transfers_state,remark,state,bill_type_no,out_dept,in_dept,
                    con_type,stock_type,bill_origin,create_by,create_date,servername)
                    VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',getdate(),'hkserver.cferp.dbo')",
                    within_code, max_id, headData.con_date, headData.handler, update_count, transfers_state, headData.remark, headData.state,
                    bill_type_no, headData.out_dept, in_dept, con_type, stock_type, bill_origin, headData.create_by);
                    strSql.Append(str);
                    //QC移交單明細
                    index = 0;
                    foreach (var item in lstTurnOverQc)
                    {
                        index += 1;
                        sequence_id = objNum.GetSequenceID(index); //生成序號
                        aim_jo_id = item.id;//組裝單號
                        aim_jo_sequence = item.sequence_id;//組裝單序號
                        str = string.Format(
                        @" Insert Into jo_materiel_con_details
                        (within_code,id,sequence_id,jo_id,jo_sequence_id,goods_id,jo_qty,c_qty,con_qty,unit_code,sec_qty,sec_unit,remark,mo_id,package_num,
                        location,carton_code,lot_no,aim_jo_id,aim_jo_sequence) Values
                        ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},'{9}',{10},'{11}','{12}','{13}',{14},'{15}','{16}','{17}','{18}','{19}')",
                        within_code, max_id, sequence_id, item.jo_id, item.jo_sequence_id, item.goods_id, jo_qty, c_qty, item.con_qty, item.unit_code,
                        item.sec_qty, item.sec_unit, item.remark, item.mo_id, package_num, item.location, item.carton_code, item.lot_no, aim_jo_id, aim_jo_sequence);
                        strSql.Append(str);
                    }
                }
            }
            else //已保存組裝單的基礎上進行增刪改
            {
                //首先處理刪除(表格一) 
                //注意加選項with(ROWLOCK),只有用到主鍵時才會用到行級鎖
                if (lstDelData1.Count > 0)
                {
                    string assembly_id = "", assembly_seq_id = "";
                    foreach (var item in lstDelData1)
                    {
                        str = string.Format(@" DELETE FROM jo_assembly_details with(ROWLOCK) Where within_code='0000' AND id='{0}' AND sequence_id='{1}'", item.id, item.sequence_id);
                        strSql.Append(str);
                        //刪除對應的移交單(包括QC移交單)
                        assembly_id = item.id;
                        assembly_seq_id = item.sequence_id;
                        str = string.Format(
                        @" DELETE FROM jo_materiel_con_details Where mo_id='{0}' And goods_id='{1}' And aim_jo_id='{2}' And aim_jo_sequence='{3}'", 
                        item.mo_id, item.goods_id, assembly_id, assembly_seq_id);
                        strSql.Append(str);
                    }
                }
                //首先處理刪除(表格二)
                if (lstDelData2.Count > 0)
                {
                    foreach (var item in lstDelData2)
                    {
                        str = string.Format(
                        @" DELETE FROM jo_assembly_details_part with(ROWLOCK) WHERE within_code='0000' AND id='{0}' AND upper_sequence='{1}' AND sequence_id='{2}'",
                        item.id, item.upper_sequence, item.sequence_id);
                        strSql.Append(str);
                    }
                }                
                //更新組裝轉換表頭
                str = string.Format(
                @" UPDATE jo_assembly_mostly with(Rowlock) SET con_date='{2}',handler='{3}',remark='{4}',update_by='{5}',update_date=getdate(),out_dept='{6}',in_dept='{7}' 
                WHERE within_code='{0}' AND id='{1}'", within_code,headData.id,headData.con_date,headData.handler,headData.remark,headData.update_by,headData.out_dept,headData.in_dept);
                strSql.Append(str);
                //更新明細表一
                decimal ldc_con_qty = 0, ldc_sec_qty = 0;
                string strQcID = "";
                foreach (var item in lstDetailData1)
                {
                    if (!string.IsNullOrEmpty(item.row_status))//item.row_status不為空,測數據有新增或修改
                    {
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
                                dtLotNo = sh.ExecuteSqlReturnDataTable(sql_lot_no);//重新獲取批號
                                lot_no = dtLotNo.Rows[0]["lot_no"].ToString();
                            }
                            //更新組裝轉換明細                            
                            str = string.Format(
                            @" UPDATE jo_assembly_details with(Rowlock) 
                            Set jo_id='{3}',jo_sequence_id='{4}',goods_id='{5}',con_qty={6},unit_code='{7}',sec_qty={8},sec_unit='{9}', mo_id='{10}', package_num={11},
                            location='{12}',carton_code='{13}',lot_no='{14}',prd_id={15},qc_qty={16}
                            WHERE within_code='{0} AND id='{1}' AND sequence_id='{2}'",
                            within_code, headData.id, item.sequence_id, item.jo_id, item.jo_sequence_id, item.goods_id, item.con_qty, item.unit_code, item.sec_qty, item.sec_unit, 
                            item.mo_id, item.package_num, item.location, item.carton_code, lot_no, item.prd_id,item.qc_qty);                           
                            strSql.Append(str);
                            //更新移交單明細                                                        
                            if (!string.IsNullOrEmpty(headData.handover_id))
                            {
                                str = string.Format(
                                @" UPDATE jo_materiel_con_details 
                                SET jo_id='{6}',jo_sequence_id='{7}',con_qty={8},unit_code='{9}',sec_qty={10},sec_unit='{11}',remark='{12}',
                                package_num={13},location='{14}',carton_code='{15}',lot_no='{16}'
                                Where mo_id='{0}' And goods_id='{1}' And within_code='{2}' And id='{3}' And aim_jo_id='{4}' And aim_jo_sequence='{5}'", 
                                item.mo_id, item.goods_id, within_code, headData.handover_id,item.id,item.sequence_id,
                                item.jo_id,item.jo_sequence_id,ldc_con_qty,item.unit_code,ldc_sec_qty,item.sec_unit,item.remark,
                                item.package_num, item.location,item.carton_code,lot_no);
                                strSql.Append(str);
                            }
                        }
                        else //INSERT ITEM//有項目新增
                        {
                            //組裝轉換插入新的明細記錄
                            dtLotNo = sh.ExecuteSqlReturnDataTable(sql_lot_no);//生成批號
                            lot_no = dtLotNo.Rows[0]["lot_no"].ToString();
                            str = string.Format(
                            @" Insert Into jo_assembly_details
                            (within_code,id,sequence_id,jo_id,jo_sequence_id,goods_id,con_qty,unit_code,sec_qty,sec_unit,mo_id,package_num,location,carton_code,lot_no,prd_id) 
                            Values('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}',{8},'{9}','{10}',{11},'{12}','{13}','{14}','{15}')",
                            within_code, headData.id, item.sequence_id, item.jo_id,item.jo_sequence_id,item.goods_id,item.con_qty,item.unit_code,item.sec_qty,item.sec_unit, 
                            item.mo_id,item.package_num, headData.out_dept, headData.out_dept, lot_no,item.prd_id);
                            strSql.Append(str);
                            decimal jo_qty = 0, c_qty = 0;
                            string aim_jo_id = "", aim_jo_sequence="";
                            if (!string.IsNullOrEmpty(headData.handover_id))//已保存有移交單號
                            {                                
                                //新增的記錄插入已存在的移交單中
                                aim_jo_id = item.id;//組裝單號
                                aim_jo_sequence = item.sequence_id;//組裝轉換單序號
                                str = string.Format(
                                @" Insert Into jo_materiel_con_details
                                (within_code,id,sequence_id,jo_id,jo_sequence_id,goods_id,jo_qty,c_qty,con_qty,unit_code,sec_qty,sec_unit,remark,mo_id,package_num,
                                location,carton_code,lot_no,aim_jo_id,aim_jo_sequence) Values
                                ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},'{9}',{10},'{11}','{12}','{13}',{14},'{15}','{16}','{17}','{18}','{19}')",
                                within_code, headData.handover_id,item.sequence_id,item.jo_id,item.jo_sequence_id,item.goods_id,jo_qty,c_qty,ldc_con_qty,item.unit_code,
                                ldc_sec_qty, item.sec_unit, item.remark, item.mo_id, item.package_num, item.location, item.carton_code, item.lot_no, aim_jo_id, aim_jo_sequence);
                                strSql.Append(str);                                
                                if (item.qc_qty > 0 && headData.in_dept!="702")/*&& item.con_qty > item.qc_qty*/
                                {
                                    //查找此組裝單號有沒生成QC的移交單,有就利用原有的移交單
                                    if (strQcID == "")
                                    {
                                        string str_f = string.Format(
                                        @"SELECT top 1 id From jo_materiel_con_details with(nolock) 
                                        Where mo_id='{0}' And goods_id='{1}' And within_code='{2}' and Substring(id,6,3)='702' AND aim_jo_id='{3}'", item.mo_id, item.goods_id, within_code, item.id);
                                        DataTable dt = new DataTable();
                                        dt = sh.ExecuteSqlReturnDataTable(str_f);
                                        if (dt.Rows.Count > 0)
                                        {
                                            //已有交QC的原始數據,在原有移交單上插入記錄
                                            strQcID = dt.Rows[0]["id"].ToString();
                                            str = string.Format(
                                            @" Insert Into jo_materiel_con_details 
                                            (within_code,id,sequence_id,jo_id,jo_sequence_id,goods_id,jo_qty,c_qty,con_qty,unit_code,sec_qty,sec_unit,remark,mo_id,package_num,
                                            location,carton_code,lot_no,aim_jo_id,aim_jo_sequence) Values
                                            ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},'{9}',{10},'{11}','{12}','{13}',{14},'{15}','{16}','{17}','{18}','{19}')",
                                            within_code,strQcID,item.sequence_id,item.jo_id,item.jo_sequence_id,item.goods_id,jo_qty,c_qty,item.qc_qty,item.unit_code,
                                            0.01, item.sec_unit,item.remark, item.mo_id, 0, item.location, item.carton_code, lot_no, aim_jo_id, aim_jo_sequence);
                                            strSql.Append(str);
                                        }
                                        else
                                        {
                                            //重新生成QC移
                                            //更新系統表  
                                            string in_dept="702", update_count = "0", transfers_state = "0", bill_type_no = "T", con_type = "0", stock_type = "0", bill_origin = "2";                                            
                                            strQcID = string.Format(@"SELECT dbo.fn_zz_sys_bill_max_jo07('{0}','{1}','{2}') as id", headData.out_dept, in_dept, "T");
                                            DataTable dtQc = sh.ExecuteSqlReturnDataTable(strQcID);
                                            strQcID = dtQc.Rows[0]["id"].ToString(); //max_id value is DT10570234510
                                            string billCode = strQcID.Substring(1, 12);//biiCode value is T10570234510
                                            string strSql_i = string.Format(
                                            @" INSERT INTO sys_bill_max_jo07(within_code,bill_id,year_month,bill_code,bill_text1,bill_text2,bill_text3,bill_text4,bill_text5) 
                                            VALUES('0000','JO07','','{0}','T','{1}','{2}','','')", billCode, headData.out_dept, in_dept);
                                            string strSql_u = string.Format(
                                            @" UPDATE sys_bill_max_jo07 SET bill_code='{0}' 
                                            WHERE within_code='0000' AND bill_id='JO07' AND bill_text1='T' AND bill_text2='{1}' and bill_text3='{2}'", billCode, headData.out_dept, in_dept);
                                            str = billCode.Substring(6, 5) != "00001" ? strSql_u : strSql_i;
                                            strSql.Append(str);
                                            //QC移交單表頭
                                            str = string.Format(
                                            @" Insert Into jo_materiel_con_mostly(within_code,id,con_date,handler, update_count,transfers_state,remark,state,bill_type_no,out_dept,in_dept,
                                            con_type,stock_type,bill_origin,create_by,create_date,servername)
                                            VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',getdate(),'hkserver.cferp.dbo')",
                                            within_code, strQcID, headData.con_date, headData.handler, update_count, transfers_state, headData.remark, headData.state,
                                            bill_type_no, headData.out_dept, in_dept, con_type, stock_type, bill_origin, headData.create_by);
                                            strSql.Append(str);
                                            //QC移交單明細 
                                            aim_jo_id = item.id;//組裝單號
                                            aim_jo_sequence = item.sequence_id;//組裝單序號
                                            str = string.Format(
                                            @" Insert Into jo_materiel_con_details
                                            (within_code,id,sequence_id,jo_id,jo_sequence_id,goods_id,jo_qty,c_qty,con_qty,unit_code,sec_qty,sec_unit,remark,mo_id,package_num,
                                            location,carton_code,lot_no,aim_jo_id,aim_jo_sequence) Values
                                            ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},'{9}',{10},'{11}','{12}','{13}',{14},'{15}','{16}','{17}','{18}','{19}')",
                                            within_code,strQcID,item.sequence_id,item.jo_id,item.jo_sequence_id,item.goods_id,jo_qty,c_qty,item.qc_qty,item.unit_code,0.01,
                                            item.sec_unit, item.remark, item.mo_id, 0, item.location, item.carton_code, lot_no, aim_jo_id, aim_jo_sequence); 
                                            strSql.Append(str);                                            
                                        }
                                    }
                                    else //strQcID不為空情況
                                    {
                                        //QC移交單明細 
                                        str = string.Format(
                                        @" Insert Into jo_materiel_con_details
                                        (within_code,id,sequence_id,jo_id,jo_sequence_id,goods_id,jo_qty,c_qty,con_qty,unit_code,sec_qty,sec_unit,remark,mo_id,package_num,
                                        location,carton_code,lot_no,aim_jo_id,aim_jo_sequence) Values
                                        ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},'{9}',{10},'{11}','{12}','{13}',{14},'{15}','{16}','{17}','{18}','{19}')",
                                        within_code,strQcID,item.sequence_id,item.jo_id,item.jo_sequence_id,item.goods_id,jo_qty,c_qty,item.qc_qty,item.unit_code,0.01,
                                        item.sec_unit, item.remark, item.mo_id, 0, item.location, item.carton_code, lot_no, aim_jo_id, aim_jo_sequence);
                                        strSql.Append(str);
                                    }
                                }   //end of QC                            
                            } //end of is have a delivery ID No.
                        } //end of item add
                    } //end is edit
                } //end for
                //更新明細表二
                foreach (var item in lstDetailData2)
                {
                    if (!string.IsNullOrEmpty(item.row_status))
                    {                        
                        if (item.row_status == "EDIT")
                        {
                            lot_no = item.lot_no;
                            str = string.Format(
                            @" UPDATE jo_assembly_details_part with(Rowlock) SET mo_id='{4}',goods_id='{5}',con_qty={6},unit_code='{7}',sec_qty={8},sec_unit='{9}',package_num={10},
                            remark='{11}',bom_qty={12},base_qty={13},lot_no='{14}' WHERE within_code='{0}' AND id='{1}' AND upper_sequence='{2}' AND sequence_id='{3}'", 
                            within_code,headData.id,item.upper_sequence,item.sequence_id,item.mo_id,item.goods_id,item.con_qty,item.unit_code,item.sec_qty,item.sec_unit,                           
                            item.package_num, item.remark, item.bom_qty, item.base_qty, item.lot_no) ;
                        }
                        else
                        {
                            str = string.Format(
                             @" Insert Into jo_assembly_details_part(within_code,id,upper_sequence,sequence_id,mo_id,goods_id,con_qty,unit_code,sec_qty,sec_unit,package_num,
                             remark,bom_qty,base_qty,lot_no) Values ('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}',{8},'{9}',{10},'{11}',{12},{13},'{14}')",
                             within_code, headData.id, item.upper_sequence, item.sequence_id,item.mo_id,item.goods_id,item.con_qty, item.unit_code,item.sec_qty,item.sec_unit, 
                             item.package_num, item.remark,item.bom_qty, item.base_qty, item.lot_no);
                        }
                        strSql.Append(str);
                    }
                }
            }
            strSql.Append(@" COMMIT TRANSACTION ");
            string result = sh.ExecuteSqlUpdate(strSql.ToString());
            return result;
        }

        //檢查庫存,返回庫存差異數
        public static List<check_part_stock> CheckStorage(List<check_part_stock> partData )
        {
            DataTable dt = new DataTable();
            dt = CommonDAL.ListToDataTable<check_part_stock>(partData);
            string strProcName = "zz_prod_assembly_check_stock";
            SqlParameter[] paras = {
                new SqlParameter("@check_stock_data",SqlDbType.Structured) {Value = dt}
            };
            dt = sh.ExecuteProcedureRetrunDataTable(strProcName, paras);
            List<check_part_stock> lstDetail = new List<check_part_stock>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                check_part_stock lst = new check_part_stock();              
                lst.out_dept = dt.Rows[i]["out_dept"].ToString();
                lst.upper_sequence = dt.Rows[i]["upper_sequence"].ToString();
                lst.sequence_id = dt.Rows[i]["sequence_id"].ToString();
                lst.goods_id = dt.Rows[i]["goods_id"].ToString();
                lst.lot_no = dt.Rows[i]["lot_no"].ToString();
                lst.con_qty = decimal.Parse(dt.Rows[i]["qty"].ToString());
                lst.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                lstDetail.Add(lst);
            }
            return lstDetail;
        }
        //是否可超生產數開移交單的参数判断
        public static string CheckParameters(string id)
        {
            string result = "0";            
            string strSql = string.Format(@"Select value From sy_parameters_setup WITH (NOLOCK) Where sequence_id = '{0}' And within_code = '0000'", id);
            DataTable dt = new DataTable();
            dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["value"].ToString();
            }           
            return result;
        }
        //是否電鍍部門
        public static string CheckPlateDept(string id)
        {
            string result = "0";                      
            string strSql = string.Format(@"Select op_dept From cd_department WITH (NOLOCK) Where within_code ='0000' and id ='{0}'", id);
            DataTable dt = new DataTable();
            dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["op_dept"].ToString();
            }            
            return result;
        }
        //批量生產單是否已開過組裝轉換
        public static string CheckIsExistsPrdId(int prd_id,string id)
        {
            string result = "";
            string strSql = string.Format(
                @"Select A.id From jo_assembly_mostly A,jo_assembly_details B
                Where A.within_code = B.within_code And A.id = B.id And A.state <> '2' And
                A.within_code='0000' And B.prd_id = {0} And B.id <> '{1}'", prd_id,id);
            DataTable dt = new DataTable();
            dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["id"].ToString();
            }            
            return result;
        }
        //檢查計劃流程是否存在
        public static int CheckIsExistsPlan(string mo_id, string goods_id,string wp_id,string next_wp_id)
        {
            int result = 0;
            string strSql = string.Format(
                @"Select Count(1) as rs From jo_bill_mostly A with(nolock),jo_bill_goods_details B with(nolock)
			    Where A.within_code = B.within_code And A.id = B.id And A.ver = B.ver And A.state Not In('0','2','V')
				And A.within_code='0000' And A.mo_id='{0}' And B.goods_id='{1}' And B.wp_id='{2}' And B.next_wp_id='{3}'",
                mo_id, goods_id,wp_id,next_wp_id);
            DataTable dt = new DataTable();
            dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                result = int.Parse(dt.Rows[0]["rs"].ToString());
            }            
            return result;            
        }

        //--得到流程对应的QC流程中的未完成QC数量,如果没有QC流程,返回0       
        //ls_assembly_dept = dw_master.GetItemString(dw_master.GetRow(),'assembly_dept')
        //ls_mo_id = dw_detail.GetItemString(row,'mo_id')
        //ls_goods_id = dw_detail.GetItemString(row,'goods_id')
        //如果有完成数量都不需要考虑QC流程了,当作已经做完了QC的移交单
        public static int GetQcQty(string mo_id, string goods_id, string wp_id)
        {
            int result = 0;
            string strSql = string.Format(
            @"Select B.prod_qty From jo_bill_mostly A with(nolock),jo_bill_goods_details B with(nolock)
            Where A.within_code=B.within_code And A.id=B.id And A.ver=B.ver And A.state Not In('2','V')
              And B.next_wp_id='702' And Isnull(B.c_qty_ok, 0) = 0 And B.within_code='0000'
              And A.mo_id='{0}' And B.goods_id='{1}' And B.wp_id='{2}'", mo_id, goods_id, wp_id);
            DataTable dt = new DataTable();
            dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                result = int.Parse(dt.Rows[0]["prod_qty"].ToString());
            }            
            return result;
        }
        //--已组装的数量        
        public static List<assembly_qty> GetAssemblyQty(string id,string mo_id, string goods_id, string out_dept) 
        {
            string strSql = string.Format(
                @"Select Sum(D.con_qty) as con_qty,Sum(D.sec_qty) as sec_qty From jo_assembly_mostly M with(nolock),jo_assembly_details D with(nolock)
		        Where M.within_code=D.within_code And M.id=D.id And M.state Not In('2','V') And D.within_code='{0}'
				And D.id <>'{1}' And D.mo_id='{2}' And D.goods_id='{3}' And M.out_dept='{4}'",
                within_code,id,mo_id,goods_id,out_dept);
            DataTable dt = new DataTable();
            dt = sh.ExecuteSqlReturnDataTable(strSql);           
            List<assembly_qty> lstDetail = new List<assembly_qty>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    assembly_qty lst = new assembly_qty();
                    lst.con_qty = decimal.Parse(dt.Rows[i]["con_qty"].ToString());
                    lst.sec_qty = decimal.Parse(dt.Rows[i]["sec_qty"].ToString());
                    lstDetail.Add(lst);
                }
            }else
            {
                assembly_qty lst = new assembly_qty();
                lst.con_qty = 0;
                lst.sec_qty = 0;
                lstDetail.Add(lst);
            }            
            return lstDetail;
        }
        //取计划流程的生产数
        public static decimal GetPlanProdQty(string out_dept,string mo_id, string goods_id)
        {
            string strSql = string.Format(
                @"Select Sum(Isnull(D.prod_qty,0)) As prod_qty From jo_bill_mostly M with(nolock)
                Inner join jo_bill_goods_details D with(nolock) ON M.within_code=D.within_code And M.id=D.id And M.ver=D.ver
                Where D.within_code ='{0}' And M.mo_id ='{1}' And M.state Not In('2','V') And D.goods_id ='{2}' And D.wp_id ='{3}'",
                within_code, mo_id, goods_id, out_dept);
            DataTable dt = new DataTable();
            dt = sh.ExecuteSqlReturnDataTable(strSql);
            decimal result = 0;
            if (dt.Rows.Count > 0)
            {
                result = decimal.Parse(dt.Rows[0]["prod_qty"].ToString());
            }
            return result;
        }

        //是否有移交特批权限
        public static string GetUserPope(string user_id)
        {
            string strSql = string.Format(
            @"DECLARE @is_pope char(1)
            SET @is_pope='0'
            DECLARE @user_id varchar(30)
            SET @user_id=''
            DECLARE @group_id varchar(30)
            SELECT @group_id=group_id FROM sys_user where user_id=@user_id

            SELECT @is_pope=B.C9_STATE FROM sys_user A,UPM_USER_POPEDOM B
            WHERE A.user_id=B.USR_NO AND A.user_id='{0}' AND B.Window_id='W_PRODUCE_ASSEMBLY' AND B.C9_ID='pb_confirm_rechange'

            IF(@is_pope='0')
            BEGIN
	            SET @user_id=@group_id
	            SELECT @is_pope=B.C9_STATE FROM sys_user A ,UPM_USER_POPEDOM B
	            WHERE A.user_id=B.USR_NO AND A.user_id=@user_id AND B.Window_id='W_PRODUCE_ASSEMBLY' AND B.C9_ID='pb_confirm_rechange'
            END
            SELECT @is_pope AS pope", user_id);
            DataTable dt = new DataTable();
            dt = sh.ExecuteSqlReturnDataTable(strSql);
            string result = "0";
            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["pope"].ToString();
            }
            return result;
        }

    }
}
