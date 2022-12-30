using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CF.Framework.Utility;
using CF.Framework.Contract;
using CF.SQLServer.DAL;
using CF.Core.Config;
using CF2025.Base.DAL;
using CF2025.Sale.Contract;


namespace CF2025.Sale.DAL
{
    
    public static class PackingListDAL
    {
        private static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        private static string within_code = "0000";
        private static string language_id = "1";
        public static PackingListViewModel GetDataFromOc(string set_flag,string mo_id)
        {
            string strSql = "";
            strSql += " Select a.it_customer AS customer_id,a.seller_id,a.linkman,a.l_phone AS phone,a.fax,a.email,a.merchandiser,a.merchandiser_phone " +
                ",a.merchandiser_email,a.m_id,a.exchange_rate,a.port_id,a.ap_id,a.contract_id,a.p_id,a.pc_id,a.sm_id" +
                ",a.transport_rate,a.disc_rate,a.disc_amt,a.disc_spare,a.insurance_rate,a.other_fare,a.tax_ticket" +
                ",a.tax_sum,a.ship_mark,a.remark,a.area,a.m_rate" +
                ",f.name AS customer,f.fake_name,f.fake_s_address,f.fake_e_address" +

                ",b.id AS order_id,b.ver AS so_ver,b.sequence_id AS so_sequence_id,b.mo_id" +
                ",b.table_head,b.customer_goods,b.customer_color_id,b.order_qty AS pcs_qty " +
                ",b.goods_unit AS unit_code,b.unit_price AS invoice_price,b.p_unit,b.disc_rate" +
                ",c.name As descript,c.english_name As english_goods_name" +
                ",d.name As color,b.contract_cid AS po_no " +
                ",b.is_free,b.table_head,b.add_remark,b.customer_test_id ";
            if (set_flag == "1")
                strSql += ",b.goods_id AS item_no";
            else
                strSql += ",e.goods_id AS item_no";
            strSql +=
                " From so_order_manage a " +
                " Inner Join so_order_details b On a.within_code=b.within_code And a.id=b.id And a.ver=b.ver ";
            if (set_flag == "1")
                strSql += " Left Join it_goods c On b.within_code=c.within_code And b.goods_id=c.id ";
            else
                strSql +=
                    " Inner Join so_order_bom e On b.within_code=e.within_code And b.id=e.id And b.ver=e.ver And b.sequence_id=e.upper_sequence" +
                    " Left Join it_goods c On e.within_code=c.within_code And e.goods_id=c.id ";
            strSql+=
                " Left Join cd_color d On c.within_code=d.within_code And c.color=d.id " +
                " Left  Join it_customer f On a.within_code=f.within_code And a.it_customer=f.id" +
                " Where b.within_code='" + within_code + "' And b.mo_id='" + mo_id + "'";
            var objData = GetSqlData(strSql);
            return objData;
        }

