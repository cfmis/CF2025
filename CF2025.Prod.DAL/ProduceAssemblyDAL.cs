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
        public static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        public static PubFunDAL pubFunction = new PubFunDAL();
        public static string within_code = "0000";
        public static string ls_servername = "hkserver.cferp.dbo";
        public static string ldt_check_date = "";
        //private static string language_id = "1";
        //private static string sequence_id = "";

        public static jo_assembly_mostly GetHeadByID(string id)
        {
            jo_assembly_mostly mdjHead = new jo_assembly_mostly();
            string strSql = string.Format(
                @"Select id,Convert(nvarchar(10),con_date,121) As con_date,bill_origin,out_dept,in_dept,handover_id,handler,remark,
                create_by,create_date,update_by,update_date,check_by,Convert(nvarchar(19),check_date,121) As check_date,update_count,state
                FROM jo_assembly_mostly with(nolock) Where within_code='{0}' AND id='{1}'", within_code,id);
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
                @"SELECT A.id,A.upper_sequence,A.sequence_id,A.mo_id,A.goods_id,B.name AS goods_name,A.con_qty,A.unit_code,
                A.sec_qty,A.sec_unit,A.package_num,A.bom_qty,A.base_qty,Isnull(A.lot_no,'') As lot_no
                FROM dbo.jo_assembly_details_part A with(nolock)
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
                @"SELECT S.mo_id,S.goods_id,S.sequence_id,S.bom_qty,S.base_qty,S.goods_name
                FROM (SELECT Distinct a.mo_id,c.materiel_id as goods_id,c.bom_sequence as sequence_id,c.bom_qty,c.base_qty,d.name as goods_name,a.state	
                     FROM jo_bill_mostly a with(nolock)
	                    INNER JOIN jo_bill_goods_details b with(nolock) ON a.within_code=b.within_code AND a.id =b.id AND a.ver=b.ver
                        INNER JOIN jo_bill_materiel_details c with(nolock) ON b.within_code=c.within_code AND b.id=c.id AND b.ver=c.ver AND b.sequence_id=c.upper_sequence
	                    LEFT JOIN it_goods d ON c.within_code=d.within_code AND c.materiel_id=d.id 
                     WHERE a.within_code='{0}' AND a.mo_id='{1}' And b.goods_id='{2}' AND Isnull(b.hold,'')='' AND b.wp_id='{3}' AND b.next_wp_id='{4}') S
                WHERE S.state NOT IN('0','2','V') 
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
                mdjDetail.lot_no = "";
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
        //保存
        public static string Save(jo_assembly_mostly headData, List<jo_assembly_details> lstDetailData1, List<jo_assembly_details_part> lstDetailData2, 
                                  List<jo_assembly_details> lstDelData1, List<jo_assembly_details_part> lstDelData2, 
                                  List<jo_assembly_details> lstTurnOver, List<jo_assembly_details> lstTurnOverQc,string user_id )
        {
            string str = "", lot_no = "", max_handover_id = "";
            StringBuilder sbSql = new StringBuilder(" SET XACT_ABORT ON ");
            sbSql.Append(" BEGIN TRANSACTION ");
            string id = headData.id;
            string head_insert_status = headData.head_status;
            
            if (head_insert_status == "NEW")//全新的單據
            {
                bool id_exists = CommonDAL.CheckIdIsExists("jo_assembly_mostly", id);
                if (id_exists)
                {
                    //已存在此單據號,重新取最大單據號
                    headData.id = CommonDAL.GetMaxID("JO19", headData.out_dept, 5);//DDI1052201001
                }
                //***begin 更新系統表組裝轉換的最大單據編號
                string dept_id = headData.out_dept;//105
                string year_month = ""; //model.id.Substring(6, 4);//2201
                string bill_code = "";  //model.id.Substring(1, 13);DC105220800001
                year_month = headData.id.Substring(5, 4);   //2201
                bill_code = headData.id.Substring(1, 13);   //C105220800001
                string sql_sys_update1 = "";
                string sql_sys_id_insert = string.Format(
                    @" INSERT INTO sys_bill_max_separate(within_code,bill_code,bill_id,year_month,bill_text2,bill_text1,bill_text3,bill_text4,bill_text5) 
                    VALUES('0000','{0}','{1}','{2}','{3}','','','','')",
                    bill_code, "JO19", year_month, dept_id);
                string sql_sys_id_udate = string.Format(
                    @" UPDATE sys_bill_max_separate SET bill_code='{0}' WHERE within_code='0000' AND bill_id='{1}' AND year_month='{2}' and bill_text2='{3}'",
                    bill_code, "JO19", year_month, dept_id);
                if (bill_code.Substring(8, 5) != "00001")
                    sql_sys_update1 = sql_sys_id_udate;
                else
                    sql_sys_update1 = sql_sys_id_insert;

                //更新系統表移交單最大單號
                //取移交單最大單號
                string sql_f = string.Format(@"SELECT dbo.fn_zz_sys_bill_max_jo07('{0}','{1}','{2}') as id", headData.out_dept, headData.in_dept, "T");
                DataTable dt = sh.ExecuteSqlReturnDataTable(sql_f);
                max_handover_id = dt.Rows[0]["id"].ToString(); //max_handover_id value is DT10560134510
                string billCode = max_handover_id.Substring(1, 12);   //biiCode value is T10560134510
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

                str = sql_sys_update1 + sql_sys_update2;
                sbSql.Append(str);
                //***end save form max id to system tabe 

                //插入主表
                //if (!string.IsNullOrEmpty(headData.in_dept))
                //{
                //  headData.handover_id = max_handover_id;
                //}
                str = string.Format(
                  @" Insert Into jo_assembly_mostly(within_code,id,con_date,handler,update_count,transfers_state,remark,state,create_by,create_date,bill_type_no,
                  out_dept,in_dept,con_type,stock_type,bill_origin,handover_id,servername,update_date,update_by) 
                  Values ('{0}','{1}','{2}','{3}','1','0','{4}','0','{5}',getdate(),'C','{6}','{7}','0','0','2','{8}','{9}',getdate(),'{10}')",
                  within_code, headData.id, headData.con_date, headData.handler, headData.remark, headData.create_by, headData.out_dept, headData.in_dept,
                  headData.handover_id, ls_servername, headData.create_by);
                sbSql.Append(str);
                //插入明細表一
                foreach (var item in lstDetailData1)
                {                   
                    lot_no = CommonDAL.GetDeptLotNo(headData.out_dept, headData.in_dept); //自動生成批號
                    //改变list中某个元素值 (例子: var model = list.Where(c => c.ID == ).FirstOrDefault(); model.Value1 = ;)
                    //历遍移交單臨時數組,更改批號與組裝單批號一致
                    lstTurnOver.ForEach(i =>
                    {
                        if (i.id == item.id && i.sequence_id == item.sequence_id)
                        {
                            i.lot_no = lot_no;
                        }
                    });
                    //历遍QC移交單臨時數組,更改批號與組裝單批號一致
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
                    within_code, headData.id, item.sequence_id, item.jo_id, item.jo_sequence_id, item.goods_id, item.con_qty, item.unit_code, item.sec_qty, item.sec_unit,
                    item.mo_id, item.package_num, headData.out_dept, headData.out_dept, headData.create_by, lot_no, item.prd_id, item.qc_qty);
                    sbSql.Append(str);
                }
                //插入明細表二
                foreach (var item in lstDetailData2)
                {
                    str = string.Format(
                    @" Insert Into jo_assembly_details_part(within_code,id,upper_sequence,sequence_id,mo_id,goods_id,con_qty,unit_code,sec_qty,sec_unit,
                    package_num,remark,bom_qty,base_qty,lot_no) Values ('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}',{8},'{9}',{10},'{11}',{12},{13},'{14}')",
                    within_code, headData.id, item.upper_sequence, item.sequence_id, item.mo_id, item.goods_id, item.con_qty, item.unit_code, item.sec_qty,
                    item.sec_unit, item.package_num, item.remark, item.bom_qty, item.base_qty, item.lot_no);
                    sbSql.Append(str);
                }
                //生成移交單
                GenNumberDAL objNum = new GenNumberDAL();
                string update_count = "1", transfers_state = "0", bill_type_no = "T", con_type = "0", stock_type = "0", bill_origin = "2";
                string sequence_id = "", aim_jo_id = "", aim_jo_sequence = "";
                decimal jo_qty = 0, c_qty = 0, package_num = 0;
                int index;
                if (lstTurnOver.Count > 0)
                {
                    //移交單表頭                    
                    str = string.Format(
                    @" Insert Into jo_materiel_con_mostly(within_code,id,con_date,handler, update_count,transfers_state,remark,state,bill_type_no,out_dept,in_dept,
                    con_type,stock_type,bill_origin,create_by,create_date,servername,update_date,update_by) VALUES
                    ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',getdate(),'{15}',getdate(),'{16}')",
                    within_code, max_handover_id, headData.con_date, headData.handler, update_count, transfers_state, headData.remark, headData.state,
                    bill_type_no, headData.out_dept, headData.in_dept, con_type, stock_type, bill_origin, headData.create_by, ls_servername, headData.create_by);
                    sbSql.Append(str);
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
                        within_code, max_handover_id, sequence_id, item.jo_id, item.jo_sequence_id, item.goods_id, jo_qty, c_qty, item.con_qty, item.unit_code,
                        item.sec_qty, item.sec_unit, item.remark, item.mo_id, package_num, headData.out_dept, headData.out_dept, item.lot_no, aim_jo_id, aim_jo_sequence);
                        sbSql.Append(str);
                    }
                }
                //生成QC移交單
                string max_handover_id_qc = "";
                if (lstTurnOverQc.Count > 0)
                {
                    string in_dept = "702";
                    sql_f = string.Format(@"SELECT dbo.fn_zz_sys_bill_max_jo07('{0}','{1}','{2}') as id", headData.out_dept, in_dept, "T");
                    DataTable dtQc = sh.ExecuteSqlReturnDataTable(sql_f);
                    max_handover_id_qc = dtQc.Rows[0]["id"].ToString(); //max_handover_id_qc value is DT10570234510
                    billCode = max_handover_id_qc.Substring(1, 12); //biiCode value is T10570234510
                    strSql_i = string.Format(
                        @" INSERT INTO sys_bill_max_jo07(within_code,bill_id,year_month,bill_code,bill_text1,bill_text2,bill_text3,bill_text4,bill_text5) 
                        VALUES('0000','JO07','','{0}','T','{1}','{2}','','')", billCode, headData.out_dept, in_dept);
                    //更新系統表最大單據號
                    strSql_u = string.Format(
                        @" UPDATE sys_bill_max_jo07 SET bill_code='{0}'
                        WHERE within_code='0000' AND bill_id='JO07' AND bill_text1='T' AND bill_text2='{1}' and bill_text3='{2}'", billCode, headData.out_dept, in_dept);
                    sql_sys_update2 = billCode.Substring(7, 5) != "00001" ? strSql_u : strSql_i;
                    sbSql.Append(sql_sys_update2);
                    //生成QC移交單表頭
                    str = string.Format(
                    @" Insert Into jo_materiel_con_mostly(within_code,id,con_date,handler, update_count,transfers_state,remark,state,bill_type_no,out_dept,in_dept,
                    con_type,stock_type,bill_origin,create_by,create_date,servername,update_date,update_by)
                    VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',getdate(),'{15}',getdate(),'{16}')",
                    within_code, max_handover_id_qc, headData.con_date, headData.handler, update_count, transfers_state, headData.remark, headData.state,
                    bill_type_no, headData.out_dept, in_dept, con_type, stock_type, bill_origin, headData.create_by, ls_servername, headData.create_by);
                    sbSql.Append(str);
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
                        within_code, max_handover_id_qc, sequence_id, item.jo_id, item.jo_sequence_id, item.goods_id, jo_qty, c_qty, item.con_qty, item.unit_code,
                        item.sec_qty, item.sec_unit, item.remark, item.mo_id, package_num, headData.out_dept, headData.out_dept, item.lot_no, aim_jo_id, aim_jo_sequence);
                        sbSql.Append(str);
                    }
                }
            }
            else //已保存組裝單的基礎上進行增刪改
            {
                //首先處理刪除(表格一) 
                //注意加選項with(ROWLOCK),只有用到主鍵時才會用到行級鎖
                if (lstDelData1 != null)
                {
                    string assembly_id = "", assembly_seq_id = "";
                    foreach (var item in lstDelData1)
                    {
                        str = string.Format(
                            @" DELETE FROM jo_assembly_details with(ROWLOCK) Where within_code='0000' AND id='{0}' AND sequence_id='{1}'",
                            item.id, item.sequence_id);
                        sbSql.Append(str);
                        //刪除對應的移交單(包括QC移交單)
                        assembly_id = item.id;
                        assembly_seq_id = item.sequence_id;
                        str = string.Format(
                        @" DELETE FROM jo_materiel_con_details Where mo_id='{0}' And goods_id='{1}' And aim_jo_id='{2}' And aim_jo_sequence='{3}'",
                        item.mo_id, item.goods_id, assembly_id, assembly_seq_id);
                        sbSql.Append(str);
                    }
                }
                //首先處理刪除(表格二)
                if (lstDelData2 != null)
                {
                    foreach (var item in lstDelData2)
                    {
                        str = string.Format(
                        @" DELETE FROM jo_assembly_details_part with(ROWLOCK) WHERE within_code='0000' AND id='{0}' AND upper_sequence='{1}' AND sequence_id='{2}'",
                        item.id, item.upper_sequence, item.sequence_id);
                        sbSql.Append(str);
                    }
                }
                //更新組裝轉換表頭
                str = string.Format(
                @" UPDATE jo_assembly_mostly with(ROWLOCK) SET con_date='{2}',handler='{3}',remark='{4}',update_by='{5}',update_date=getdate(),out_dept='{6}',in_dept='{7}',
                    update_count= Convert(nvarchar(5),Convert(int,update_count)+1)
                WHERE within_code='{0}' AND id='{1}'",
                within_code, headData.id, headData.con_date, headData.handler, headData.remark, headData.update_by, headData.out_dept, headData.in_dept);
                sbSql.Append(str);
                //更新明細表一
                decimal ldc_con_qty = 0, ldc_sec_qty = 0;
                string strQcID = "";
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
                            //更新組裝轉換明細                            
                            str = string.Format(
                            @" UPDATE jo_assembly_details with(Rowlock) 
                            Set jo_id='{3}',jo_sequence_id='{4}',goods_id='{5}',con_qty={6},unit_code='{7}',sec_qty={8},sec_unit='{9}', mo_id='{10}', package_num={11},
                            location='{12}',carton_code='{13}',lot_no='{14}',prd_id={15},qc_qty={16}
                            WHERE within_code='{0}' AND id='{1}' AND sequence_id='{2}'",
                            within_code, headData.id, item.sequence_id, item.jo_id, item.jo_sequence_id, item.goods_id, item.con_qty, item.unit_code, item.sec_qty, item.sec_unit,
                            item.mo_id, item.package_num, item.location, item.carton_code, lot_no, item.prd_id, item.qc_qty);
                            sbSql.Append(str);
                            //更新移交單明細                                                        
                            if (!string.IsNullOrEmpty(headData.handover_id))
                            {
                                //已考慮扣減交QC部分
                                str = string.Format(
                                @" UPDATE jo_materiel_con_details 
                                SET jo_id='{6}',jo_sequence_id='{7}',con_qty={8},unit_code='{9}',sec_qty={10},sec_unit='{11}',remark='{12}',
                                package_num={13},location='{14}',carton_code='{15}',lot_no='{16}'
                                Where mo_id='{0}' And goods_id='{1}' And within_code='{2}' And id='{3}' And aim_jo_id='{4}' And aim_jo_sequence='{5}'",
                                item.mo_id, item.goods_id, within_code, headData.handover_id, item.id, item.sequence_id,
                                item.jo_id, item.jo_sequence_id, ldc_con_qty, item.unit_code, ldc_sec_qty, item.sec_unit, item.remark,
                                item.package_num, item.location, item.carton_code, lot_no);
                                sbSql.Append(str);
                            }
                        }
                        else //INSERT ITEM//有項目新增
                        {
                            //組裝轉換明細,插入新增的記錄                            
                            lot_no = CommonDAL.GetDeptLotNo(headData.out_dept, headData.in_dept); //生成批號
                            str = string.Format(
                            @" Insert Into jo_assembly_details
                            (within_code,id,sequence_id,jo_id,jo_sequence_id,goods_id,con_qty,unit_code,sec_qty,sec_unit,mo_id,package_num,location,carton_code,lot_no,prd_id) 
                            Values('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}',{8},'{9}','{10}',{11},'{12}','{13}','{14}','{15}')",
                            within_code, headData.id, item.sequence_id, item.jo_id, item.jo_sequence_id, item.goods_id, item.con_qty, item.unit_code, item.sec_qty, item.sec_unit,
                            item.mo_id, item.package_num, headData.out_dept, headData.out_dept, lot_no, item.prd_id);
                            sbSql.Append(str);
                            decimal jo_qty = 0, c_qty = 0;
                            string aim_jo_id = "", aim_jo_sequence = "";
                            if (!string.IsNullOrEmpty(headData.handover_id))//已保存有移交單號
                            {
                                //新增的記錄插入已存在的移交單中
                                aim_jo_id = item.id;//組裝單號
                                aim_jo_sequence = item.sequence_id;//組裝轉換單的序號
                                str = string.Format(
                                @" Insert Into jo_materiel_con_details
                                (within_code,id,sequence_id,jo_id,jo_sequence_id,goods_id,jo_qty,c_qty,con_qty,unit_code,sec_qty,sec_unit,remark,mo_id,package_num,
                                location,carton_code,lot_no,aim_jo_id,aim_jo_sequence) Values
                                ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},'{9}',{10},'{11}','{12}','{13}',{14},'{15}','{16}','{17}','{18}','{19}')",
                                within_code, headData.handover_id, item.sequence_id, item.jo_id, item.jo_sequence_id, item.goods_id, jo_qty, c_qty, ldc_con_qty, item.unit_code,
                                ldc_sec_qty, item.sec_unit, item.remark, item.mo_id, item.package_num, headData.out_dept, headData.out_dept, item.lot_no, aim_jo_id, aim_jo_sequence);
                                sbSql.Append(str);
                                //判斷是否有交QC移交單數據?
                                if (item.qc_qty > 0 && headData.in_dept != "702") //*&& item.con_qty > item.qc_qty*/
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
                                            //已保存有移交QC的原始數據,在原有QC移交單上插入記錄
                                            strQcID = dt.Rows[0]["id"].ToString();
                                            str = string.Format(
                                            @" Insert Into jo_materiel_con_details 
                                            (within_code,id,sequence_id,jo_id,jo_sequence_id,goods_id,jo_qty,c_qty,con_qty,unit_code,sec_qty,sec_unit,remark,mo_id,package_num,
                                            location,carton_code,lot_no,aim_jo_id,aim_jo_sequence) Values
                                            ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},'{9}',{10},'{11}','{12}','{13}',{14},'{15}','{16}','{17}','{18}','{19}')",
                                            within_code, strQcID, item.sequence_id, item.jo_id, item.jo_sequence_id, item.goods_id, jo_qty, c_qty, item.qc_qty, item.unit_code,
                                            0.01, item.sec_unit, item.remark, item.mo_id, 0, headData.out_dept, headData.out_dept, lot_no, aim_jo_id, aim_jo_sequence);
                                            sbSql.Append(str);
                                        }
                                        else
                                        {
                                            //重新生成QC移交單
                                            //更新系統表最大單據編號  
                                            string in_dept = "702", update_count = "1", transfers_state = "0", bill_type_no = "T", con_type = "0", stock_type = "0", bill_origin = "2";
                                            strQcID = string.Format(@"SELECT dbo.fn_zz_sys_bill_max_jo07('{0}','{1}','{2}') AS id", headData.out_dept, in_dept, "T");
                                            DataTable dtQc = sh.ExecuteSqlReturnDataTable(strQcID);
                                            strQcID = dtQc.Rows[0]["id"].ToString();    //max_id value is DT10570234510
                                            string billCode = strQcID.Substring(1, 12); //biiCode value is T10570234510
                                            string strSql_i = string.Format(
                                            @" INSERT INTO sys_bill_max_jo07(within_code,bill_id,year_month,bill_code,bill_text1,bill_text2,bill_text3,bill_text4,bill_text5) 
                                            VALUES('0000','JO07','','{0}','T','{1}','{2}','','')", billCode, headData.out_dept, in_dept);
                                            string strSql_u = string.Format(
                                            @" UPDATE sys_bill_max_jo07 SET bill_code='{0}' 
                                            WHERE within_code='0000' AND bill_id='JO07' AND bill_text1='T' AND bill_text2='{1}' and bill_text3='{2}'", billCode, headData.out_dept, in_dept);
                                            str = billCode.Substring(7, 5) != "00001" ? strSql_u : strSql_i;
                                            sbSql.Append(str);
                                            //QC移交單表頭
                                            str = string.Format(
                                            @" Insert Into jo_materiel_con_mostly(within_code,id,con_date,handler,update_count,transfers_state,remark,state,bill_type_no,out_dept,in_dept,
                                            con_type,stock_type,bill_origin,create_by,create_date,servername,update_date,update_by)
                                            VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',getdate(),'hkserver.cferp.dbo',getdate(),'{15}')",
                                            within_code, strQcID, headData.con_date, headData.handler, update_count, transfers_state, headData.remark, headData.state,
                                            bill_type_no, headData.out_dept, in_dept, con_type, stock_type, bill_origin, headData.create_by, headData.create_by);
                                            sbSql.Append(str);
                                            //QC移交單明細 
                                            aim_jo_id = item.id;//組裝單號
                                            aim_jo_sequence = item.sequence_id;//組裝單序號
                                            str = string.Format(
                                            @" Insert Into jo_materiel_con_details
                                            (within_code,id,sequence_id,jo_id,jo_sequence_id,goods_id,jo_qty,c_qty,con_qty,unit_code,sec_qty,sec_unit,remark,mo_id,package_num,
                                            location,carton_code,lot_no,aim_jo_id,aim_jo_sequence) Values
                                            ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},'{9}',{10},'{11}','{12}','{13}',{14},'{15}','{16}','{17}','{18}','{19}')",
                                            within_code, strQcID, item.sequence_id, item.jo_id, item.jo_sequence_id, item.goods_id, jo_qty, c_qty, item.qc_qty, item.unit_code, 0.01,
                                            item.sec_unit, item.remark, item.mo_id, 0, headData.out_dept, headData.out_dept, lot_no, aim_jo_id, aim_jo_sequence);
                                            sbSql.Append(str);
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
                                        within_code, strQcID, item.sequence_id, item.jo_id, item.jo_sequence_id, item.goods_id, jo_qty, c_qty, item.qc_qty, item.unit_code, 0.01,
                                        item.sec_unit, item.remark, item.mo_id, 0, headData.out_dept, headData.out_dept, lot_no, aim_jo_id, aim_jo_sequence);
                                        sbSql.Append(str);
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
                            remark='{11}',bom_qty={12},base_qty={13},lot_no='{14}' 
                            WHERE within_code='{0}' AND id='{1}' AND upper_sequence='{2}' AND sequence_id='{3}'",
                            within_code, headData.id, item.upper_sequence, item.sequence_id, item.mo_id, item.goods_id, item.con_qty, item.unit_code, item.sec_qty, item.sec_unit,
                            item.package_num, item.remark, item.bom_qty, item.base_qty, item.lot_no);
                        }
                        else
                        {
                            str = string.Format(
                             @" Insert Into jo_assembly_details_part(within_code,id,upper_sequence,sequence_id,mo_id,goods_id,con_qty,unit_code,sec_qty,sec_unit,package_num,
                             remark,bom_qty,base_qty,lot_no) Values ('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}',{8},'{9}',{10},'{11}',{12},{13},'{14}')",
                             within_code, headData.id, item.upper_sequence, item.sequence_id, item.mo_id, item.goods_id, item.con_qty, item.unit_code, item.sec_qty, item.sec_unit,
                             item.package_num, item.remark, item.bom_qty, item.base_qty, item.lot_no);
                        }
                        sbSql.Append(str);
                    }
                }
            }
            sbSql.Append(@" COMMIT TRANSACTION ");
            string result = sh.ExecuteSqlUpdate(sbSql.ToString());
            sbSql.Clear();
            
            //保存成功之后進行批準操作           
            //更新PAD生產嗙貨給組裝轉換已入單數據標識
            if (result == "")
            {
                //--start更新PAD生產嗙貨給組裝轉換已入單標識
                string sql = string.Format(
                @"Select Count(1) As cnt 
                From jo_assembly_mostly A WITH(NOLOCK),jo_assembly_details B WITH(NOLOCK),product_records C WITH(NOLOCK)
                Where A.within_code=B.within_code And A.id=B.id And B.prd_id=C.prd_id And Isnull(C.imput_flag,'')='' And
                      A.within_code='{0}' And A.id='{1}'", within_code, headData.id);
                DataTable dt = sh.ExecuteSqlReturnDataTable(sql);
                if (int.Parse(dt.Rows[0]["cnt"].ToString()) > 0)
                {                    
                    sbSql.Clear();
                    sbSql.Append(@" SET XACT_ABORT ON ");
                    sbSql.Append(@" BEGIN TRANSACTION ");                                            
                    sql = string.Format(
                    @" Update C WITH(ROWLOCK) Set C.imput_flag ='Y',C.imput_time=GetDate()        
                    From jo_assembly_mostly A,jo_assembly_details B,product_records C        
                    Where A.within_code = B.within_code And A.id = B.id And B.prd_id = C.prd_id        
                            And Isnull(C.imput_flag,'')='' And A.within_code='{0}' And A.id ='{1}'", within_code, headData.id);
                    sbSql.Append(sql);
                    sbSql.Append(@" COMMIT TRANSACTION ");
                    result = sh.ExecuteSqlUpdate(sbSql.ToString());
                    sbSql.Clear();
                }
                //--end 更新PAD生產嗙貨給組裝轉換已入單數據標識

                //--start 批準組裝轉換單,移交單
                //因前面生成的移交單的并沒有交易數據,還是未批準狀態,還需單獨批準對應的移交單,并生成交易和庫存
                //批準組裝轉換單,批準移交單,生成組裝轉換單,移交單的交易信息,庫存信息,更新批準狀態
                if (result == "")
                {                    
                    if (head_insert_status == "NEW")//新的組裝轉換單
                    {                        
                        headData.handover_id = max_handover_id;
                    }
                    //批準組裝轉換單,移交單
                    result = Approve(headData, user_id, "1");//批準
                } 
                else
                {
                    result = "-1" + result;
                }
                //--end 批準移交單

            }// end 保存成功
            return result;
        }
        
        /// <summary>
        /// 批準移交單,更改移交單的批準狀態,交易的生成,庫存的更改
        /// </summary>
        /// <param name="headData"></param>
        /// <param name="handover_id"></param>
        /// <param name="check_date"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public static string ApproveRechange(jo_assembly_mostly headData,string handover_id,string check_date, string user_id)
        {
            string result = "00",str="";
            string handover_id1 = "", handover_id2 = "";
            if (!string.IsNullOrEmpty(handover_id))
            {
                handover_id1 = handover_id.Substring(0, 8);  //DT105601
                handover_id2 = handover_id.Substring(0, 5) + "702";  //DT105702
            }
            else
            {
                result = "00";
                return result;
            }                      
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(@" SET XACT_ABORT ON ");
            sbSql.Append(@" BEGIN TRANSACTION ");
            str = string.Format(
            @"SELECT Distinct S.id 
                    FROM (select a.id from jo_materiel_con_mostly a with(nolock),jo_materiel_con_details b with(nolock) 
                            where a.within_code=b.within_code and a.id=b.id and a.within_code='{0}' and a.id like '{1}%' and a.con_date='{3}' and b.aim_jo_id='{4}'
	                        union all
	                        select a.id from jo_materiel_con_mostly a with(nolock),jo_materiel_con_details b with(nolock) 
                            where a.within_code=b.within_code and a.id=b.id and a.within_code='{0}' and a.id like '{2}%' and a.con_date='{3}' and b.aim_jo_id='{4}'
	                        ) S", within_code, handover_id1, handover_id2, headData.con_date, headData.id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(str);
            if (dt.Rows.Count > 0)
            {
                result = "00";
                string window_id = "w_produce_assembly";
                //循環生成移交單交易及更新移交單相關庫存
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    result = SetRechangeStBusiness(dt.Rows[i]["id"].ToString(), check_date, "pfc_ok", "JO", user_id, window_id);
                    if (result.Substring(0, 2) == "-1")
                    {
                        break;//失敗,退出循環
                    }
                    else
                    {
                        //成功,更新移交單批準狀態 
                        str = string.Format(
                                @" Update jo_materiel_con_mostly with(Rowlock) SET check_date='{2}',check_by='{3}',state='{4}' WHERE within_code='{0}' and id='{1}'",
                                within_code, dt.Rows[i]["id"].ToString(), check_date, user_id, "1");
                        sbSql.Append(str);
                    }
                }

                if (result.Substring(0, 2) == "00") // SetRechangeStBusiness()執行成功
                {
                    if (headData.in_dept == "702")
                    {
                        /*以下情況有可能發生,即組裝一張交QC(702)的組裝轉換單,
                            *因組裝單批準的模塊中更新jo_assembly_mostly的批準信息時過慮掉702
                            *所以這里要單獨更新
                        */
                        str = string.Format(
                        @" Update jo_assembly_mostly with(Rowlock) 
                                    SET check_date='{2}',check_by='{3}',state='{4}',handover_id='{5}'
                                    WHERE within_code='{0}' and id='{1}'",
                            within_code, headData.id, check_date, user_id, "1", handover_id1);
                        sbSql.Append(str);
                    }
                    //提交移交單批準人,批準日期"
                    sbSql.Append(" COMMIT TRANSACTION ");
                    result = sh.ExecuteSqlUpdate(sbSql.ToString());
                    sbSql.Clear();
                    if (result == "")
                    {
                        result = "00";
                    }else
                    {
                        result = "-1" + result;
                    }
                }
            }            

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
            string strSql = string.Format(@"Select value From sy_parameters_setup WITH (NOLOCK) Where sequence_id='{0}' And within_code='0000'", id);
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
                @"Select Count(1) As rs From jo_bill_mostly A with(nolock),jo_bill_goods_details B with(nolock)
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
        public static decimal GetQcQty(string mo_id, string goods_id, string wp_id)
        {
            decimal result = 0;
            string strSql = string.Format(
            @"Select B.prod_qty From jo_bill_mostly A with(nolock),jo_bill_goods_details B with(nolock)
            Where A.within_code=B.within_code And A.id=B.id And A.ver=B.ver And A.state Not In('2','V')
              And B.next_wp_id='702' And Isnull(B.c_qty_ok, 0) = 0 And B.within_code='0000'
              And A.mo_id='{0}' And B.goods_id='{1}' And B.wp_id='{2}'", mo_id, goods_id, wp_id);
            DataTable dt = new DataTable();
            dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                result = decimal.Parse(dt.Rows[0]["prod_qty"].ToString());
            }            
            return result;
        }
        //--已组装的数量        
        public static List<assembly_qty> GetAssemblyQty(string id,string mo_id, string goods_id, string out_dept) 
        {
            string strSql = string.Format(
                @"Select Isnull(Sum(D.con_qty),0) As con_qty,Isnull(Sum(D.sec_qty),0) As sec_qty 
                From jo_assembly_mostly M with(nolock),jo_assembly_details D with(nolock)
		        Where M.within_code=D.within_code And M.id=D.id And M.state Not In('2','V') And D.within_code='{0}'
				And D.id <>'{1}' And D.mo_id='{2}' And D.goods_id='{3}' And M.out_dept='{4}'",
                within_code,id,mo_id,goods_id,out_dept);
            DataTable dt = new DataTable();
            dt = sh.ExecuteSqlReturnDataTable(strSql);           
            List<assembly_qty> lstDetail = new List<assembly_qty>();
            if (dt.Rows.Count > 0)
            {
                assembly_qty lst = new assembly_qty();
                lst.con_qty = decimal.Parse(dt.Rows[0]["con_qty"].ToString());
                lst.sec_qty = decimal.Parse(dt.Rows[0]["sec_qty"].ToString());
                lstDetail.Add(lst);
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
        
        /// <summary>
        /// 批準/反批準,生成組裝單,移交單的交易,庫存更新等(失敗返回的字串前兩位是"-1")
        /// </summary>
        /// <param name="id">組裝單號</param>        
        /// <param name="user_id">當前用戶</param>
        /// <param name="approve_type">approve_type:1--批準;0--反批準</param>
        /// <returns>返回字串為空,表示成功</returns>        
        public static string Approve(jo_assembly_mostly head,string user_id, string approve_type)
        {
            string result = "", return_value = "", strSql = "";
            StringBuilder sbSql = new StringBuilder();
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
                sbSql.Clear();
                sbSql.Append(" SET XACT_ABORT ON ");
                sbSql.Append(" BEGIN TRANSACTION ");
                sbSql.Append(strSql);
                sbSql.Append(@" COMMIT TRANSACTION ");
                result = sh.ExecuteSqlUpdate(sbSql.ToString());
                sbSql.Clear();
                if (result == "")
                {
                    result = "00";//成功
                }
                else
                { 
                    result ="-1"+ result+ "\r\n"+ "交易数据保存失败!" + "\r\n" + "<" + active_name + ">(st_business_record)" + "\r\n";
                    return result;
                }
                //更新库存
                return_value = pubFunction.of_update_st_details("I", "31", head.id, "*", ldt_check_date, "");               
                if (return_value.Substring(0,2) == "-1")
                {
                    result = return_value + "\r\n"+ "庫存數據保存失败![扣除原料库存]+" + "\r\n" + "<" + active_name + ">(st_details)" + "\r\n";
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
                      A.within_code=C.within_code And A.out_dept=C.id And a.id='{1}' and isNull(C.location,'')<>'' ",within_code, head.id);
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
                    within_code, head.id, user_id,ldt_check_date, ls_servername);
                    sbSql.Clear();
                    sbSql.Append(" SET XACT_ABORT ON ");
                    sbSql.Append(" BEGIN TRANSACTION ");
                    sbSql.Append(strSql);
                    sbSql.Append(@" COMMIT TRANSACTION ");
                    result = sh.ExecuteSqlUpdate(sbSql.ToString());
                    sbSql.Clear();
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
                    if (return_value.Substring(0,2) == "-1")
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
                within_code, head.id, user_id,ldt_check_date,ls_servername);
                sbSql.Clear();
                sbSql.Append(" SET XACT_ABORT ON ");
                sbSql.Append(" BEGIN TRANSACTION ");
                sbSql.Append(strSql);
                sbSql.Append(@" COMMIT TRANSACTION ");
                result = sh.ExecuteSqlUpdate(sbSql.ToString());
                sbSql.Clear();
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
                if (return_value.Substring(0,2) == "-1")
                {
                    //数据保存失败
                    result = return_value + "\r\n" + "数据保存失败!<批準>((st_details)" ;
                    return result;
                }
                //--end action_id:29               
                //--如果主表中有选择收货部门就自动生成相应的移交单数据
                //If wf_generate_turn_over() = -1 Then Return - 1
               
                //批準移交單(其中包含有調用生成移交單交易、更新移交單相關庫存的方法)
                result = ApproveRechange(head, head.handover_id, ldt_check_date, user_id);

            } //--end 批準

            //--start 反批準組裝轉換單
            if (approve_type == "0")
            {                
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
                    if (return_value.Substring(0,2) == "-1")
                    {
                        result = return_value + "数据保存失败![D,29]" + "\r\n" + "<" + active_name + ">(st_details)" + "\r\n";
                        return result;
                    }
                    //更新库存
                    return_value = pubFunction.of_update_st_details("D", "31", head.id, "*", ldt_check_date, "");
                    if (return_value.Substring(0,2) == "-1")
                    {
                        result = return_value + "\r\n" + "数据保存失败![D,31]" + "\r\n" + "<" + active_name + ">(st_details)" + "\r\n";
                        return result;
                    }
                    //---
                    ll_cnt = 0;
                    strSql = string.Format(
                    @"Select Count(1) as cnt FROM st_business_record WITH(NOLOCK) Where within_code='{0}' and id='{1}' And action_id='51'",within_code,head.id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    ll_cnt = int.Parse(dt.Rows[0]["cnt"].ToString());
                    if (ll_cnt > 0)
                    {
                        //更新库存
                        return_value = pubFunction.of_update_st_details("D", "51", head.id, "*", ldt_check_date, "");
                        if (return_value.Substring(0,2) == "-1")
                        {
                            result = return_value + "\r\n"+"数据保存失败![D,51]" + "\r\n" + "<" + active_name + ">(st_details)" + "\r\n";
                            return result;
                        }
                    }
                    //---
                    //刪除相并交易
                    strSql = string.Format(
                        @" Delete FROM st_business_record WITH(ROWLOCK) Where within_code='{0}' and id='{1}' And action_id In('31','51','29')",within_code, head.id);
                    sbSql.Clear();
                    sbSql.Append(" SET XACT_ABORT ON ");
                    sbSql.Append(" BEGIN TRANSACTION ");
                    sbSql.Append(strSql);
                    sbSql.Append(@" COMMIT TRANSACTION ");
                    result = sh.ExecuteSqlUpdate(sbSql.ToString());
                    sbSql.Clear();
                    if (result == "")
                    {
                        result = "00";
                    }
                    else {
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
        
        
        /// <summary>
        /// 反批準
        /// </summary>
        /// <param name="id">組裝單號</param>
        /// <param name="handover_id">移交單號</param>
        /// <param name="user_id">當前用戶</param>        
        /// <returns>返回字串為空,表示成功</returns>        
        public static string UnApprove(jo_assembly_mostly head, string handover_id, string user_id,string check_date)
        {
            string result = "00";
            //檢查是否存在相關的移交單已批準,不允許反批準組裝轉換單(考慮多用戶操作環境)handover_id
            if (handover_id != "")
            {
                //查包含的全部移交單是否有已批準的
                if(CheckRechangeStatus(within_code, head.handover_id, head.con_date, head.id, "1") == "1")
                {
                    result = "-1" + "注意:對應的移交單[" + handover_id + "]已批準,當前操作無效!!";
                    return result;
                }                
                //strSql = string.Format(
                //    @"select [state] from jo_materiel_con_mostly with(nolock) where within_code='{0}' and id='{1}' AND [state]='1'", within_code, handover_id);
                //DataTable dt = new DataTable();
                //dt = sh.ExecuteSqlReturnDataTable(strSql);
                //if (dt.Rows.Count > 0)
                //{
                //    result ="-1"+ "注意:對應的移交單[" + handover_id + "]已批準,當前操作無效!!";
                //    return result;
                //}
            }            
            ldt_check_date = check_date;//取組裝轉換單中的批準日期
            //執行反批準
            result = Approve(head, user_id, "0"); //反批準組裝轉換單,與移交單無關,因相關移交單已在另一功能模塊中反批準.

            if (result.Substring(0,2) =="-1")
            {
                result = result + "反批準操作失敗!";
                return result;
            }
            //清空批準相關信息           
            string strSql = string.Format(
                @"Update jo_assembly_mostly with(Rowlock) SET check_date=null,check_by='',state='{2}',update_by='{3}',update_date=getdate()
                  WHERE within_code='{0}' and id='{1}'", within_code, head.id, "0",user_id );           
            result = sh.ExecuteSqlUpdate(strSql);
            result = (result == "") ? "00" : "-1" + result + "反批準失败![" + head.id + "]";  //"00"表示成功

            return result;
        }


        /// <summary>
        /// 生成移交單交易數據(返回值為字符串,字串前兩位:-1代表失敗;00:成功)
        /// </summary>
        /// <param name="as_id">移交單據編號</param>
        /// <param name="as_check_date">當前批準日期時間</param>
        /// <param name="as_active_name">pfc_ok/pfc_unok</param>
        /// <param name="as_bill_type">"JO:移交單"</param>
        /// <param name="as_user_id"></param>
        /// <returns></returns>
        public static string SetRechangeStBusiness(string as_id,string as_check_date, string as_active_name, string as_bill_type,string as_user_id,string as_window_id)
        {
            string strStatus = "00"; //strStatus字符串前兩位:-1代表失敗;00:成功
            string result = "", return_value = "";        
           
            StringBuilder sbSql = new StringBuilder(" SET XACT_ABORT ON ");
            sbSql.Append(" BEGIN TRANSACTION ");

            string ls_dept_id = "", ls_out_dept = "", ls_aim_jo_id = "",  ls_goods_id = "", ls_bill_type_no = "", ls_mo_id = "";
            string ls_con_date = "", ls_check_date = "", ls_bill_origin = "", ls_stock_type = "", ls_aim_jo_sequence = "",ls_error="";
            decimal ldec_con_qty = 0, ldec_sec_qty = 0;
            int li_cnt = 0, li_cnt_state1 = 0, li_cnt_state3 = 0, li_cnt_state4 = 0;
            ls_check_date = as_check_date;
            //反批准时清空日期变量
            if (as_active_name == "pfc_unok")
            {
                ls_check_date = "";  //**** 更新要注意NULL轉換  *********
            }
            //******************
            //是否存在移交單
            string strSql = string.Format(
                @"SELECT a.bill_type_no,ISNULL(a.bill_origin,'') as bill_origin,ISNULL(a.stock_type, '') as stock_type, a.out_dept,
                a.in_dept,Convert(varchar(10),a.con_date,121) as con_date
                FROM jo_materiel_con_mostly a WITH(NOLOCK)
                WHERE a.within_code='{0}' AND a.id ='{1}' AND a.state <>'2'", within_code,as_id);
            DataTable dt = new DataTable();
            dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count < 1)
            {
                strStatus = "00";               
                return strStatus;//沒有移交單直接返回
            }
            ls_bill_type_no = dt.Rows[0]["bill_type_no"].ToString();
            ls_bill_origin = dt.Rows[0]["bill_origin"].ToString();
            ls_stock_type = dt.Rows[0]["stock_type"].ToString();
            ls_out_dept = dt.Rows[0]["out_dept"].ToString();
            ls_dept_id = dt.Rows[0]["in_dept"].ToString();
            ls_con_date = dt.Rows[0]["con_date"].ToString();
            //******************

            //as_bill_type = "JO" 為移交單;
            if (as_bill_type == "JO")
            {
                //理论上要求同一张移交单需要使用同一张组装转换单的数据,所以只以第一笔数据的组装单号为准
                strSql = string.Format(
                    @"SELECT TOP 1 ISNULL(aim_jo_id,'') As aim_jo_id FROM jo_materiel_con_details WITH(NOLOCK) WHERE within_code='{0}' AND id ='{1}'",
                    within_code, as_id);
                dt = sh.ExecuteSqlReturnDataTable(strSql);
                if (dt.Rows.Count > 0)
                {
                    ls_aim_jo_id = dt.Rows[0]["aim_jo_id"].ToString();
                }
                ls_aim_jo_id = string.IsNullOrEmpty(ls_aim_jo_id) ? "" : ls_aim_jo_id;

                //更新组装单中的移交单号字段,批準/反批準信息
                //有702通常都有另一長正常的移交單,因正常移交單已更新组装单中的批準/反批準信息,所以批準702的移交單時不需再重復更新组装单中的批準信息
                if (ls_dept_id != "702")
                {
                    if (ls_aim_jo_id != "")
                    {
                        strSql = string.Format(
                            @" UPDATE A WITH(ROWLOCK) 
                            SET A.handover_id='{2}',
                                A.check_date = (Case When '{3}'='pfc_ok' Then '{4}' Else null End),
                                A.check_by = (Case When '{3}'='pfc_ok' Then '{5}' Else null End),
                                A.state = (Case When '{3}'='pfc_ok' Then '1' Else  '0' End)
                            FROM jo_assembly_mostly A
                            WHERE A.within_code='{0}' And A.id='{1}'", // AND Isnull(A.handover_id,'')=''
                            within_code, ls_aim_jo_id, as_id, as_active_name, ls_check_date,as_user_id);
                        sbSql.Append(strSql);
                    }
                }
            }//--end if (as_bill_type == "JO")

            //start as_active_name == "pfc_ok" || as_active_name == "pfc_unok"
            if (as_active_name == "pfc_ok" || as_active_name == "pfc_unok")
            {
                if (as_bill_type == "JO") //有生產計劃流程情况的处理
                {
                    strSql = string.Format(
                        @"Select b.goods_id,b.con_qty,b.sec_qty,Isnull(b.aim_jo_id,'') as aim_jo_id,Isnull(b.aim_jo_sequence,'') as aim_jo_sequence,a.in_dept,a.out_dept,b.mo_id
                        From jo_materiel_con_mostly a with(nolock)
                        Inner Join jo_materiel_con_details b with(nolock) On a.within_code = b.within_code And a.id = b.id
                        Where a.within_code='{0}' And a.id ='{1}'", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ls_goods_id = dt.Rows[i]["goods_id"].ToString();
                        ls_aim_jo_id = dt.Rows[i]["aim_jo_id"].ToString();
                        ls_aim_jo_sequence = dt.Rows[i]["aim_jo_sequence"].ToString();
                        ls_dept_id = dt.Rows[i]["in_dept"].ToString();
                        ls_out_dept = dt.Rows[i]["out_dept"].ToString();
                        ls_mo_id = dt.Rows[i]["mo_id"].ToString();
                        ldec_con_qty = pubFunction.checkDecimal(dt.Rows[i]["con_qty"].ToString());
                        ldec_sec_qty = pubFunction.checkDecimal(dt.Rows[i]["sec_qty"].ToString());
                        if (as_active_name == "pfc_unok")
                        {
                            ldec_con_qty = -ldec_con_qty;
                            ldec_sec_qty = -ldec_sec_qty;
                        }
                        //--更新组装明细资料中批準/反批準相關字段信息
                        if (ls_aim_jo_id != "") //加此判斷因電鍍絕大多沒有組裝單,直接開移交單,ls_aim_jo_id為空
                        {
                            strSql = string.Format(
                            @" Update A WITH(ROWLOCK)
                                Set A.signfor = (Case When '{3}'='pfc_ok' Then '1' Else '' End),
							        A.sign_by = (Case When '{3}'='pfc_ok' Then '{4}' Else '' End),
							        A.sign_date = (Case When '{3}'='pfc_ok' Then '{5}' Else null End)
                                From jo_assembly_details A
                                Where A.within_code='{0}' And A.id='{1}' And A.sequence_id='{2}'", 
                                within_code, ls_aim_jo_id, ls_aim_jo_sequence, as_active_name, as_user_id, ls_check_date);
                            sbSql.Append(strSql);
                        }
                        
                        // 20150922 (P528)修改到签收时处理
                        //更新累計移交數量及最后移交日期predept_rechange_date,predept_rechange_qty
                        li_cnt = 0;
                        strSql = string.Format(
                        @"Select count(1) as cnt
                        From jo_bill_mostly M with(NOLOCK)
                            Inner Join jo_bill_goods_details D WITH(NOLOCK) On M.within_code = D.within_code And M.id = D.id And M.ver = D.ver
                            Inner Join jo_bill_materiel_details DD WITH(NOLOCK) On D.within_code=DD.within_code And D.id=DD.id And D.ver=DD.ver And D.sequence_id=DD.upper_sequence
                        Where M.within_code ='{0}' And M.mo_id ='{1}' And M.state <>'2' And D.wp_id ='{2}' And DD.materiel_id ='{3}'",
                        within_code, ls_mo_id, ls_dept_id, ls_goods_id);
                        dt = sh.ExecuteSqlReturnDataTable(strSql);
                        li_cnt = dt.Rows.Count;
                        if (li_cnt > 0)
                        {                            
                            strSql = string.Format(
                                @" Update D WITH(ROWLOCK)        
                                SET D.predept_rechange_qty = Isnull(D.predept_rechange_qty, 0) + IsNull({4}, 0),
                                    D.predept_rechange_sec_qty = Isnull(D.predept_rechange_sec_qty, 0) + IsNull({5}, 0)        
                                From jo_bill_mostly M
                                     Inner Join jo_bill_goods_details D On M.within_code = D.within_code And M.id = D.id And M.ver = D.ver        
                                     Inner Join jo_bill_materiel_details DD On D.within_code=DD.within_code And D.id=DD.id And D.ver=DD.ver And D.sequence_id=DD.upper_sequence        
                                Where M.within_code='{0}' And M.mo_id='{1}' And M.state<>'2' And D.wp_id='{2}' And DD.materiel_id='{3}' ",
                                within_code, ls_mo_id, ls_dept_id, ls_goods_id, ldec_con_qty, ldec_sec_qty);
                            sbSql.Append(strSql);
                            //START	HXL	2013-10-22	P452
                            strSql = string.Format(
                            @" Update DD WITH(ROWLOCK)        
                               SET DD.i_qty = Isnull(DD.i_qty,0) + {4}, DD.i_sec_qty = Isnull(DD.i_sec_qty,0) + {5}       
                               From jo_bill_mostly M
                                    Inner Join jo_bill_goods_details D On M.within_code = D.within_code And M.id = D.id And M.ver = D.ver        
                                    Inner Join jo_bill_materiel_details DD On D.within_code=DD.within_code And D.id=DD.id And D.ver=DD.ver And D.sequence_id=DD.upper_sequence        
                               Where M.within_code='{0}' And M.mo_id='{1}' AND M.state<>'2' And D.wp_id='{2}' And DD.materiel_id='{3}' And Isnull(DD.location,'')=''",
                             within_code, ls_mo_id, ls_dept_id, ls_goods_id, ldec_con_qty, ldec_sec_qty);
                            //End	HXL	2013-10-22	P452
                            sbSql.Append(strSql);                         
                        }

                        //更新完成數量:c_qty_ok,c_sec_qty_ok,移交下部门日期:f_complete_date
                        li_cnt = 0;
                        strSql = string.Format(
                           @"Select Count(1) as cnt 
                            From jo_bill_mostly M WITH (NOLOCK)
                                Inner Join jo_bill_goods_details D WITH(NOLOCK) On M.within_code=D.within_code And M.id=D.id And M.ver=D.ver
                            Where M.within_code='{0}' And M.mo_id='{1}' And M.state<>'2' And D.goods_id='{2}' And D.wp_id='{3}' And D.next_wp_id='{4}'", 
                           within_code, ls_mo_id, ls_goods_id, ls_out_dept, ls_dept_id);
                        dt = sh.ExecuteSqlReturnDataTable(strSql);
                        li_cnt = int.Parse(dt.Rows[0]["cnt"].ToString());
                        //--start 流程是否存在
                        if (li_cnt > 0)
                        {
                            strSql = string.Format(
                            @" Update D WITH(ROWLOCK)
                            SET D.c_qty_ok = Isnull(D.c_qty_ok,0) + {5},
                                D.c_sec_qty_ok=Isnull(D.c_sec_qty_ok,0) + {6},
                                D.c_qty = Isnull(D.c_qty, 0) + {5},
                                D.c_sec_qty = Isnull(D.c_sec_qty,0) + {6}
                            From jo_bill_mostly M Inner Join jo_bill_goods_details D On M.within_code = D.within_code And M.id = D.id And M.ver = D.ver
                            Where M.within_code='{0}' And M.mo_id='{1}' And M.state<>'2' And D.goods_id='{2}' And D.wp_id='{3}' And D.next_wp_id ='{4}' ",
                            within_code, ls_mo_id, ls_goods_id, ls_out_dept, ls_dept_id, ldec_con_qty, ldec_sec_qty);
                            sbSql.Append(strSql);
                            //20130813 从触发器中移过来,更新流程完成狀態,'4'完成,'3'未完成, state='G'強制完成
                            strSql = string.Format(
                            @" Update D WITH(ROWLOCK)
                            SET D.goods_state=(Case When (Isnull(D.prod_qty,0)-Isnull(D.c_qty,0)<=0) Then '4' Else (Case When Isnull(D.c_qty,0)>0 Then '3' Else '1' End) End) 
                            From jo_bill_mostly M Inner Join jo_bill_goods_details D On M.within_code = D.within_code And M.id = D.id And M.ver = D.ver
                            Where M.within_code='{0}' And M.mo_id ='{1}' And M.state<>'2' And d.goods_id='{2}' And D.wp_id='{3}' And D.next_wp_id ='{4}' ",
                            within_code, ls_mo_id, ls_goods_id, ls_out_dept, ls_dept_id);
                            sbSql.Append(strSql);
                           
                            //=====start 20130813 从触发器中移过来
                            li_cnt = 0;
                            li_cnt_state1 = 0;
                            li_cnt_state3 = 0;
                            li_cnt_state4 = 0;
                            //--                            
                            strSql = string.Format(
                            @" Select Sum(1) as cnt,
                                  Sum(Case When Isnull(D.goods_state, '0') = '1' Then 1 Else 0 End) as cnt_state1,
                                  Sum(Case When Isnull(D.goods_state, '0') = '3' Then 1 Else 0 End) as cnt_state3,
                                  Sum(Case When Isnull(D.goods_state, '0') = '4' Then 1 Else 0 End) as cnt_state4
                             From jo_bill_mostly M WITH (NOLOCK) Inner Join jo_bill_goods_details D WITH(NOLOCK) On M.within_code=D.within_code And M.id=D.id And M.ver=D.ver
                             Where  M.within_code='{0}' And M.mo_id='{1}' AND M.state<>'2'", within_code, ls_mo_id);
                            dt = sh.ExecuteSqlReturnDataTable(strSql);
                            li_cnt = pubFunction.checkInt(dt.Rows[0]["cnt"].ToString());                            
                            li_cnt_state1 = pubFunction.checkInt(dt.Rows[0]["cnt_state1"].ToString());
                            li_cnt_state3 = pubFunction.checkInt(dt.Rows[0]["cnt_state3"].ToString());
                            li_cnt_state4 = pubFunction.checkInt(dt.Rows[0]["cnt_state4"].ToString());
                            
                            if (li_cnt > 0) //如果明细有生产中的数据,主表的状态也要为生产中
                            {
                                strSql = string.Format(
                                @" Update M WITH(ROWLOCK)
                                Set M.state=(Case When({2} > 0) Then '3' 
                                                  When({3} = {4}) Then '4' 
                                                  When({3} = {5}) Then '1'
                                                  Else M.state 
                                            End)
						        From jo_bill_mostly M
                                Where M.within_code='{0}' And M.mo_id='{1}' And M.state Not In('2','V','3')", 
                                within_code, ls_mo_id, li_cnt_state3, li_cnt, li_cnt_state4, li_cnt_state1 );//And M.state<>'3'
                                sbSql.Append(strSql);                              
                            }

                            //--清除预留库存
                            li_cnt = 0;
                            //--start 20131107
                            strSql = string.Format(
                                @"Select Count(1) as cnt
                                From jo_bill_materiel_details A WITH (NOLOCK),
                                     mrp_st_details_lot B WITH(NOLOCK),
                                     jo_bill_mostly C WITH(NOLOCK),
                                     jo_bill_goods_details D WITH(NOLOCK)
                                Where A.within_code = B.within_code And A.mrp_id = B.id
                                    And A.within_code = C.within_code And A.id = C.id And A.ver = C.ver
                                    And A.within_code = D.within_code And A.id=D.id And A.ver=D.ver And A.upper_sequence=D.sequence_id
                                    And A.within_code = '{0}' And C.mo_id='{1}' And D.goods_id='{2}' And D.wp_id='{3}' And D.next_wp_id='{4}'",
                                within_code, ls_mo_id, ls_goods_id, ls_out_dept, ls_dept_id);
                            dt = sh.ExecuteSqlReturnDataTable(strSql);
                            li_cnt = int.Parse(dt.Rows[0]["cnt"].ToString());
                            //--end 20131107
                            if (li_cnt > 0)
                            {
                                strSql = string.Format(
                                    @" Delete B WITH(ROWLOCK)
                                    From jo_bill_materiel_details A,
                                         mrp_st_details_lot B,
                                         jo_bill_mostly C,
                                         jo_bill_goods_details D
                                    Where A.within_code = B.within_code And A.mrp_id = B.id
                                        And A.within_code = C.within_code And A.id = C.id And A.ver = C.ver
                                        And A.within_code=D.within_code And A.id=D.id And A.ver=D.ver And A.upper_sequence=D.sequence_id
                                        And A.within_code='{0}' And C.mo_id='{1}' And D.goods_id='{2}' And D.wp_id='{3}' And D.next_wp_id='{4}'",
                                    within_code, ls_mo_id, ls_goods_id, ls_out_dept, ls_dept_id);
                                sbSql.Append(strSql);                               
                            }
                            // ===== end 20130813 huangyun 从触发器中移过来

                        } // --end 流程是否存在
                    }  //--end for  FOR循環相當于后臺SQL的游標cur_jo_date
                }  //--ene  if (as_bill_type == "JO")

                //以上是批準(pfc_ok)與反批準(pfc_unok)都要執行的部分,更組裝單信息,生產計劃完成數量,刪除MRP相關數據


                /************************************************************************************************/
                //扣除原料库存 strat 90
                li_cnt = 0;
                if (as_active_name == "pfc_ok")
                {
                    //插入库存交易表(action_id:50 - 减项)
                    strSql = string.Format(
                    @" INSERT INTO st_business_record(within_code, id, goods_id, goods_name, unit, action_time, action_id, ii_qty, ii_location_id,
                        ii_code, rate, sequence_id,operator, check_date, ib_qty, qty, mo_id, lot_no, sec_qty, sec_unit, dept_id, servername)
                    SELECT b.within_code,b.id,b.goods_id,b.goods_name,b.unit_code,a.con_date,
					       '50',--//移交单成品出库
                           ABS(b.con_qty),C.location,C.location,0 as swit_rate,b.sequence_id,'{2}','{3}',
					       ABS(dbo.FN_CHANGE_UNITBYCV(b.within_code, b.goods_id, b.unit_code, 1, '', '', '*')),
					       ABS(Round(dbo.FN_CHANGE_UNITBYCV(b.within_code, b.goods_id, b.unit_code, b.con_qty, '', '', '/'), 4)),
					       b.mo_id,b.lot_no,ABS(b.sec_qty),b.sec_unit,A.out_dept,'{4}'
                    FROM jo_materiel_con_mostly a,
                         jo_materiel_con_details b,
				         cd_department C
                    Where a.within_code = b.within_code and a.id = b.id And a.within_code='{0}' And
                          a.within_code=C.within_code And a.out_dept=C.id And a.id='{1}' And ISNULL(C.location,'')<>'' ",
                          within_code, as_id, as_user_id, ls_check_date, ls_servername);
                    sbSql.Append(strSql);                   
                    sbSql.Append(@" COMMIT TRANSACTION ");
                    result = sh.ExecuteSqlUpdate(sbSql.ToString());//提交更改

                    if (result == "") //前面執行成功
                    {
                        //更新库存(action_id:50 - 减项 ) 
                        return_value = pubFunction.of_update_st_details("I", "50", as_id, "*", ls_check_date, ls_error);//扣减库存前检查对应的交易是否存在,交易不存在不执行对应库存的扣减
                        if (return_value.Substring(0,2) == "-1")
                        {
                            strStatus = return_value + "\r\n" + "数据保存失败!" + "\r\n" + "<" + as_active_name + ">(50:st_details)" + "\r\n";                           
                            return strStatus;
                        }
                        //--start 20120910 
                        //--添加成品的库存增加记录(action_id:49 + 加項)
                        //--固定添加到ZZZ仓位中,目的是为了Hold住这一些库存,不允许盘点,调整,转仓
                        strSql = string.Format(
                        @" INSERT INTO st_business_record(within_code, id, goods_id, goods_name, unit, action_time, action_id, ii_qty,ii_location_id,
                            ii_code, rate, sequence_id,operator, check_date, ib_qty, qty, mo_id, lot_no, sec_qty, sec_unit, dept_id, servername)
                        SELECT b.within_code,b.id,b.goods_id,b.goods_name,b.unit_code,a.con_date,
				             '49',--//移交单成品入库
                             ABS(b.con_qty),C.location,'ZZZ',0 as swit_rate,b.sequence_id,'{2}','{3}',
				             ABS(dbo.FN_CHANGE_UNITBYCV(b.within_code, b.goods_id, b.unit_code, 1,'','','*')),
				             ABS(Round(dbo.FN_CHANGE_UNITBYCV(b.within_code, b.goods_id, b.unit_code, b.con_qty,'','','/'),4)),
				             b.mo_id,b.lot_no,ABS(b.sec_qty),b.sec_unit,a.out_dept,'{4}'
                        FROM jo_materiel_con_mostly a,
                             jo_materiel_con_details b,
				             cd_department C
                        Where a.within_code = b.within_code And a.id = b.id And a.within_code ='{0}' And
                              a.within_code = C.within_code And a.out_dept = C.id And a.id ='{1}' And ISNULL(C.location,'')<>'' ",
                        within_code, as_id, as_user_id, ls_check_date, ls_servername);
                        result = sh.ExecuteSqlUpdate(strSql);
                        if (result != "")
                        {
                            strStatus = "-1" + result + "\r\n" + "数据保存失败!" + "\r\n" + "<" + as_active_name + ">(49:st_business_record)" + "\r\n";                       
                            return strStatus;
                        }
                        //--end 20120910

                        //更新库存(action_id:49 + 加項)
                        return_value = pubFunction.of_update_st_details("I", "49", as_id, "*", ls_check_date, ls_error); //增加库存前检查对应的交易是否存在,交易記錄不存在則不执行对应库存的增加
                        if (return_value.Substring(0,2) == "-1")
                        {
                            strStatus = return_value + "\r\n" + "数据保存失败!" + "\r\n" + "<" + as_active_name + ">(49:st_details)" + "\r\n";                            
                            return strStatus;
                        }
                    }
                }  //--end if (as_active_name == "pfc_ok") 90

                //--start 91
                if (as_active_name == "pfc_unok")
                {
                    //--Start 執行pfc_ok/pfc_unok公共部分的語句
                    sbSql.Append(@" COMMIT TRANSACTION ");
                    result = sh.ExecuteSqlUpdate(sbSql.ToString());//提交更改
                    if (result != "")
                    {
                        strStatus = "-1" + result + "\r\n" + "数据保存失败!" + "\r\n" + "<" + as_active_name + ">(jo_bill_goods)" + "\r\n";                      
                        return strStatus;
                    }
                    //--end 

                    strSql = string.Format(
                        @"Select Count(1) as cnt FROM st_business_record WITH(NOLOCK) Where within_code='{0}' And id='{1}' And action_id In('49','50')",
                        within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["cnt"].ToString());
                    if (li_cnt > 0)
                    {
                        //先更新库存,再删除交易数据 ***
                        return_value = pubFunction.of_update_st_details("D", "49", as_id, "*", as_check_date, ls_error);
                        if (return_value.Substring(0,2) == "-1")
                        {
                            strStatus = return_value + "\r\n" + "数据保存失败!" + "\r\n" + "<" + as_active_name + ">(49:st_details)" + "\r\n";                           
                            return strStatus;
                        }
                        //先更新库存,再删除交易数据 ***
                        return_value = pubFunction.of_update_st_details("D", "50", as_id, "*", as_check_date, ls_error);
                        if (return_value.Substring(0,2) == "-1")
                        {
                            strStatus = return_value + "\r\n" + "数据保存失败!" + "\r\n" + "<" + as_active_name + ">(50:st_details)" + "\r\n";                           
                            return strStatus;
                        }
                        //--
                        strSql = string.Format(
                        @" Delete FROM st_business_record WITH(ROWLOCK)
                        Where within_code ='{0}' And id = '{1}' And action_id In('49','50')",
                        within_code, as_id);
                        result = sh.ExecuteSqlUpdate(strSql);
                        if (result != "")
                        {
                            strStatus = "-1" + result + "\r\n" + "数据保存失败!" + "\r\n" + "<" + as_active_name + ">(Del 49,50:st_business_record)" + "\r\n";
                            return strStatus;
                        }
                    }
                } //--end if (as_active_name == "pfc_unok")
                // --end 91

                //无条件控制负数的出现2012-05-22
                //w_produce_assembly 组装转换單
                if (as_window_id != "")
                {
                    return_value = pubFunction.wf_check_inventory_qty(as_id, as_window_id);//組裝轉換單:"w_produce_assembly"
                    if (return_value.Substring(0, 2) == "-1")
                    {
                        strStatus = return_value;
                        return strStatus;
                    }
                }

            }//end as_active_name == "pfc_ok" or "pfc_unok"
            
            return strStatus;
        }//--end SetRechangeStBusiness()

        /// <summary>
        /// 檢查組裝單包含的移交單號是都批準或反批準
        /// </summary>
        /// <param name="within_code"></param>
        /// <param name="handover_id">移交單號</param>
        /// <param name="con_date">組裝單號日期</param>
        /// <param name="id">組裝單號</param>
        /// <param name="state">查詢狀態:1--查批準;0--查未批準</param>
        /// <returns>返回值:1--表示存在;0--表示不存</returns>
        public static string CheckRechangeStatus(string within_code,string handover_id,string con_date,string id,string state)
        {
            string result = "0";
            if(string.IsNullOrEmpty(handover_id))
            {
                result = "1";
                return result ;
            }
            string handover_id1 = handover_id.Substring(0, 8);  //DT105601           
            string handover_id2 = handover_id.Substring(0, 5) + "702";  //DT105702
            string strSql = string.Format(
                @"SELECT Distinct S.id 
                FROM (select a.id from jo_materiel_con_mostly a with(nolock),jo_materiel_con_details b with(nolock) 
                      where a.within_code=b.within_code and a.id=b.id and a.within_code='{0}' and a.id like '{4}%' and a.con_date='{1}' and a.state='{2}' and b.aim_jo_id='{3}'
                      union all
                      select a.id from jo_materiel_con_mostly a with(nolock),jo_materiel_con_details b with(nolock) 
                      where a.within_code=b.within_code and a.id=b.id and a.within_code='{0}' and a.id like '{5}%' and a.con_date='{1}' and a.state='{2}' and b.aim_jo_id='{3}'
                     ) S", within_code, con_date,state,id, handover_id1, handover_id2);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            result = (dt.Rows.Count > 0) ? "1" : "0";            
            return result;
        }

    } // --end public static class ProduceAssemblyDAL
}
