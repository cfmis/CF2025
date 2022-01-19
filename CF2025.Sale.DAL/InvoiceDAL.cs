using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CF.Framework.Contract;
using CF.SQLServer.DAL;
using CF.Core.Config;
using CF2025.Sale.Contract;

namespace CF2025.Sale.DAL
{
    public static class InvoiceDAL
    {
        private static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        private static string within_code = "0000";
        public static List<ModelBaseList> GetComboxList(string SourceType)
        {
            string strSql = "";
            switch (SourceType)
            {
                case "DocSourceTypeList"://單據來源
                    strSql += "Select substring(id,2,1) As id,name,name As english_name From sys_bill_origin Where function_id='SO01' AND language='3' Order By id";
                    break;
                case "OutStoreList"://發貨倉位
                    strSql += "Select id,name,english_name From cd_mo_type Where within_code='" + within_code + "' And mo_type='7' Order By id";
                    break;
                case "InvSourceTypeList"://單據種類
                    strSql += "Select id,name,english_name From cd_shipment Where within_code='" + within_code + "' Order By id";
                    break;
                case "PaymentTypeList"://付款方式
                    strSql += "Select id,name,name As english_name From cd_payment Where within_code='" + within_code + "' Order By id";
                    break;
                case "PriceCondList"://價格條件
                    strSql += "Select id,name,english_name From cd_payment_condition Where within_code='" + within_code + "' Order By id";
                    break;
                case "ShipModeList"://運輸方式
                    strSql += "Select id,name,name As english_name From cd_shipping_mode Where within_code='" + within_code + "' Order By id";
                    break;
                case "AccountList"://銀行賬號
                    strSql += "Select abbreviate As id,accounts As name,accounts As english_name From cd_company_accounts Where within_code='" + within_code + "' Order By abbreviate";
                    break;
                case "ShipWayList"://運輸途徑
                    strSql += "Select id,name,english_name From cd_mo_type Where within_code='" + within_code + "' And mo_type='B' Order By id";
                    break;
                case "ShipPortList"://發貨港口&目的港口
                    strSql += "Select id,name,english_name From cd_port Where within_code='" + within_code + "' Order By id";
                    break;
                default:
                    strSql += "";
                    break;
            }
            //
            List<ModelBaseList> ls = new List<ModelBaseList>();
            ModelBaseList obj1 = new ModelBaseList();
            obj1.value = "";
            obj1.label = "";
            ls.Add(obj1);
            DataTable dtSalesman = sh.ExecuteSqlReturnDataTable(strSql);
            for (int i = 0; i < dtSalesman.Rows.Count; i++)
            {
                DataRow dr = dtSalesman.Rows[i];
                ModelBaseList obj = new ModelBaseList();
                obj.value = dr["id"].ToString();
                obj.label = dr["id"].ToString().Trim() + "--" + dr["name"].ToString().Trim();
                ls.Add(obj);
            }
            return ls;
        }
        public static so_invoice_mostly GetMostlyFromOc(string mo_id)
        {
            so_invoice_mostly mdjInv = new so_invoice_mostly();
            string strSql = "";
            strSql += " Select a.it_customer,a.seller_id,a.linkman,a.l_phone,a.fax,a.email,a.merchandiser,a.merchandiser_phone " +
                ",a.merchandiser_email,a.m_id,c.exchange_rate,a.port_id,a.ap_id,a.contract_id,a.p_id,a.pc_id,a.sm_id" +
                ",a.transport_rate,a.disc_rate,a.disc_amt,a.disc_spare,a.insurance_rate,a.other_fare,a.tax_ticket" +
                ",a.tax_sum,a.ship_mark,a.remark" +
                " From so_order_manage a " +
                " Inner Join so_order_details b On a.within_code=b.within_code And a.id=b.id And a.ver=b.ver " +
                " Left Join cd_exchange_rate c ON a.within_code=c.within_code AND a.m_id=c.id" +
                " Where b.within_code='" + within_code + "' And b.mo_id='" + mo_id + "' And c.state='0' ";
            DataTable dtMostly = sh.ExecuteSqlReturnDataTable(strSql);
            if (dtMostly.Rows.Count > 0)
            {
                DataRow dr = dtMostly.Rows[0];
                mdjInv.it_customer = dr["it_customer"].ToString().Trim();
                mdjInv.finally_buyer = dr["it_customer"].ToString().Trim();
                mdjInv.seller_id = dr["seller_id"].ToString().Trim();
                mdjInv.po_no = dr["contract_id"].ToString().Trim(); 
                mdjInv.phone = dr["l_phone"].ToString().Trim();
                mdjInv.linkman = dr["linkman"].ToString().Trim();
                mdjInv.l_phone = dr["l_phone"].ToString().Trim();
                mdjInv.fax = dr["fax"].ToString().Trim();
                mdjInv.email = dr["email"].ToString().Trim();
                mdjInv.merchandiser = dr["merchandiser"].ToString().Trim();
                mdjInv.merchandiser_phone = dr["merchandiser_phone"].ToString().Trim();
                mdjInv.m_id = dr["m_id"].ToString().Trim();
                mdjInv.exchange_rate = Convert.ToDecimal(dr["exchange_rate"].ToString()); 
                mdjInv.p_id = dr["p_id"].ToString().Trim();
                mdjInv.sm_id = dr["sm_id"].ToString().Trim();
                mdjInv.pc_id = dr["pc_id"].ToString().Trim();
                mdjInv.tax_ticket = dr["tax_ticket"].ToString().Trim();
                mdjInv.ship_remark = dr["ship_mark"].ToString().Trim();
                mdjInv.remark = dr["remark"].ToString().Trim();
            }
            return mdjInv;
        }
        public static List<viewOc> GetDetailsFromOc(string mo_id)
        {
            List<viewOc> lsInv = new List<viewOc>();
            string strSql = "";
            strSql += " Select b.id,a.it_customer,b.mo_id,b.goods_id,b.table_head,b.customer_goods,b.customer_color_id,b.order_qty " +
                ",b.goods_unit,b.unit_price,b.p_unit,b.disc_rate,c.name As goods_name,d.name As color,b.contract_cid,e.name As big_class " +
                ",b.is_free,b.table_head,b.add_remark,b.customer_test_id " +
                ",f.fare_id,f.name As fare_name,f.tf_percent,f.qty As fare_qty,f.price As fare_price,f.fare_sum" +
                ",f.mo_id As fare_mo_id,f.mould_no,f.remark As fare_remark " +
                " From so_order_manage a " +
                " Inner Join so_order_details b On a.within_code=b.within_code And a.id=b.id And a.ver=b.ver " +
                " Left Join it_goods c On b.within_code=c.within_code And b.goods_id=c.id " +
                " Left Join cd_color d On c.within_code=d.within_code And c.color=d.id " +
                " Left Join cd_goods_class e On c.within_code=e.within_code And c.big_class=e.id " +
                " Left Join so_other_fare f On b.within_code=f.within_code And b.id=f.id And b.ver=f.ver And b.mo_id=f.mo_id " +
                " Where b.within_code='" + within_code + "' And b.mo_id='" + mo_id + "'";
            DataTable dtMostly = sh.ExecuteSqlReturnDataTable(strSql);
            for(int i=0;i< dtMostly.Rows.Count;i++)
            {
                DataRow dr = dtMostly.Rows[i];
                viewOc objInv = new viewOc();
                objInv.ocMostly.it_customer = dr["it_customer"].ToString().Trim();
                objInv.ocDetails.mo_id = dr["mo_id"].ToString().Trim();
                objInv.ocDetails.goods_id = dr["goods_id"].ToString().Trim();
                objInv.ocDetails.goods_name = dr["goods_name"].ToString().Trim();
                objInv.ocDetails.u_invoice_qty = Convert.ToDecimal(dr["order_qty"].ToString());
                objInv.ocDetails.goods_unit = dr["goods_unit"].ToString().Trim();
                objInv.ocDetails.invoice_price = Convert.ToDecimal(dr["unit_price"].ToString());
                objInv.ocDetails.p_unit = dr["p_unit"].ToString().Trim();
                objInv.ocDetails.customer_goods = dr["customer_goods"].ToString().Trim();
                objInv.ocDetails.customer_color_id = dr["customer_color_id"].ToString().Trim();
                objInv.ocDetails.color = dr["color"].ToString().Trim();
                objInv.ocDetails.contract_cid = dr["contract_cid"].ToString().Trim();
                objInv.ocDetails.order_id = dr["id"].ToString().Trim();
                objInv.ocDetails.disc_rate = Convert.ToDecimal(dr["disc_rate"].ToString());
                objInv.ocDetails.is_print = "Y";
                objInv.ocDetails.location_id = "Y10";
                objInv.ocDetails.big_class = dr["big_class"].ToString().Trim();
                objInv.ocDetails.is_free = dr["is_free"].ToString().Trim();
                objInv.ocDetails.table_head = dr["table_head"].ToString().Trim();
                objInv.ocDetails.remark = dr["add_remark"].ToString().Trim();
                objInv.ocDetails.customer_test_id = dr["customer_test_id"].ToString().Trim();
                objInv.ocOtherFare.EditFlag = 1;
                objInv.ocOtherFare.fare_id = dr["fare_id"].ToString().Trim();
                objInv.ocOtherFare.name = dr["fare_name"].ToString().Trim();
                objInv.ocOtherFare.tf_percent = dr["tf_percent"].ToString() == "" ? 0 : Convert.ToDecimal(dr["tf_percent"].ToString());
                objInv.ocOtherFare.qty = dr["fare_qty"].ToString() == "" ? 0 : Convert.ToDecimal(dr["fare_qty"].ToString());
                objInv.ocOtherFare.price = dr["fare_price"].ToString() == "" ? 0 : Convert.ToDecimal(dr["fare_price"].ToString());
                objInv.ocOtherFare.fare_sum = dr["fare_sum"].ToString() == "" ? 0 : Convert.ToDecimal(dr["fare_sum"].ToString());
                objInv.ocOtherFare.mo_id = dr["fare_mo_id"].ToString().Trim();
                objInv.ocOtherFare.mould_no = dr["mould_no"].ToString().Trim();
                objInv.ocOtherFare.remark = dr["fare_remark"].ToString().Trim();
                lsInv.Add(objInv);
            }
            return lsInv;
        }
        public static UpdateStatusModel SaveInvoice(so_invoice_mostly InvMostly, List<so_invoice_details> InvDetails, List<so_other_fare> InvOtherFare)
        {
            string ID = InvMostly.ID == null ? "" : InvMostly.ID;
            int Ver = InvMostly.Ver;
            string strSql = "";
            strSql += string.Format(@" SET XACT_ABORT  ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");
            if(InvMostly.EditFlag==1)
            {
                if (!CehckInvMostlyExist(ID))
                {
                    ID = InvoiceBase.GenDocNumber("SI01", InvMostly.issues_wh, InvMostly.bill_type_no, InvMostly.oi_date);
                    Ver = 0;
                    strSql += string.Format(@" Insert Into so_invoice_mostly (within_code,id,Ver,oi_date,it_customer,separate,phone
                            ,fax,payment_date,linkman,l_phone,department_id
                            ,email,issues_wh,bill_type_no,merchandiser,merchandiser_phone)" +
                            " Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}')"
                                , within_code, ID, Ver, InvMostly.oi_date, InvMostly.it_customer, InvMostly.separate, InvMostly.phone
                            , InvMostly.fax, InvMostly.payment_date, InvMostly.linkman, InvMostly.l_phone, InvMostly.department_id
                            , InvMostly.email, InvMostly.issues_wh, InvMostly.bill_type_no, InvMostly.merchandiser, InvMostly.merchandiser_phone);
                }
                else
                    strSql += string.Format(@" Update so_invoice_mostly Set oi_date='{3}',it_customer='{4}',separate='{5}',phone='{6}' " +
                                ",fax='{7}',payment_date='{8}',linkman='{9}',l_phone='{10}',department_id='{11}',email='{12}'" +
                                ",issues_wh='{13}',bill_type_no='{14}',merchandiser='{15}',merchandiser_phone='{16}'" +
                                " Where within_code='{0}' And id='{1}' And Ver='{2}' "
                                , within_code, ID, Ver, InvMostly.oi_date, InvMostly.it_customer, InvMostly.separate, InvMostly.phone
                            , InvMostly.fax, InvMostly.payment_date, InvMostly.linkman, InvMostly.l_phone, InvMostly.department_id
                            , InvMostly.email, InvMostly.issues_wh, InvMostly.bill_type_no, InvMostly.merchandiser
                            , InvMostly.merchandiser_phone);
            }
            if (InvDetails != null)
            {
                int MaxSeq = GetDetailsMaxSeq("so_invoice_details",ID, Ver);
                for (int i = 0; i < InvDetails.Count; i++)
                {
                    if (InvDetails[i].EditFlag == 1)
                    {
                        string sequence_id = "";
                        if (InvDetails[i].sequence_id == null)
                        {
                            MaxSeq += 1;
                            sequence_id = MaxSeq.ToString().PadLeft(4, '0') + "h";
                        }
                        else
                            sequence_id = InvDetails[i].sequence_id;
                        if (!CheckInvDetailsExist("so_invoice_details",ID, Ver, sequence_id))
                            strSql += string.Format(@" Insert Into so_invoice_details (within_code,id,Ver,sequence_id,mo_id,shipment_suit,goods_id
                                ,table_head,goods_name,u_invoice_qty,goods_unit,sec_qty,sec_unit )" +
                                " Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')"
                                , within_code, ID, Ver, sequence_id, InvDetails[i].mo_id, InvDetails[i].shipment_suit, InvDetails[i].goods_id
                                , InvDetails[i].table_head, InvDetails[i].goods_name, InvDetails[i].u_invoice_qty, InvDetails[i].goods_unit
                                , InvDetails[i].sec_qty, InvDetails[i].sec_unit);
                        else
                            strSql += string.Format(@" Update so_invoice_details Set mo_id='{4}',shipment_suit='{5}',goods_id='{6}',table_head='{7}',goods_name='{8}' " +
                                ",u_invoice_qty='{9}',goods_unit='{10}',sec_qty='{11}',sec_unit='{12}'" +
                                " Where within_code='{0}' And id='{1}' And Ver='{2}' And sequence_id='{3}' "
                                , within_code, ID, Ver, sequence_id, InvDetails[i].mo_id, InvDetails[i].shipment_suit, InvDetails[i].goods_id
                                , InvDetails[i].table_head, InvDetails[i].goods_name, InvDetails[i].u_invoice_qty, InvDetails[i].goods_unit
                                , InvDetails[i].sec_qty, InvDetails[i].sec_unit);
                    }
                }
            }
            if (InvOtherFare != null)
            {
                int MaxSeq = GetDetailsMaxSeq("so_invoice_other_fare", ID, Ver);
                for (int i = 0; i < InvOtherFare.Count; i++)
                {
                    if (InvOtherFare[i].EditFlag == 1)
                    {
                        string sequence_id = "";
                        if (InvOtherFare[i].sequence_id == null)
                        {
                            MaxSeq += 1;
                            sequence_id = MaxSeq.ToString().PadLeft(4, '0') + "h";
                        }
                        else
                            sequence_id = InvOtherFare[i].sequence_id;
                        if (!CheckInvDetailsExist("so_invoice_other_fare",ID, Ver, sequence_id))
                            strSql += string.Format(@" Insert Into so_invoice_other_fare (within_code,id,Ver,sequence_id,fare_id,name,tf_percent
                                ,fare_sum,sum_kind,mo_id,remark,is_free,qty,price )" +
                                " Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')"
                                , within_code, ID, Ver, sequence_id, InvOtherFare[i].fare_id, InvOtherFare[i].name, InvOtherFare[i].tf_percent
                                , InvOtherFare[i].fare_sum, InvOtherFare[i].sum_kind, InvOtherFare[i].mo_id, InvOtherFare[i].remark
                                , InvOtherFare[i].is_free, InvOtherFare[i].qty, InvOtherFare[i].price);
                        else
                            strSql += string.Format(@" Update so_invoice_other_fare Set fare_id='{4}',name='{5}',tf_percent='{6}',fare_sum='{7}',sum_kind='{8}' " +
                                ",mo_id='{9}',remark='{10}',is_free='{11}',qty='{12}',price='{13}'" +
                                " Where within_code='{0}' And id='{1}' And Ver='{2}' And sequence_id='{3}' "
                                , within_code, ID, Ver, sequence_id, InvOtherFare[i].fare_id, InvOtherFare[i].name, InvOtherFare[i].tf_percent
                                , InvOtherFare[i].fare_sum, InvOtherFare[i].sum_kind, InvOtherFare[i].mo_id, InvOtherFare[i].remark
                                , InvOtherFare[i].is_free, InvOtherFare[i].qty, InvOtherFare[i].price);
                    }
                }
            }
            strSql += string.Format(@" COMMIT TRANSACTION ");
            string result = sh.ExecuteSqlUpdate(strSql);
            UpdateStatusModel mdj = new UpdateStatusModel();
            mdj.ReturnValue = ID;
            if (result == "")
            {
                mdj.Status = "0";
                mdj.Msg = "";
            }
            else
            {
                mdj.Status = "1";
                mdj.Msg = result;
            }
            return mdj;
        }
        private static bool CehckInvMostlyExist(string ID)
        {
            bool result = true;
            string strSql = "Select id FROM so_invoice_mostly Where within_code='" + within_code + "' And id='" + ID + "'";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
                result = true;
            else
                result = false;
            return result;
        }
        private static int GetDetailsMaxSeq(string tbName,string ID, int Ver)
        {
            int result = 0;
            string strSql = "";
            strSql = "Select MAX(Sequence_id) AS Sequence_id FROM "+tbName+" Where within_code='" + within_code + "' And id='" + ID + "' And Ver='" + Ver + "'";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
                result = dt.Rows[0]["Sequence_id"].ToString() != "" ? Convert.ToInt32(dt.Rows[0]["Sequence_id"].ToString().Substring(0,4)) : 0;
            else
                result = 0;
            return result;
        }
        private static bool CheckInvDetailsExist(string tbName,string ID, int Ver, string Sequence_id)
        {
            bool result = true;
            string strSql = "Select sequence_id FROM " + tbName + " Where within_code='" + within_code
                + "' And id='" + ID + "' And Ver='" + Ver + "' And sequence_id='" + Sequence_id + "'";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
                result = true;
            else
                result = false;
            return result;
        }

        public static List<viewOc> GetDataByID(string ID)
        {
            List<viewOc> lsInv = new List<viewOc>();
            string strSql = "";
            strSql = "Select a.id,a.Ver,a.oi_date,a.separate,a.Shop_no,a.it_customer,a.phone,a.fax,a.payment_date,a.linkman" +
                ",a.l_phone,a.department_id,a.email,a.issues_wh,a.bill_type_no,a.merchandiser,a.merchandiser_phone,a.po_no" +
                ",a.shipping_methods,a.seller_id,a.m_id,a.exchange_rate,a.goods_sum,a.other_fare,a.disc_rate,a.disc_amt" +
                ",a.disc_spare,a.total_sum,a.tax_ticket,a.tax_sum,a.amount,a.other_fee,a.total_package_num,a.total_weight" +
                ",a.total_package_num,a.total_weight,a.remark2,a.ship_remark,a.ship_remark2,a.ship_remark3,a.remark,a.p_id" +
                ",a.pc_id,a.sm_id,a.accounts,a.per,a.final_destination,a.issues_state,a.transport_style,a.ship_date" +
                ",a.loading_port,a.ap_id,a.tranship_port,a.finally_buyer,a.mo_group,a.packinglistno,a.box_no,a.create_by" +
                ",a.create_date,a.update_by,a.update_date,a.state,a.check_date" +
                ",b.sequence_id,b.mo_id,b.goods_id " +
                " FROM so_invoice_mostly a"+
                " Inner Join so_invoice_details b On a.within_code=b.within_code And a.id=b.id And a.Ver=b.Ver"+
                " Where a.within_code='" + within_code + "' And a.id='" + ID + "'";
            strSql += " Order By b.sequence_id ";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            for(int i=0;i<dt.Rows.Count;i++)
            {
                DataRow dr = dt.Rows[i];
                viewOc mdj = new viewOc();
                mdj.ocMostly.EditFlag = 0;
                mdj.ocMostly.ID = dr["id"].ToString();
                mdj.ocMostly.oi_date = dr["oi_date"].ToString();
                mdj.ocMostly.it_customer = dr["it_customer"].ToString();
                mdj.ocMostly.Ver = Convert.ToInt32(dr["Ver"]);
                mdj.ocMostly.oi_date = dr["oi_date"].ToString();
                mdj.ocMostly.separate = dr["separate"].ToString();
                mdj.ocMostly.Shop_no = dr["Shop_no"].ToString();
                mdj.ocMostly.phone = dr["phone"].ToString();
                mdj.ocMostly.fax = dr["fax"].ToString();
                mdj.ocMostly.payment_date = dr["payment_date"].ToString();
                mdj.ocMostly.linkman = dr["linkman"].ToString();
                mdj.ocMostly.l_phone = dr["l_phone"].ToString();
                mdj.ocMostly.department_id = dr["department_id"].ToString();
                mdj.ocMostly.email = dr["email"].ToString();
                mdj.ocMostly.issues_wh = dr["issues_wh"].ToString();
                mdj.ocMostly.bill_type_no = dr["bill_type_no"].ToString();
                mdj.ocMostly.merchandiser = dr["merchandiser"].ToString();
                mdj.ocMostly.merchandiser_phone = dr["merchandiser_phone"].ToString();
                mdj.ocMostly.po_no = dr["po_no"].ToString();
                mdj.ocMostly.shipping_methods = dr["shipping_methods"].ToString();
                mdj.ocMostly.seller_id = dr["seller_id"].ToString();
                mdj.ocMostly.m_id = dr["m_id"].ToString();
                mdj.ocMostly.exchange_rate = dr["exchange_rate"].ToString() == "" ? 0 : Convert.ToDecimal(dr["exchange_rate"]);
                mdj.ocMostly.goods_sum = dr["goods_sum"].ToString() == "" ? 0 : Convert.ToDecimal(dr["goods_sum"]);
                mdj.ocMostly.other_fare = dr["other_fare"].ToString() == "" ? 0 : Convert.ToDecimal(dr["other_fare"]);
                mdj.ocMostly.disc_rate = dr["disc_rate"].ToString() == "" ? 0 : Convert.ToDecimal(dr["disc_rate"]);
                mdj.ocMostly.disc_amt = dr["disc_amt"].ToString() == "" ? 0 : Convert.ToDecimal(dr["disc_amt"]);
                mdj.ocMostly.disc_spare = dr["disc_spare"].ToString() == "" ? 0 : Convert.ToDecimal(dr["disc_spare"]);
                mdj.ocMostly.total_sum = dr["total_sum"].ToString() == "" ? 0 : Convert.ToDecimal(dr["total_sum"]);
                mdj.ocMostly.tax_ticket = dr["it_customer"].ToString();
                mdj.ocMostly.tax_sum = dr["tax_sum"].ToString() == "" ? 0 : Convert.ToDecimal(dr["tax_sum"]);
                mdj.ocMostly.amount = dr["amount"].ToString() == "" ? 0 : Convert.ToDecimal(dr["amount"]);
                mdj.ocMostly.other_fee = dr["other_fee"].ToString() == "" ? 0 : Convert.ToDecimal(dr["other_fee"]);
                mdj.ocMostly.total_package_num = dr["total_package_num"].ToString() == "" ? 0 : Convert.ToInt32(dr["total_package_num"]);
                mdj.ocMostly.total_weight = dr["total_weight"].ToString() == "" ? 0 : Convert.ToDecimal(dr["total_weight"]);
                mdj.ocMostly.total_package_num = dr["total_package_num"].ToString() == "" ? 0 : Convert.ToInt32(dr["total_package_num"]);
                mdj.ocMostly.total_weight = dr["total_weight"].ToString() == "" ? 0 : Convert.ToDecimal(dr["total_weight"]);
                mdj.ocMostly.remark2 = dr["remark2"].ToString();
                mdj.ocMostly.ship_remark = dr["ship_remark"].ToString();
                mdj.ocMostly.ship_remark2 = dr["ship_remark2"].ToString();
                mdj.ocMostly.ship_remark3 = dr["ship_remark3"].ToString();
                mdj.ocMostly.remark = dr["remark"].ToString();
                mdj.ocMostly.p_id = dr["p_id"].ToString();
                mdj.ocMostly.pc_id = dr["pc_id"].ToString();
                mdj.ocMostly.sm_id = dr["sm_id"].ToString();
                mdj.ocMostly.accounts = dr["accounts"].ToString();
                mdj.ocMostly.per = dr["per"].ToString();
                mdj.ocMostly.final_destination = dr["final_destination"].ToString();
                mdj.ocMostly.issues_state = dr["issues_state"].ToString();
                mdj.ocMostly.transport_style = dr["transport_style"].ToString();
                mdj.ocMostly.ship_date = dr["ship_date"].ToString();
                mdj.ocMostly.loading_port = dr["loading_port"].ToString();
                mdj.ocMostly.ap_id = dr["ap_id"].ToString();
                mdj.ocMostly.tranship_port = dr["tranship_port"].ToString();
                mdj.ocMostly.finally_buyer = dr["finally_buyer"].ToString();
                mdj.ocMostly.mo_group = dr["mo_group"].ToString();
                mdj.ocMostly.packinglistno = dr["packinglistno"].ToString();
                mdj.ocMostly.box_no = dr["box_no"].ToString();
                mdj.ocMostly.create_by = dr["create_by"].ToString();
                mdj.ocMostly.create_date = dr["create_date"].ToString();
                mdj.ocMostly.update_by = dr["update_by"].ToString();
                mdj.ocMostly.update_date = dr["update_date"].ToString();
                mdj.ocMostly.state = dr["state"].ToString();
                mdj.ocMostly.check_date = dr["check_date"].ToString();
                mdj.ocDetails.EditFlag = 0;
                mdj.ocDetails.sequence_id = dr["sequence_id"].ToString();
                mdj.ocDetails.mo_id = dr["mo_id"].ToString();
                mdj.ocDetails.goods_id = dr["goods_id"].ToString();
                lsInv.Add(mdj);
            }
            return lsInv;
        }
        public static List<so_other_fare> GetFareDataByID(string ID)
        {
            List<so_other_fare> lsInvFare = new List<so_other_fare>();
            string strSql = "";
            strSql = "Select a.id,a.Ver,b.sequence_id,b.mo_id,b.fare_id,b.name,b.tf_percent,b.sum_kind " +
                ",b.qty,b.price,b.fare_sum,b.remark,b.is_free" +
                " FROM so_invoice_mostly a" +
                " Inner Join so_invoice_other_fare b On a.within_code=b.within_code And a.id=b.id And a.Ver=b.Ver" +
                " Where a.within_code='" + within_code + "' And a.id='" + ID + "'";
            strSql += " Order By b.sequence_id ";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                so_other_fare mdj = new so_other_fare();
                mdj.EditFlag = 0;
                mdj.sequence_id = dr["sequence_id"].ToString();
                mdj.mo_id = dr["mo_id"].ToString();
                mdj.fare_id = dr["fare_id"].ToString();
                mdj.name = dr["name"].ToString();
                mdj.tf_percent = dr["tf_percent"] == null ? 0 : Convert.ToDecimal(dr["tf_percent"]);
                mdj.qty = dr["qty"] == null ? 0 : Convert.ToDecimal(dr["qty"]);
                mdj.price = dr["price"] == null ? 0 : Convert.ToDecimal(dr["price"]);
                mdj.fare_sum = dr["fare_sum"] == null ? 0 : Convert.ToDecimal(dr["fare_sum"]);
                mdj.remark = dr["remark"].ToString();
                mdj.is_free = dr["is_free"].ToString();
                lsInvFare.Add(mdj);
            }
            return lsInvFare;
        }
    }
}