        public static PackingListViewModel GetDataFromInvoice(string ID,string mo_id)
        {
            string strSql = "";
            strSql = "Select a.id AS invoice_id,a.Ver,a.oi_date,a.separate,a.Shop_no" +
                ",a.it_customer AS customer_id,g.name AS customer,a.phone,a.fax,a.payment_date,a.linkman" +
                ",a.l_phone,a.department_id,a.email,a.issues_wh,a.bill_type_no,a.merchandiser,a.merchandiser_phone,a.po_no" +
                ",a.per,a.remark2 AS remark,a.ship_remark AS packing_list,a.ship_remark2 AS packing_list2,a.ship_remark3 AS packing_list3" +
                ",a.loading_port AS port_id,a.ap_id" +

                ",b.contract_cid AS po_no,b.order_id,b.sequence_id,b.mo_id,shipment_suit" +
                ",b.goods_id AS item_no,b.table_head,b.goods_name AS descript,c.english_name AS english_goods_name,b.u_invoice_qty AS pcs_qty" +
                ",b.goods_unit AS unit_code,b.sec_qty,b.sec_unit,b.box_no,d.name As color" +

                " FROM so_invoice_mostly a" +
                " Inner Join so_invoice_details b On a.within_code=b.within_code And a.id=b.id And a.Ver=b.Ver" +
                " Left  Join it_customer g On a.within_code=g.within_code And a.it_customer=g.id" +
                " Inner Join it_goods c On b.within_code=c.within_code And b.goods_id=c.id " +
                " Inner Join cd_color d On c.within_code=d.within_code And c.color=d.id " +
                " Where a.within_code='" + within_code + "'";
            if (ID != "")
                strSql += " And a.id='" + ID + "'";
            if (mo_id != "")
                strSql += " And b.mo_id='" + mo_id + "'";
            var objData = GetSqlData(strSql);
            return objData;
        }
        public static PackingListViewModel GetDataFromPackingList(string packing_type,string ID, string mo_id)
        {
            PackingListViewModel objData = new PackingListViewModel();
            string strSql = "",strSqlMostly="";
            strSql = "Select b.id,b.sequence_id,b.mark_no,b.box_no,b.item_no,b.descript,b.po_no,b.pcs_qty,b.unit_code" +
                ",b.pbox_qty,b.box_qty,b.cube_ctn,b.total_cube,b.nw_each,b.gw_each,b.tal_nw,b.tal_gw" +
                ",b.order_id,b.so_sequence_id,b.ref_id,b.ref_sequence_id,b.length,b.width,b.height,b.spec,b.sec_unit,b.sec_qty" +
                ",b.mo_id,b.remark,b.Pl_id,b.Pl_sequence_id,b.pl_qty,b.state,b.tr_id,b.tr_sequence_id,b.carton_size,b.symbol" +
                ",b.shipment_suit,b.english_goods_name" +
                ",d.name AS color" +
                " FROM xl_packing_list a" +
                " Inner Join xl_packing_list_details b On a.within_code=b.within_code And a.id=b.id" +
                " Inner Join it_goods c On b.within_code=c.within_code And b.item_no=c.id " +
                " Inner Join cd_color d On c.within_code=d.within_code And c.color=d.id " +
                " Where b.within_code='" + within_code + "'";
            if (ID != "")
            {
                strSqlMostly += "Select a.within_code,a.id,a.packing_date,a.packing_type,a.packing_list,a.customer_id,a.customer" +
                ",a.linkman,a.phone,a.fax,a.messrs,a.shippedby,a.perss,a.sailing_date,a.ap_id,a.port_id,a.contrainer_no" +
                ",a.order_id,a.proceduce_area,a.shipping_tool,a.remark,a.state,a.origin_id" +
                ",a.seal_no,a.registration_mark,a.destination,a.type,a.packing_list2,a.packing_list3,a.per" +
                ",a.total_box_qty,a.fake_name,a.create_by,a.create_date,a.update_by,a.update_date" +
                " FROM xl_packing_list a" +
                " Where a.within_code='" + within_code + "'";
                strSqlMostly += " And a.id='" + ID + "' And a.packing_type='" + packing_type + "'";
                strSql += " And a.id='" + ID + "'";
                DataTable dtPk = sh.ExecuteSqlReturnDataTable(strSqlMostly);
                xl_packing_list_model objMostly = ConvertHelper.DataTableToModel<xl_packing_list_model>(dtPk);
                objData.xl_packing_list_mostly = objMostly;
            }else
            {
                var objInvMostly = GetDataFromInvoice("", mo_id);
                objData.xl_packing_list_mostly = objInvMostly.xl_packing_list_mostly;
                strSql += " And b.mo_id='" + mo_id + "' And a.packing_type='" + packing_type + "'";
            }
            strSql += " Order By b.sequence_id ";
            DataTable dtPkD = sh.ExecuteSqlReturnDataTable(strSql);
            List<xl_packing_list_details_model> objDetails = ConvertHelper.DataTableToList<xl_packing_list_details_model>(dtPkD);
            objData.xl_packing_list_details = objDetails;
            return objData;
        }
        private static PackingListViewModel GetSqlData(string strSql)
        {
            PackingListViewModel objData = new PackingListViewModel();
            DataTable dtOc = sh.ExecuteSqlReturnDataTable(strSql);
            xl_packing_list_model objMostly = new xl_packing_list_model();
            List<xl_packing_list_details_model> objDetails = new List<xl_packing_list_details_model>();
            objMostly = ConvertHelper.DataTableToModel<xl_packing_list_model>(dtOc);
            objDetails = ConvertHelper.DataTableToList<xl_packing_list_details_model>(dtOc);
            objData.xl_packing_list_mostly = objMostly;
            objData.xl_packing_list_details = objDetails;
            return objData;
        }

        public static UpdateStatusModel Save(xl_packing_list_model PkMostly, List<xl_packing_list_details_model> PkDetails)
        {
            string ID = PkMostly.ID == null ? "" : PkMostly.ID;
            UpdateStatusModel mdj = new UpdateStatusModel();
            DataTable dtMostly = CehckPkMostlyExist(ID);
            if (dtMostly.Rows.Count > 0)
            {
                if (dtMostly.Rows[0]["state"].ToString().Trim() != "0")
                {
                    mdj.ReturnValue = ID;
                    mdj.Status = "1";
                    mdj.Msg = "該發票為非編輯狀態!";
                    return mdj;
                }
            }

            string strSql = "";
            string strSql1 = "";
            strSql += string.Format(@" SET XACT_ABORT  ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");
            string userid = "";
            string now_date = Converter.ToCnDateTimeString(System.DateTime.Now);
            if (PkMostly.EditFlag == 1)
            {
                PkMostly.packing_type = "0";
                PkMostly.packing_list = Converter.ReplaceSpecChar(PkMostly.packing_list);
                PkMostly.packing_list2 = Converter.ReplaceSpecChar(PkMostly.packing_list2);
                PkMostly.packing_list3 = Converter.ReplaceSpecChar(PkMostly.packing_list3);
                PkMostly.remark = Converter.ReplaceSpecChar(PkMostly.remark);
                PkMostly.destination = Converter.ReplaceSpecChar(PkMostly.destination);
                
                if (dtMostly.Rows.Count == 0)
                {
                    UpdateStatusModel mdjID = new UpdateStatusModel();
                    mdjID= GenDocNumber(PkMostly.packing_date);
                    ID = mdjID.ReturnValue;
                    strSql += mdjID.ReturnValue1;
                    strSql1 += " Insert Into xl_packing_list (" +
                        " within_code,id,packing_date,packing_type,packing_list,customer_id,customer"+
                        ",linkman,phone,fax,messrs,shippedby,perss,sailing_date,ap_id,port_id,contrainer_no"+
                        ",order_id,proceduce_area,shipping_tool,remark,state,origin_id"+
                        ",seal_no,registration_mark,destination,type,packing_list2,packing_list3,per"+
                        ",total_box_qty,fake_name,create_by,create_date,update_by,update_date";
                    strSql1 += ") Values (";
                    strSql1 += " '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'" +
                        ",'{10}','{11}','{12}','{13}','{14}','{15}','{16}', '{17}','{18}','{19}'" +
                        ",'{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}'" +
                        ",'{30}','{31}','{32}','{33}','{32}','{33}'";
                    strSql1 += ")";

                }
                else
                {
                    strSql1 += " Update xl_packing_list Set " +
                        " packing_date='{2}',packing_type='{3}',packing_list='{4}',customer_id='{5}',customer='{6}',linkman='{7}',phone='{8}'" +
                        ",fax='{9}',messrs='{10}',shippedby='{11}',perss='{12}',sailing_date='{13}',ap_id='{14}',port_id='{15}',contrainer_no='{16}'" +
                        ",order_id='{17}',proceduce_area='{18}',shipping_tool='{19}',remark='{20}',state='{21}'" +
                        ",origin_id='{22}',seal_no='{23}',registration_mark='{24}',destination='{25}',type='{26}',packing_list2='{27}',packing_list3='{28}',per='{29}'" +
                        ",total_box_qty='{30}',fake_name='{31}',update_by='{32}',update_date='{33}'";
                    strSql1 += " Where within_code='{0}' And id='{1}' ";
                }
                strSql += string.Format(@strSql1
                        , within_code, ID, PkMostly.packing_date, PkMostly.packing_type, PkMostly.packing_list
                        , PkMostly.customer_id, PkMostly.customer
                        , PkMostly.linkman, PkMostly.phone, PkMostly.fax, PkMostly.messrs, PkMostly.shippedby, PkMostly.perss
                        , PkMostly.sailing_date, PkMostly.ap_id, PkMostly.port_id, PkMostly.contrainer_no
                        , PkMostly.order_id, PkMostly.proceduce_area, PkMostly.shipping_tool, PkMostly.remark, PkMostly.state
                        , PkMostly.origin_id, PkMostly.seal_no, PkMostly.registration_mark
                        , PkMostly.destination, PkMostly.type, PkMostly.packing_list2, PkMostly.packing_list3, PkMostly.per
                        , PkMostly.total_box_qty, PkMostly.fake_name
                        , userid,now_date
                    );
            }
            if (PkDetails != null)
            {

                int MaxSeq = GetDetailsMaxSeq(ID);
                for (int i = 0; i < PkDetails.Count; i++)
                {
                    xl_packing_list_details_model objDetail = PkDetails[i];
                    if (objDetail.EditFlag == 1)
                    {
                        strSql1 = "";
                        string sequence_id = "";
                        if (objDetail.sequence_id == null)
                        {
                            MaxSeq += 1;
                            sequence_id = MaxSeq.ToString().PadLeft(4, '0') + "d";
                        }
                        else
                            sequence_id = objDetail.sequence_id;
                        if (!CheckPkDetailsExist(ID, sequence_id))
                            strSql1 += " Insert Into xl_packing_list_details (" +
                                "within_code,id,sequence_id,mark_no,box_no,item_no,descript,po_no,pcs_qty,unit_code" +
                                ",pbox_qty,box_qty,cube_ctn,total_cube,nw_each,gw_each,tal_nw,tal_gw" +
                                ",order_id,so_sequence_id,ref_id,ref_sequence_id,length,width,height,spec,sec_unit,sec_qty" +
                                ",mo_id,remark,Pl_id,Pl_sequence_id,pl_qty,state,tr_id,tr_sequence_id,carton_size,symbol" +
                                ",shipment_suit,english_goods_name" +
                                " )" +
                                " Values " +
                                " ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'" +
                                ",'{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}'" +
                                ",'{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}'" +
                                ",'{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}'" +
                                " )";
                        else
                        {
                            strSql1 += " Update xl_packing_list_details Set " +
                                " mark_no='{3}',box_no='{4}',item_no='{5}',descript='{6}',po_no='{7}',pcs_qty='{8}',unit_code='{9}'" +
                                ",pbox_qty='{10}',box_qty='{11}',cube_ctn='{12}',total_cube='{13}',nw_each='{14}',gw_each='{15}',tal_nw='{16}',tal_gw='{17}'" +
                                ",order_id='{18}',so_sequence_id='{19}',ref_id='{20}',ref_sequence_id='{21}',length='{22}',width='{23}',height='{24}',spec='{25}',sec_unit='{26}',sec_qty='{27}'" +
                                ",mo_id='{28}',remark='{29}',Pl_id='{30}',Pl_sequence_id='{31}',pl_qty='{32}',state='{33}',tr_id='{34}',tr_sequence_id='{35}',carton_size='{36}',symbol='{37}'" +
                                ",shipment_suit='{38}',english_goods_name='{39}' ";
                            strSql1 += " Where within_code='{0}' And id='{1}' And sequence_id='{2}'";
                        }
                        objDetail.english_goods_name = Converter.ReplaceSpecChar(objDetail.english_goods_name);
                        objDetail.remark = Converter.ReplaceSpecChar(objDetail.remark);
                        strSql += string.Format(@strSql1
                            , within_code, ID, sequence_id, objDetail.mark_no, objDetail.box_no, objDetail.item_no, objDetail.descript
                            , objDetail.po_no, objDetail.pcs_qty, objDetail.unit_code, objDetail.pbox_qty, objDetail.box_qty
                            , objDetail.cube_ctn, objDetail.total_cube, objDetail.nw_each, objDetail.gw_each, objDetail.tal_nw
                            , objDetail.tal_gw, objDetail.order_id, objDetail.so_sequence_id, objDetail.ref_id, objDetail.ref_sequence_id
                            , objDetail.length, objDetail.width, objDetail.height, objDetail.spec, objDetail.sec_unit, objDetail.sec_qty
                            , objDetail.mo_id, objDetail.remark, objDetail.Pl_id, objDetail.Pl_sequence_id, objDetail.pl_qty
                            , objDetail.state, objDetail.tr_id, objDetail.tr_sequence_id, objDetail.carton_size, objDetail.symbol
                            , objDetail.shipment_suit, objDetail.english_goods_name
                            );
                    }
                }
                if (PkMostly.EditFlag == 0)
                {
                    strSql1 = " Update xl_packing_list Set update_by='{2}',update_date='{3}'" +
                        " Where within_code='{0}' And id='{1}' ";
                    strSql += string.Format(@strSql1, within_code, ID, userid, now_date);
                }
            }

            strSql += string.Format(@" COMMIT TRANSACTION ");
            string result = sh.ExecuteSqlUpdate(strSql);
            mdj.ReturnValue = ID;
            if (result == "")
            {
                mdj.Status = "0";
                mdj.Msg = "發票儲存成功!";
            }
            else
            {
                mdj.Status = "1";
                mdj.Msg = result;
            }
            return mdj;
        }

        private static DataTable CehckPkMostlyExist(string ID)
        {
            string strSql = "Select id,state " +
                " FROM xl_packing_list Where within_code='" + within_code + "' And id='" + ID + "'";
            DataTable dtInv = sh.ExecuteSqlReturnDataTable(strSql);
            return dtInv;
        }
        private static UpdateStatusModel GenDocNumber(string packing_date)
        {

            //within_code bill_id year_month bill_code
            UpdateStatusModel mdjID = new UpdateStatusModel();
            string bill_id = "PL01";
            string year_month = packing_date.Substring(2, 2) + packing_date.Substring(5, 2);
            string bill_code = bill_id.Substring(0, 2) + year_month;
            string strSql = "";
            strSql = "Select bill_code From sys_bill_max Where within_code='" + within_code + "' And bill_id='" +
                bill_id + "' And year_month='" + year_month + "'";
            DataTable dtID = sh.ExecuteSqlReturnDataTable(strSql);
            if (dtID.Rows.Count == 0)
            {
                bill_code = bill_code + "0001";//PL22110001
                strSql = string.Format(@"Insert Into sys_bill_max (within_code,bill_id,year_month,bill_code) Values " +
                    "('{0}','{1}','{2}','{3}')"
                    , within_code, bill_id, year_month, bill_code);
            }
            else
            {
                string IDSeq = (Convert.ToInt32(dtID.Rows[0]["bill_code"].ToString().Substring(6, 4)) + 1).ToString().PadLeft(4, '0');
                bill_code = bill_code + IDSeq;//HC21C00006
                strSql = string.Format(@"Update sys_bill_max set bill_code='{3}' Where " +
                    "within_code='{0}' And bill_id='{1}' And year_month='{2}'"
                    , within_code, bill_id, year_month, bill_code);
            }
            //string result = sh.ExecuteSqlUpdate(strSql);
            mdjID.ReturnValue1 = strSql;
            mdjID.ReturnValue = bill_code;
            return mdjID;
        }

        private static int GetDetailsMaxSeq(string ID)
        {
            int result = 0;
            string strSql = "";
            strSql = "Select MAX(Sequence_id) AS Sequence_id FROM xl_packing_list_details Where within_code='" + within_code + "' And id='" + ID + "'";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
                result = dt.Rows[0]["Sequence_id"].ToString() != "" ? Convert.ToInt32(dt.Rows[0]["Sequence_id"].ToString().Substring(0, 4)) : 0;
            else
                result = 0;
            return result;
        }
        private static bool CheckPkDetailsExist(string ID, string Sequence_id)
        {
            bool result = true;
            string strSql = "Select sequence_id FROM xl_packing_list_details Where within_code='" + within_code
                + "' And id='" + ID + "' And sequence_id='" + Sequence_id + "'";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
                result = true;
            else
                result = false;
            return result;
        }

        public static UpdateStatusModel Delete(string ID,string sequence_id)
        {
            UpdateStatusModel mdj = new UpdateStatusModel();
            DataTable dtMostly = CehckPkMostlyExist(ID);
            if(dtMostly.Rows.Count==0)
            {
                mdj.Msg = "沒有要刪除的記錄!";
                mdj.ReturnValue = ID;
                return mdj;
            }else
            {
                if(dtMostly.Rows[0]["state"].ToString()!="0")
                {
                    mdj.Msg = "此單為非編輯狀態,不能刪除!";
                    mdj.ReturnValue = ID;
                    return mdj;
                }
            }
            string strSql = "";
            strSql += string.Format(@" SET XACT_ABORT  ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");
            strSql += string.Format(" Delete From xl_packing_list_details " +
                " Where within_code='{0}' And id='{1}' And sequence_id='{2}' "
                , within_code, ID, sequence_id);

            strSql += string.Format(@" COMMIT TRANSACTION ");
            string result = sh.ExecuteSqlUpdate(strSql);
            
            mdj.ReturnValue = ID;
            if (result == "")
            {
                mdj.Status = "0";
                mdj.Msg = "刪除記錄成功!";
            }
            else
            {
                mdj.Status = "1";
                mdj.Msg = result;
            }
            return mdj;
        }
    }
}
