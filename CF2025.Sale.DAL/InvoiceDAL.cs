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
    public static class InvoiceDAL
    {
        private static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        private static string within_code = "0000";
        private static string temp_location_id = "ZZZ";
        private static string language_id = "1";
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
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                ModelBaseList obj = new ModelBaseList();
                obj.value = dr["id"].ToString();
                obj.label = dr["id"].ToString().Trim() + "--" + dr["name"].ToString().Trim();
                ls.Add(obj);
            }
            return ls;
        }
        public static so_invoice_mostly GetMostlyFromOc(string mo_id)
        {
            so_invoice_mostly mdjOc = new so_invoice_mostly();
            string strSql = "";
            strSql += " Select a.it_customer,a.seller_id,a.linkman,a.l_phone,a.fax,a.email,a.merchandiser,a.merchandiser_phone " +
                ",a.merchandiser_email,a.m_id,a.exchange_rate,a.port_id,a.ap_id,a.contract_id,a.p_id,a.pc_id,a.sm_id" +
                ",a.transport_rate,a.disc_rate,a.disc_amt,a.disc_spare,a.insurance_rate,a.other_fare,a.tax_ticket" +
                ",a.tax_sum,a.ship_mark,a.remark,a.area,a.m_rate" +
                " From so_order_manage a " +
                " Inner Join so_order_details b On a.within_code=b.within_code And a.id=b.id And a.ver=b.ver " +
                " Where b.within_code='" + within_code + "' And b.mo_id='" + mo_id + "'";
            DataTable dtMostly = sh.ExecuteSqlReturnDataTable(strSql);
            mdjOc = ConvertHelper.DataTableToModel<so_invoice_mostly>(dtMostly);
            return mdjOc;
        }

        public static so_order_details GetDetailsFromOc(string mo_id)
        {
            so_order_details objInv = new so_order_details();
            string strSql = "";
            strSql += " Select b.id AS order_id,b.ver AS so_ver,b.sequence_id AS so_sequence_id,a.it_customer,b.mo_id,b.goods_id" +
                ",b.table_head,b.customer_goods,b.customer_color_id,b.order_qty AS u_invoice_qty " +
                ",b.goods_unit,b.unit_price AS invoice_price,b.p_unit,b.disc_rate,c.name As goods_name,c.english_name As goods_ename" +
                ",d.name As color,b.contract_cid,e.name As big_class " +
                ",b.is_free,b.table_head,b.add_remark,b.customer_test_id " +
                " From so_order_manage a " +
                " Inner Join so_order_details b On a.within_code=b.within_code And a.id=b.id And a.ver=b.ver " +
                " Left Join it_goods c On b.within_code=c.within_code And b.goods_id=c.id " +
                " Left Join cd_color d On c.within_code=d.within_code And c.color=d.id " +
                " Left Join cd_goods_class e On c.within_code=e.within_code And c.big_class=e.id " +
                " Where b.within_code='" + within_code + "' And b.mo_id='" + mo_id + "'";
            DataTable dtOc = sh.ExecuteSqlReturnDataTable(strSql);
            objInv = ConvertHelper.DataTableToModel<so_order_details>(dtOc);
            List<so_other_fare> lsFare = new List<so_other_fare>();
            if (dtOc.Rows.Count > 0)
                lsFare = GetFareFromOc(dtOc.Rows[0]["order_id"].ToString().Trim(), dtOc.Rows[0]["mo_id"].ToString().Trim());
            objInv.ocOtherFare = lsFare;
            objInv.EditFlag = 1;
            objInv.location_id = "Y10";
            objInv.is_print = "Y";
            if (objInv.it_customer.Substring(0, 2) == "DO")
                objInv.goods_name = objInv.goods_ename;
            
            return objInv;
        }

        public static List<so_other_fare> GetFareFromOc(string ID,string mo_id)
        {
            string strSql = "";
            strSql += " Select f.fare_id,f.name,f.tf_percent,f.qty,f.price,f.fare_sum" +
                ",f.mo_id,f.mould_no,f.remark " +
                " From so_order_manage a " +
                " Inner Join so_order_details b On a.within_code=b.within_code And a.id=b.id And a.ver=b.ver " +
                " Left Join so_other_fare f On b.within_code=f.within_code And b.id=f.id And b.ver=f.ver And b.mo_id=f.mo_id " +
                " Where b.within_code='" + within_code + "' And b.id='" + ID + "' And b.mo_id='" + mo_id + "'";
            DataTable dtFare = sh.ExecuteSqlReturnDataTable(strSql);
            var lsFare = ConvertHelper.DataTableToList<so_other_fare>(dtFare);
            for(int i=0;i<lsFare.Count;i++)
            {
                lsFare[i].EditFlag = 1;
            }
            return lsFare;
        }
        public static UpdateStatusModel CheckGoodsQty(string ID, string mo_id, string goods_id, string u_invoice_qty, string goods_unit, string sec_qty, string location_id)
        {
            UpdateStatusModel mdjResult = new UpdateStatusModel();
            decimal u_invoice_qty_pcs = ConvertQtyToPCS(u_invoice_qty, goods_unit);
            //檢查發票數是否大於訂單數
            mdjResult = CheckGoodsOc(ID, mo_id, goods_id, u_invoice_qty_pcs);
            if (mdjResult.Status == "1")
                return mdjResult;
            //檢查倉存數
            decimal sec_qty1 = sec_qty != null ? Convert.ToDecimal(sec_qty) : 0;
            mdjResult = CheckGoodsStore(ID, mo_id, goods_id, location_id, u_invoice_qty_pcs, sec_qty1);
            if (mdjResult.Status == "1")
                return mdjResult;


            return mdjResult;
        }
        private static decimal ConvertQtyToPCS(string u_invoice_qty,string goods_unit)
        {
            decimal u_invoice_qty_pcs = 0;
            decimal qty_rate = Convert.ToDecimal(DataComboxList.GetQtyUnitRateList(goods_unit)[1].rate);
            u_invoice_qty_pcs = Convert.ToDecimal(u_invoice_qty) * qty_rate;
            return u_invoice_qty_pcs;
        }
        public static UpdateStatusModel ValidGoodsData(string ID,string mo_id, string goods_id, string u_invoice_qty, string goods_unit, string sec_qty,string location_id)
        {
            UpdateStatusModel mdjResult = new UpdateStatusModel();

            //檢查Mo是否存在檢驗分類編號
            mdjResult = CheckMoTestReport(ID, mo_id);
            if (mdjResult.Status == "1")
                return mdjResult;
            //檢查發票數是否大於訂單數、存倉數
            mdjResult = CheckGoodsQty( ID, mo_id, goods_id, u_invoice_qty, goods_unit, sec_qty, location_id);
            if (mdjResult.Status == "1")
                return mdjResult;

            return mdjResult;
        }
        private static UpdateStatusModel CheckCustStatus(string ID, string it_customer)
        {
            string strSql = "";
            //it_customer= "DC-A0073";
            UpdateStatusModel mdjResult = new UpdateStatusModel();
            strSql += " Select a.action_state,b.name AS action_state_name" +
                " From it_customer a " +
                " Left Join cd_mo_type b ON a.within_code=b.within_code AND a.action_state=b.id" +
                " Where a.within_code='" + within_code + "' And a.id='" + it_customer + "' And b.mo_type='Y'";
            DataTable dtCust = sh.ExecuteSqlReturnDataTable(strSql);
            if (dtCust.Rows.Count > 0)
            {
                DataRow drCust = dtCust.Rows[0];
                if (string.Compare(drCust["action_state"].ToString().Trim(), "0") > 0)
                {
                    mdjResult.Status = "1";
                    mdjResult.Msg = "客戶編號: " + it_customer + " 因:" + drCust["action_state_name"].ToString().Trim()
                        + "\r\n" + "不能出貨!";
                    mdjResult.ReturnValue = ID;
                    return mdjResult;
                }
            }
            return mdjResult;
        }
        private static UpdateStatusModel CheckMoTestReport(string ID, string mo_id)
        {
            string strSql = "";
            UpdateStatusModel mdjResult = new UpdateStatusModel();
            strSql += " Select a.brand_id,a.customer_test_id,b.is_qc_report" +
                " From so_order_details a " +
                " Left Join cd_brand b ON a.within_code=b.within_code AND a.brand_id=b.id" +
                " Where a.within_code='" + within_code + "' And a.mo_id='" + mo_id + "' ";
            DataTable dtRpt = sh.ExecuteSqlReturnDataTable(strSql);
            if(dtRpt.Rows.Count>0)
            {
                DataRow drRpt = dtRpt.Rows[0];
                if(drRpt["is_qc_report"].ToString().Trim()=="1")
                {
                    if (drRpt["customer_test_id"].ToString().Trim() == "")
                    {
                        mdjResult.Status = "1";
                        mdjResult.Msg = "制單編號: " + mo_id + " 檢驗分類編號不存在!"
                            + "\r\n" + "因為該制單的牌子:"+ drRpt["brand_id"].ToString().Trim() + "必須有檢驗報告才可出貨!";
                        mdjResult.ReturnValue = ID;
                        return mdjResult;
                    }
                }
                string customer_test_id = drRpt["customer_test_id"].ToString().Trim();//"DET-03-000002";
                if (customer_test_id != "")
                {
                    string now_date = Converter.ToCnDataString(System.DateTime.Now);
                    strSql = " Select a.effect" +
                        " From cd_customer_test_details a " +
                        " Where a.within_code='" + within_code + "' And a.customer_test_id='" + customer_test_id + "' " +
                        " And a.effect < '" + now_date + "'";
                    dtRpt = sh.ExecuteSqlReturnDataTable(strSql);
                    if (dtRpt.Rows.Count > 0)
                    {
                        mdjResult.Status = "1";
                        mdjResult.Msg = "制單編號: " + mo_id + " 檢驗分類編號報告已過期，不能出貨!"
                            + "\r\n" + "檢驗報告編號:" + customer_test_id;
                        mdjResult.ReturnValue = ID;
                        return mdjResult;
                    }
                }
            }
            return mdjResult;
        }
        private static UpdateStatusModel CheckGoodsOc(string ID,string mo_id,string goods_id,decimal u_invoice_qty_pcs)
        {
            string strSql = "";
            decimal s_invoice_qty_pcs = 0;
            decimal order_qty_pcs = 0;
            UpdateStatusModel mdjResult = new UpdateStatusModel();
            strSql = "Select Sum(b.u_invoice_qty*c.rate) AS s_invoice_qty_pcs " +
                " FROM so_invoice_mostly a" +
                " Inner Join so_invoice_details b On a.within_code=b.within_code And a.id=b.id And a.Ver=b.Ver" +
                " Inner Join it_coding c On b.within_code=c.within_code And b.goods_unit=c.unit_code " +
                " Where a.within_code='" + within_code + "' And b.mo_id='" + mo_id + "' And b.goods_id='" + goods_id +
                "' And a.id<>'" + ID + "' And a.state<>'V'" + " And b.state<>'V'" + " And c.id='*'";
            DataTable dtInv = sh.ExecuteSqlReturnDataTable(strSql);
            if (dtInv.Rows.Count > 0)
            {
                s_invoice_qty_pcs = dtInv.Rows[0]["s_invoice_qty_pcs"].ToString() == "" ? 0 : Convert.ToDecimal(dtInv.Rows[0]["s_invoice_qty_pcs"]);
            }
            strSql = "Select a.id,a.Ver,b.sequence_id,b.mo_id,b.goods_id,(b.order_qty*c.rate) AS order_qty_pcs,b.goods_unit " +
                " FROM so_order_manage a" +
                " Inner Join so_order_details b On a.within_code=b.within_code And a.id=b.id And a.Ver=b.Ver" +
                " Inner Join it_coding c On b.within_code=c.within_code And b.goods_unit=c.unit_code " +
                " Where a.within_code='" + within_code + "' And b.mo_id='" + mo_id + "' And b.goods_id='" + goods_id +
                "' And a.state<>'V'" + " And b.state<>'V'" + " And c.id='*'";
            DataTable dtOrder = sh.ExecuteSqlReturnDataTable(strSql);
            if (dtOrder.Rows.Count > 0)
            {
                order_qty_pcs = dtOrder.Rows[0]["order_qty_pcs"].ToString() == "" ? 0 : Convert.ToDecimal(dtOrder.Rows[0]["order_qty_pcs"]);
                if (u_invoice_qty_pcs + s_invoice_qty_pcs > order_qty_pcs)
                {
                    mdjResult.Status = "1";
                    mdjResult.Msg = "制單編號: " + mo_id + " 累計開發票數量已大於訂單數!"
                         + "\r\n" + "訂單數: " + Convert.ToInt32(order_qty_pcs).ToString() + " PCS"
                         + "\r\n" + "已開發票數: " + Convert.ToInt32(s_invoice_qty_pcs).ToString() + " PCS"
                         + "\r\n" + "待開發票數: " + Convert.ToInt32(u_invoice_qty_pcs).ToString() + " PCS";
                    mdjResult.ReturnValue = ID;
                    return mdjResult;
                }
            }
            else
            {
                mdjResult.Status = "1";
                mdjResult.Msg = "制單編號: " + mo_id + " 訂單記錄不存在!";
                mdjResult.ReturnValue = ID;
                return mdjResult;
            }
            return mdjResult;
        }
        private static UpdateStatusModel CheckGoodsStore(string ID,string mo_id,string goods_id,string location_id,decimal u_invoice_qty_pcs,decimal sec_qty)
        {
            decimal st_qty_pcs = 0;
            decimal st_sec_qty = 0;
            UpdateStatusModel mdjResult = new UpdateStatusModel();
            DataTable dtStore = GetGoodsStoreSum(mo_id, goods_id, location_id);
            if (dtStore.Rows.Count > 0)
            {
                st_qty_pcs = dtStore.Rows[0]["st_qty_pcs"].ToString() == "" ? 0 : Convert.ToDecimal(dtStore.Rows[0]["st_qty_pcs"]);
                st_sec_qty = dtStore.Rows[0]["st_sec_qty"].ToString() == "" ? 0 : Convert.ToDecimal(dtStore.Rows[0]["st_sec_qty"]);
                if (u_invoice_qty_pcs > st_qty_pcs)
                {
                    mdjResult.Status = "1";
                    mdjResult.Msg = "制單編號: " + mo_id + " 發票數量已大於倉存數!"
                         + "\r\n" + "倉存數: " + Convert.ToInt32(st_qty_pcs).ToString() + " PCS"
                         + "\r\n" + "發票數: " + Convert.ToInt32(u_invoice_qty_pcs).ToString() + " PCS"
                         + "\r\n" + "  差額: " + Convert.ToInt32(u_invoice_qty_pcs - st_qty_pcs).ToString() + " PCS";
                    mdjResult.ReturnValue = ID;
                    return mdjResult;
                }else if (sec_qty > st_sec_qty)
                {
                    mdjResult.Status = "1";
                    mdjResult.Msg = "制單編號: " + mo_id + " 發票重量已大於倉存重量!"
                         + "\r\n" + "倉存重量: " + Convert.ToInt32(st_sec_qty).ToString() + " KG"
                         + "\r\n" + "發票重量: " + Convert.ToInt32(sec_qty).ToString() + " KG"
                         + "\r\n" + "  差額: " + Convert.ToInt32(sec_qty - st_sec_qty).ToString() + " KG";
                    mdjResult.ReturnValue = ID;
                    return mdjResult;
                }
            }
            else
            {
                mdjResult.Status = "1";
                mdjResult.Msg = "制單編號: " + mo_id + " 倉存記錄不存在!";
                mdjResult.ReturnValue = ID;
                return mdjResult;
            }
            return mdjResult;
        }
        private static DataTable GetGoodsStoreSum(string mo_id, string goods_id, string location_id)
        {
            string strSql = "";
            strSql = "Select sum(a.qty) AS st_qty_pcs,sum(a.sec_qty) AS st_sec_qty " +
                 " FROM st_details_lot a" +
                 " Where a.within_code='" + within_code + "' And a.location_id='" + location_id + "' And a.carton_code='" + location_id
                 + "' And a.mo_id='" + mo_id + "' And a.goods_id='" + goods_id + "' And ( a.qty>0 Or a.sec_qty>0)";
            DataTable dtStore = sh.ExecuteSqlReturnDataTable(strSql);

            return dtStore;
        }
        public static decimal GetGoodsWegFromStore(string mo_id,string goods_id,string location_id,string u_invoice_qty,string goods_unit)
        {
            decimal sec_qty = 0;
            DataTable dtStore = GetGoodsStoreSum(mo_id, goods_id, location_id);
            if(dtStore.Rows.Count>0)
            {
                decimal u_invoice_qty_pcs = ConvertQtyToPCS(u_invoice_qty, goods_unit);
                decimal st_sec_qty = dtStore.Rows[0]["st_sec_qty"].ToString().Trim() != "" ? Convert.ToDecimal(dtStore.Rows[0]["st_sec_qty"]) : 0;
                decimal st_qty_pcs = dtStore.Rows[0]["st_qty_pcs"].ToString().Trim() != "" ? Convert.ToDecimal(dtStore.Rows[0]["st_qty_pcs"]) : 1;
                sec_qty = Math.Round((st_sec_qty / st_qty_pcs) * u_invoice_qty_pcs, 2);
            }
            return sec_qty;
        }
        public static UpdateStatusModel SaveInvoice(so_invoice_mostly InvMostly, List<so_invoice_details> InvDetails, List<so_other_fare> InvOtherFare)
        {
            string ID = InvMostly.ID == null ? "" : InvMostly.ID;
            UpdateStatusModel mdj = new UpdateStatusModel();
            DataTable dtMostly = CehckInvMostlyExist(ID);
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
            mdj = CheckCustStatus(ID, InvMostly.it_customer);
            if (mdj.Status == "1")
                return mdj;
            for (int i = 0; i < InvDetails.Count; i++)
            {
                if (InvDetails[i].EditFlag == 1)
                {
                    mdj = ValidGoodsData(ID, InvDetails[i].mo_id, InvDetails[i].goods_id, InvDetails[i].u_invoice_qty.ToString(), InvDetails[i].goods_unit, InvDetails[i].sec_qty.ToString(), InvDetails[i].location_id);
                    if (mdj.Status == "1")
                        return mdj;
                }
            }
            int Ver = InvMostly.Ver;
            string strSql = "";
            string strSql1 = "",strSql2="";
            string cust_name = "";
            string fake_bill_address = "";
            string cust_address = "";
            string fake_name = "";
            string fake_address = "";
            string bill_address = "";
            string cd_disc = "1";
            string flag = "0";
            string servername = "dgserver.cferp.dbo";
            string confirm_status = "0";
            strSql += string.Format(@" SET XACT_ABORT  ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");
            string payment_date = Converter.ConvertFieldToCnDataString(InvMostly.payment_date);
            string ship_date = Converter.ConvertFieldToCnDataString(InvMostly.ship_date);
            string create_date = Converter.ConvertFieldToCnDateTimeString(InvMostly.create_date);
            string update_date = Converter.ConvertFieldToCnDateTimeString(InvMostly.update_date);
            string check_date = Converter.ConvertFieldToCnDateTimeString(InvMostly.check_date);
            if (InvMostly.EditFlag==1)
            {
                if (dtMostly.Rows.Count==0)
                {
                    ID = InvoiceBase.GenDocNumber("SI01", InvMostly.issues_wh, InvMostly.bill_type_no, InvMostly.oi_date);
                    Ver = 0;
                    DataTable dtCust = GetCustData(InvMostly.it_customer);
                    DataRow drCust = dtCust.Rows[0];
                    cust_name= drCust["name"] != null ? drCust["name"].ToString().Trim() : "";
                    cust_name = cust_name.Contains("'") ? cust_name.Replace("'", "''") : cust_name;
                    fake_bill_address = " " + (drCust["fake_s_address"] != null ? drCust["fake_s_address"].ToString().Trim() : "")
                        + (InvMostly.linkman != null ? " ATTN:" + InvMostly.linkman : "")
                        + (InvMostly.l_phone != null ? " TEL:" + InvMostly.l_phone : "")
                        + (InvMostly.fax != null ? " FAX:" + InvMostly.fax : "")
                        + (InvMostly.email != null ? " EMAIL:" + InvMostly.email : "");
                    fake_bill_address = fake_bill_address.Contains("'") ? fake_bill_address.Replace("'", "''") : fake_bill_address;
                    cust_address = cust_name + fake_bill_address;
                    fake_name = drCust["fake_name"] != null ? drCust["fake_name"].ToString().Trim() : "";
                    fake_name = fake_name.Contains("'") ? fake_name.Replace("'", "''") : fake_name;
                    fake_address = fake_name + fake_bill_address;
                    bill_address = cust_address;
                    strSql1 += " Insert Into so_invoice_mostly (" +
                        " within_code,id,Ver,oi_date,separate,Shop_no,it_customer,phone,fax" +
                        ",linkman,l_phone,department_id,email,issues_wh,bill_type_no,merchandiser,merchandiser_phone,po_no,shipping_methods" +
                        ",seller_id,m_id,exchange_rate,goods_sum,other_fare,disc_rate,disc_amt,disc_spare,total_sum,tax_ticket" +
                        ",tax_sum,amount,other_fee,total_package_num,total_weight,remark2,ship_remark,ship_remark2,ship_remark3,remark" +
                        ",p_id,pc_id,sm_id,accounts,per,final_destination,issues_state,transport_style,loading_port" +
                        ",ap_id,tranship_port,finally_buyer,mo_group,packinglistno,box_no,create_by,update_by,state" +
                        ",name,address,fake_name,fake_bill_address,bill_address,fake_address,cd_disc,flag,servername" +
                        ",area,m_rate,confirm_status";
                    if (payment_date != "")
                    {
                        strSql1 += ",payment_date";
                        strSql2 += ",'{69}'";
                    }
                    if (ship_date != "")
                    {
                        strSql1 += ",ship_date";
                        strSql2 += ",'{70}'";
                    }
                    if (create_date != "")
                    {
                        strSql1 += ",create_date";
                        strSql2 += ",'{71}'";
                    }
                    if (update_date != "")
                    {
                        strSql1 += ",update_date";
                        strSql2 += ",'{72}'";
                    }
                    if (check_date != "")
                    {
                        strSql1 += ",check_date";
                        strSql2 += ",'{73}'";
                    }
                    strSql1 += ") Values (";
                    strSql1 += " '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'" +
                        ",'{10}','{11}','{12}','{13}','{14}','{15}','{16}', '{17}','{18}','{19}'" +
                        ",'{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}'" +
                        ",'{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}'" +
                        ",'{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}'" +
                        ",'{50}','{51}','{52}','{53}','{54}','{55}','{56}'" +
                        ",'{57}','{58}','{59}','{60}','{61}','{62}','{63}','{64}','{65}','{66}','{67}','{68}'";
                    strSql1 += strSql2;

                    strSql1 += ")";

                }
                else
                {
                    strSql1 += " Update so_invoice_mostly Set " +
                        " oi_date='{3}',separate='{4}',Shop_no='{5}',it_customer='{6}',phone='{7}',fax='{8}',linkman='{9}'" +
                        ",l_phone='{10}',department_id='{11}',email='{12}',issues_wh='{13}',bill_type_no='{14}',merchandiser='{15}',merchandiser_phone='{16}',po_no='{17}',shipping_methods='{18}'" +
                        ",seller_id='{19}',m_id='{20}',exchange_rate='{21}',goods_sum='{22}',other_fare='{23}',disc_rate='{24}',disc_amt='{25}',disc_spare='{26}',total_sum='{27}',tax_ticket='{28}'" +
                        ",tax_sum='{29}',amount='{30}',other_fee='{31}',total_package_num='{32}',total_weight='{33}',remark2='{34}',ship_remark='{35}',ship_remark2='{36}',ship_remark3='{37}',remark='{38}'" +
                        ",p_id='{39}',pc_id='{40}',sm_id='{41}',accounts='{42}',per='{43}',final_destination='{44}',issues_state='{45}',transport_style='{46}',loading_port='{47}'" +
                        ",ap_id='{48}',tranship_port='{49}',finally_buyer='{50}',mo_group='{51}',packinglistno='{52}',box_no='{53}',update_by='{55}'" +
                        ",area='{66}',m_rate='{67}'";
                    //,state='{56}',create_by='{54}',cd_disc='{62}',flag='{63}',servername='{64}'
                    //更新狀態，不再更新這些字段：,name='{57}',address='{58}',fake_bill_address='{59}',bill_address='{60}',fake_address='{61}'
                    if (payment_date != "")
                        strSql1 += ",payment_date='{69}'";
                    if (ship_date != "")
                        strSql1 += ",ship_date='{70}'";
                    if (create_date != "")
                        strSql1 += ",create_date='{71}'";
                    update_date= Converter.ToCnDateTimeString(System.DateTime.Now);
                    strSql1 += ",update_date='{72}'";
                    if (check_date != "")
                        strSql1 += ",check_date='{73}'";
                    strSql1 += " Where within_code='{0}' And id='{1}' And Ver='{2}' ";
                }
                strSql += string.Format(@strSql1
                    , within_code, ID, Ver, InvMostly.oi_date, InvMostly.separate, InvMostly.Shop_no, InvMostly.it_customer, InvMostly.phone, InvMostly.fax
                    , InvMostly.linkman, InvMostly.l_phone, InvMostly.department_id, InvMostly.email, InvMostly.issues_wh, InvMostly.bill_type_no, InvMostly.merchandiser, InvMostly.merchandiser_phone, InvMostly.po_no, InvMostly.shipping_methods
                    , InvMostly.seller_id, InvMostly.m_id, InvMostly.exchange_rate, InvMostly.goods_sum, InvMostly.other_fare, InvMostly.disc_rate, InvMostly.disc_amt, InvMostly.disc_spare, InvMostly.total_sum, InvMostly.tax_ticket
                    , InvMostly.tax_sum, InvMostly.amount, InvMostly.other_fee, InvMostly.total_package_num, InvMostly.total_weight, InvMostly.remark2, InvMostly.ship_remark, InvMostly.ship_remark2, InvMostly.ship_remark3, InvMostly.remark
                    , InvMostly.p_id, InvMostly.pc_id, InvMostly.sm_id, InvMostly.accounts, InvMostly.per, InvMostly.final_destination, InvMostly.issues_state, InvMostly.transport_style, InvMostly.loading_port
                    , InvMostly.ap_id, InvMostly.tranship_port, InvMostly.finally_buyer, InvMostly.mo_group, InvMostly.packinglistno, InvMostly.box_no, InvMostly.create_by, InvMostly.update_by, InvMostly.state
                    , cust_name, cust_address, fake_name, fake_bill_address, bill_address, fake_address
                    , cd_disc, flag, servername, InvMostly.area, InvMostly.m_rate, confirm_status
                    , payment_date, ship_date, create_date, update_date, check_date
                    );
            }
            if (InvDetails != null)
            {
                
                int MaxSeq = GetDetailsMaxSeq("so_invoice_details",ID, Ver);
                for (int i = 0; i < InvDetails.Count; i++)
                {
                    if (InvDetails[i].EditFlag == 1)
                    {
                        strSql1 = "";
                        string sequence_id = "";
                        if (InvDetails[i].sequence_id == null)
                        {
                            MaxSeq += 1;
                            sequence_id = MaxSeq.ToString().PadLeft(4, '0') + "h";
                        }
                        else
                            sequence_id = InvDetails[i].sequence_id;
                        if (!CheckInvDetailsExist("so_invoice_details", ID, Ver, sequence_id))
                            strSql1 += " Insert Into so_invoice_details (" +
                                "within_code,id,Ver,sequence_id,mo_id,shipment_suit,goods_id,table_head,goods_name,u_invoice_qty" +
                                ",goods_unit,sec_qty,sec_unit,invoice_price,p_unit,disc_rate,disc_price,total_sum,disc_amt,buy_id " +
                                ",order_id,issues_id,ref1,ref2,ncv,is_print,apprise_id,is_free,corresponding_code,nwt " +
                                ",gross_wt,package_num,box_no,subject_id,contract_cid,location_id " +
                                ",customer_goods,customer_color_id,remark,state,carton_code " +
                                ",so_ver,so_sequence_id,so_bom_sequence,ui_qty,under_value_price,dummy_location_id,dummy_carton_code" +
                                " )" +
                                " Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'" +
                                ",'{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}'" +
                                ",'{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}'" +
                                ",'{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}'" +
                                ",'{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}'" +
                                " )";
                        else
                            strSql1 += " Update so_invoice_details Set " +
                                " mo_id='{4}',shipment_suit='{5}',goods_id='{6}',table_head='{7}',goods_name='{8}',u_invoice_qty='{9}' " +
                                ",goods_unit='{10}',sec_qty='{11}',sec_unit='{12}',invoice_price='{13}',p_unit='{14}',disc_rate='{15}',disc_price='{16}',total_sum='{17}',disc_amt='{18}',buy_id='{19}'" +
                                ",order_id='{20}',issues_id='{21}',ref1='{22}',ref2='{23}',ncv='{24}',is_print='{25}',apprise_id='{26}',is_free='{27}',corresponding_code='{28}',nwt='{29}'" +
                                ",gross_wt='{30}',package_num='{31}',box_no='{32}',subject_id='{33}',contract_cid='{34}',location_id='{35}' " +
                                ",customer_goods='{36}',customer_color_id='{37}',remark='{38}' " +
                                ",carton_code='{40}',so_ver='{41}',so_sequence_id='{42}',so_bom_sequence='{43}',ui_qty='{44}'" +
                                ",under_value_price='{45}',dummy_location_id='{46}',dummy_carton_code='{47}'" +
                                " Where within_code='{0}' And id='{1}' And Ver='{2}' And sequence_id='{3}' ";

                        strSql += string.Format(@strSql1
                            , within_code, ID, Ver, sequence_id, InvDetails[i].mo_id, InvDetails[i].shipment_suit, InvDetails[i].goods_id
                            , InvDetails[i].table_head, InvDetails[i].goods_name, InvDetails[i].u_invoice_qty, InvDetails[i].goods_unit
                            , InvDetails[i].sec_qty, InvDetails[i].sec_unit, InvDetails[i].invoice_price, InvDetails[i].p_unit
                            , InvDetails[i].disc_rate, InvDetails[i].disc_price, InvDetails[i].total_sum, InvDetails[i].disc_amt, InvDetails[i].buy_id
                            , InvDetails[i].order_id, InvDetails[i].issues_id, InvDetails[i].ref1, InvDetails[i].ref2, InvDetails[i].ncv
                            , InvDetails[i].is_print, InvDetails[i].apprise_id, InvDetails[i].is_free, InvDetails[i].corresponding_code, InvDetails[i].nwt
                            , InvDetails[i].gross_wt, InvDetails[i].package_num, InvDetails[i].box_no
                            , InvDetails[i].subject_id, InvDetails[i].contract_cid, InvDetails[i].location_id
                            , InvDetails[i].customer_goods, InvDetails[i].customer_color_id, InvDetails[i].remark
                            , InvDetails[i].state, InvDetails[i].location_id, InvDetails[i].so_ver, InvDetails[i].so_sequence_id, InvDetails[i].so_bom_sequence
                            , InvDetails[i].u_invoice_qty, InvDetails[i].invoice_price, temp_location_id, temp_location_id
                            );
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
        private static DataTable GetCustData(string it_customer)
        {
            string strSql = "Select name,fake_name,fake_s_address From it_customer" +
                " Where within_code='" + within_code + "' And id='" + it_customer + "'";
            DataTable dtCust = sh.ExecuteSqlReturnDataTable(strSql);
            return dtCust;
        }
        private static DataTable CehckInvMostlyExist(string ID)
        {
            string strSql = "Select id,ver,state FROM so_invoice_mostly Where within_code='" + within_code + "' And id='" + ID + "'";
            DataTable dtInv = sh.ExecuteSqlReturnDataTable(strSql);
            return dtInv;
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

        //public static List<viewOc> GetInvoiceByID(string ID)
        public static viewInv GetInvoiceByID(string ID)
        {
            //List<viewOc> lsInv= new List<viewOc>();
            string strSql = "";
            strSql = "Select a.id,a.Ver,a.oi_date,a.separate,a.Shop_no,a.it_customer,a.phone,a.fax,a.payment_date,a.linkman" +
                ",a.l_phone,a.department_id,a.email,a.issues_wh,a.bill_type_no,a.merchandiser,a.merchandiser_phone,a.po_no" +
                ",a.shipping_methods,a.seller_id,a.m_id,a.exchange_rate,a.goods_sum,a.other_fare,a.disc_rate,a.disc_amt" +
                ",a.disc_spare,a.total_sum,a.tax_ticket,a.tax_sum,a.amount,a.other_fee,a.total_package_num,a.total_weight" +
                ",a.total_package_num,a.total_weight,a.remark2,a.ship_remark,a.ship_remark2,a.ship_remark3,a.remark,a.p_id" +
                ",a.pc_id,a.sm_id,a.accounts,a.per,a.final_destination,a.issues_state,a.transport_style,a.ship_date" +
                ",a.loading_port,a.ap_id,a.tranship_port,a.finally_buyer,a.mo_group,a.packinglistno,a.box_no,a.create_by" +
                ",a.create_date,a.update_by,a.update_date,a.state,f.matter,a.check_date" +
                ",a.m_rate,a.flag,a.as_id,a.area,a.confirm_status" +

                ",b.sequence_id,b.mo_id,shipment_suit,b.goods_id,b.table_head,b.goods_name,b.u_invoice_qty" +
                ",b.goods_unit,b.sec_qty,b.sec_unit,b.invoice_price,b.p_unit,b.disc_rate AS disc_rate_d,b.disc_price" +
                ",b.total_sum AS total_sum_d,b.disc_amt AS disc_amt_d,b.buy_id " +
                ",b.order_id,b.issues_id,b.ref1,b.ref2,b.ncv,b.is_print,b.apprise_id,b.is_free,b.corresponding_code,b.nwt " +
                ",b.gross_wt,b.package_num,b.box_no,b.subject_id,b.contract_cid,b.location_id " +
                ",b.customer_goods,b.customer_color_id,b.remark,b.carton_code" +
                ",b.so_ver,b.so_sequence_id,b.so_bom_sequence" +
                ",d.name As color,e.name As big_class" +
                " FROM so_invoice_mostly a" +
                " Inner Join so_invoice_details b On a.within_code=b.within_code And a.id=b.id And a.Ver=b.Ver" +
                " Inner Join it_goods c On b.within_code=c.within_code And b.goods_id=c.id " +
                " Inner Join cd_color d On c.within_code=d.within_code And c.color=d.id " +
                " Inner Join cd_goods_class e On c.within_code=e.within_code And c.big_class=e.id " +
                " Left  Join sy_bill_state f On a.within_code=f.within_code And a.state=f.id" +
                " Where a.within_code='" + within_code + "' And a.id='" + ID + "' And f.language_id='" + language_id + "'";
            strSql += " Order By b.sequence_id ";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);//
            viewInv lsInv = new viewInv();
            var mdjMostly = ConvertHelper.DataTableToModel<so_invoice_mostly>(dt);
            var mdjDetails = ConvertHelper.DataTableToList<so_invoice_details>(dt);
            lsInv.ocMostly = mdjMostly;
            lsInv.ocDetails = mdjDetails;
            lsInv.ocOtherFare = GetFareDataByID(ID);
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
            lsInvFare = ConvertHelper.DataTableToList<so_other_fare>(dt);
            return lsInvFare;
        }


        public static UpdateStatusModel DelInvoice(so_invoice_mostly InvMostly, string sequence_id, List<so_other_fare> InvOtherFare)
        {
            string ID = InvMostly.ID == null ? "" : InvMostly.ID;
            int Ver = InvMostly.Ver;
            string strSql = "";
            string strSql1 = "";
            strSql += string.Format(@" SET XACT_ABORT  ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");
            string update_date = Converter.ConvertFieldToCnDateTimeString(InvMostly.update_date);
            strSql1 += " Update so_invoice_mostly Set " +
                " goods_sum='{3}',other_fare='{4}',disc_rate='{5}',disc_amt='{6}',disc_spare='{7}',total_sum='{8}',tax_ticket='{9}'" +
                ",tax_sum='{10}',amount='{11}',other_fee='{12}',total_package_num='{13}',total_weight='{14}',update_date='{15}'";
            update_date = Converter.ToCnDateTimeString(System.DateTime.Now);
            strSql1 += "";
            strSql1 += " Where within_code='{0}' And id='{1}' And Ver='{2}' ";
            strSql += string.Format(@strSql1
                , within_code, ID, Ver, InvMostly.goods_sum, InvMostly.other_fare, InvMostly.disc_rate, InvMostly.disc_amt, InvMostly.disc_spare
                , InvMostly.total_sum, InvMostly.tax_ticket, InvMostly.tax_sum, InvMostly.amount
                , InvMostly.other_fee, InvMostly.total_package_num, InvMostly.total_weight, update_date
                );
            if (sequence_id != "")
            {
                strSql1 = "";
                strSql1 += " Delete From so_invoice_details " +
                    " Where within_code='{0}' And id='{1}' And Ver='{2}' And sequence_id='{3}' ";

                strSql += string.Format(@strSql1
                    , within_code, ID, Ver, sequence_id
                    );
            }
            if (InvOtherFare != null)
            {
                for (int i = 0; i < InvOtherFare.Count; i++)
                {
                    strSql += string.Format(@" Delete From so_invoice_other_fare " +
                        " Where within_code='{0}' And id='{1}' And Ver='{2}' And sequence_id='{3}' "
                        , within_code, ID, Ver, InvOtherFare[i].sequence_id);
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

        //批準
        public static UpdateStatusModel ApproveInvoice(so_invoice_mostly InvMostly, List<so_invoice_details> InvDetails, List<so_other_fare> InvOtherFare)
        {
            string ID = InvMostly.ID == null ? "" : InvMostly.ID;
            UpdateStatusModel mdj = new UpdateStatusModel();
            
            DataTable dtMostly = CehckInvMostlyExist(ID);
            if (dtMostly.Rows.Count > 0)
            {
                if (dtMostly.Rows[0]["state"].ToString().Trim() != "0")
                {
                    mdj.ReturnValue = ID;
                    mdj.Status = "1";
                    mdj.Msg = "該發票為非編輯狀態，不能批準!";
                    return mdj;
                }
            }
            mdj = CheckCustStatus(ID, InvMostly.it_customer);
            if (mdj.Status == "1")
                return mdj;
            mdj.ReturnValue = ID;
            for (int i = 0; i < InvDetails.Count; i++)
            {
                mdj = ValidGoodsData(ID, InvDetails[i].mo_id, InvDetails[i].goods_id, InvDetails[i].u_invoice_qty.ToString(), InvDetails[i].goods_unit, InvDetails[i].sec_qty.ToString(), InvDetails[i].location_id);
                if (mdj.Status == "1")
                    return mdj;
            }
            string strSql = "";
            string strSql1 = "";
            strSql += string.Format(@" SET XACT_ABORT  ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");
            string action_time = Converter.ConvertFieldToCnDataString(InvMostly.oi_date);
            string check_date = Converter.ToCnDateTimeString(System.DateTime.Now);
            for (int i = 0; i < InvDetails.Count; i++)
            {
                string sequence_id = InvDetails[i].sequence_id;
                decimal u_invoice_qty_pcs = 0;
                decimal qty_rate = Convert.ToDecimal(DataComboxList.GetQtyUnitRateList(InvDetails[i].goods_unit)[1].rate);
                u_invoice_qty_pcs = Math.Round(Convert.ToDecimal(InvDetails[i].u_invoice_qty) * qty_rate, 2);
                decimal sec_qty = Math.Round(Convert.ToDecimal(InvDetails[i].sec_qty), 2);
                string location_id = InvDetails[i].location_id;
                string mo_id = InvDetails[i].mo_id;
                string goods_id = InvDetails[i].goods_id;
                string goods_name = InvDetails[i].goods_name;
                string lot_no = "";
                string lot_no_new = ID + sequence_id;
                string lot_remark = "";
                decimal def_qty = u_invoice_qty_pcs;
                decimal def_sec_qty = sec_qty;
                DataTable dtStore = GetGoodsStore(location_id, mo_id, goods_id);
                for(int j=0;j<dtStore.Rows.Count;j++)
                {
                    
                    DataRow drStore = dtStore.Rows[j];
                    if (j == 0)
                        lot_no = drStore["lot_no"].ToString().Trim();
                    string lot_no1 = drStore["lot_no"].ToString().Trim();
                    decimal st_qty = Convert.ToDecimal(drStore["qty"]);
                    decimal st_sec_qty = Convert.ToDecimal(drStore["sec_qty"]);
                    decimal upd_qty = 0;
                    decimal upd_sec_qty = 0;
                    //20
                    //10
                    //8
                    //10
                    lot_remark += lot_no1;
                    if (def_qty >= st_qty)
                    {
                        upd_qty = 0;
                        lot_remark += "/@/" + st_qty.ToString().Trim();
                        def_qty = def_qty - st_qty;
                    }
                    else
                    {
                        upd_qty = st_qty - def_qty;
                        lot_remark += "/@/" + def_qty.ToString().Trim();
                        def_qty = 0;
                    }
                    if (def_sec_qty >= st_sec_qty)
                    {
                        upd_sec_qty = (decimal)0.00;
                        lot_remark += "/@/" + st_sec_qty.ToString().Trim();
                        def_sec_qty = def_sec_qty - st_sec_qty;
                    }
                    else
                    {
                        upd_sec_qty = st_sec_qty - def_sec_qty;
                        lot_remark += "/@/" + def_sec_qty.ToString().Trim();
                        def_sec_qty = (decimal)0.00;
                    }
                    lot_remark += ";";
                    strSql += string.Format(@" Update st_details_lot Set qty='{6}',sec_qty='{7}',out_date='{8}'" +
                        " Where within_code='{0}' And location_id='{1}' And carton_code='{2}' And goods_id='{3}'" +
                        " And lot_no='{4}' And mo_id='{5}'"
                        , within_code, location_id, location_id, goods_id, lot_no1, mo_id, upd_qty, upd_sec_qty, check_date);
                    if (def_qty == 0 && def_sec_qty == 0)
                        break;
                    lot_no = null;//當時分多批扣數時，將批號設置為null
                }
                //更新倉數到臨時倉
                if (!CheckGoodsStoreRecords(temp_location_id, mo_id, goods_id, lot_no_new))
                    strSql += string.Format(@"Insert Into st_details_lot (" +
                        " within_code,location_id,carton_code,goods_id,lot_no,goods_name,qty,sec_qty,average_cost,update_date" +
                        " ,state,remark,mo_id,vendor_id,in_date" +
                        " ) Values (" +
                        " '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'" +
                        ",'{10}','{11}','{12}','{13}','{14}' " +
                        " )"
                        , within_code, temp_location_id, temp_location_id, goods_id, lot_no_new, goods_name, u_invoice_qty_pcs, sec_qty
                        , 0, check_date, "0", temp_location_id, mo_id, "", check_date);
                else
                    strSql += string.Format(@" Update st_details_lot Set " +
                        " qty='{6}',sec_qty='{7}',update_date='{8}',in_date='{9}'" +
                        " Where within_code='{0}' And location_id='{1}' And carton_code='{2}' And goods_id='{3}' And lot_no='{4}' And mo_id='{5}'"
                        , within_code, temp_location_id, temp_location_id, goods_id, lot_no_new, mo_id, u_invoice_qty_pcs, sec_qty
                        , check_date, check_date);
                //插入流水帳記錄
                strSql1 += " Insert Into st_business_record (" +
                    " within_code,action_id,id,sequence_id,goods_id,goods_name,lot_no,unit,qty,base_unit" +
                    " ,ii_qty,ii_location_id,ii_code,ib_qty,sec_qty,sec_unit,action_time,check_date,transfers_state,mo_id" +
                    ",lot_remark";
                strSql1 += ") Values (";
                strSql1 += " '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'" +
                    ",'{10}','{11}','{12}','{13}','{14}','{15}','{16}', '{17}','{18}','{19}'" +
                    ",'{20}'";
                strSql1 += ")";
                //正式記錄插入到流水賬
                strSql += string.Format(@strSql1
                          , within_code, "25", ID, sequence_id, InvDetails[i].goods_id, goods_name, lot_no, InvDetails[i].goods_unit, u_invoice_qty_pcs, InvDetails[i].goods_unit
                          , InvDetails[i].u_invoice_qty, InvDetails[i].location_id, InvDetails[i].location_id, 1, InvDetails[i].sec_qty, InvDetails[i].sec_unit
                          , action_time, check_date, "0", mo_id, lot_remark
                          );
                //臨時倉的記錄插入到流水帳
                lot_remark = lot_no_new + "/@/" + u_invoice_qty_pcs.ToString().Trim() + "/@/" + sec_qty.ToString().Trim() + ";";
                strSql += string.Format(@strSql1
                          , within_code, "35", ID, sequence_id, InvDetails[i].goods_id, goods_name, lot_no_new, InvDetails[i].goods_unit, u_invoice_qty_pcs, InvDetails[i].goods_unit
                          , InvDetails[i].u_invoice_qty, temp_location_id, temp_location_id, 1, InvDetails[i].sec_qty, InvDetails[i].sec_unit
                          , action_time, check_date, "0", mo_id, lot_remark
                          );
                //將發票數更新到OC中:i_invoice_qty
                strSql += string.Format(@" Update so_order_details Set i_invoice_qty=i_invoice_qty+'{4}'" +
                    " Where within_code='{0}' And id='{1}' And ver='{2}' And sequence_id='{3}'"
                    , within_code, InvDetails[i].order_id, InvDetails[i].so_ver, InvDetails[i].so_sequence_id, u_invoice_qty_pcs
                 );
            }
            strSql += string.Format(@" Update so_invoice_mostly Set state='{3}',check_date='{4}'" +
                " Where within_code='{0}' And id='{1}' And ver='{2}'"
                 , within_code, ID, InvMostly.Ver, "1", check_date
                 );
            strSql += string.Format(@" COMMIT TRANSACTION ");
            string result = sh.ExecuteSqlUpdate(strSql);
            mdj.ReturnValue = ID;
            if (result == "")
            {
                mdj.Status = "0";
                mdj.Msg = "發票批準成功!";
            }
            else
            {
                mdj.Status = "1";
                mdj.Msg = result;
            }
            return mdj;
        }
        private static DataTable GetGoodsStore(string location_id,string mo_id,string goods_id)
        {
            string strSql = "";
            strSql = "Select lot_no,qty,sec_qty " +
                 " FROM st_details_lot a" +
                 " Where a.within_code='" + within_code + "' And a.location_id='" + location_id + "' And a.carton_code='" + location_id
                 + "' And a.mo_id='" + mo_id + "' And a.goods_id='" + goods_id + "' And (qty>0 Or sec_qty>0)";
            DataTable dtStore = sh.ExecuteSqlReturnDataTable(strSql);
            return dtStore;
        }
        private static bool CheckGoodsStoreRecords(string location_id, string mo_id, string goods_id,string lot_no)
        {
            bool result = false;
            string strSql = "";
            strSql = "Select lot_no,qty,sec_qty " +
                 " FROM st_details_lot a" +
                 " Where a.within_code='" + within_code + "' And a.location_id='" + location_id + "' And a.carton_code='" + location_id
                 + "' And a.mo_id='" + mo_id + "' And a.goods_id='" + goods_id + "' And lot_no='" + lot_no + "'";
            DataTable dtStore = sh.ExecuteSqlReturnDataTable(strSql);
            if (dtStore.Rows.Count > 0)
                result = true;
            return result;
        }

        //新版本
        public static UpdateStatusModel NewVersion(so_invoice_mostly InvMostly, List<so_invoice_details> InvDetails, List<so_other_fare> InvOtherFare)
        {
            string ID = InvMostly.ID == null ? "" : InvMostly.ID;
            UpdateStatusModel mdj = new UpdateStatusModel();
            DataTable dtMostly = CehckInvMostlyExist(ID);
            if (dtMostly.Rows.Count > 0)
            {
                if (dtMostly.Rows[0]["state"].ToString().Trim() != "1")
                {
                    mdj.ReturnValue = ID;
                    mdj.Status = "1";
                    mdj.Msg = "只有已批準的發票才可建立新版本!";
                    return mdj;
                }
            }
            string strSql = "";
            string strSql1 = "";
            int new_ver = InvMostly.Ver + 1;
            strSql += string.Format(@" SET XACT_ABORT  ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");
            
            string action_time = Converter.ConvertFieldToCnDataString(InvMostly.oi_date);
            string check_date = Converter.ToCnDateTimeString(System.DateTime.Now);
            for (int i = 0; i < InvDetails.Count; i++)
            {
                string sequence_id = InvDetails[i].sequence_id;
                decimal u_invoice_qty_pcs = 0;
                string mo_id = InvDetails[i].mo_id;
                string goods_id = InvDetails[i].goods_id;
                DataTable dtBus = GetBusinessRecords(ID, sequence_id, goods_id, "");
                for (int j = 0; j < dtBus.Rows.Count; j++)
                {

                    DataRow drBus = dtBus.Rows[j];
                    string action_id = drBus["action_id"].ToString().Trim();
                    string location_id = drBus["ii_location_id"].ToString().Trim();
                    string lot_remark = drBus["lot_remark"].ToString().Trim();
                    u_invoice_qty_pcs = Convert.ToInt32(drBus["qty"]);
                    string[] strSp = lot_remark.Split(';');
                    for (int k = 0; k < strSp.Length -1; k++)
                    {
                        string[] strSp1 = strSp[k].Split(new char[3] { '/', '@', '/' });
                        string lot_no1 = strSp1[0];
                        decimal upd_qty = Convert.ToDecimal(strSp1[3]);
                        decimal upd_sec_qty = Convert.ToDecimal(strSp1[6]);
                        strSql1 = " Update st_details_lot Set qty=qty+'{6}',sec_qty=sec_qty+'{7}'";
                        if (action_id == "25")
                            strSql1 += ",in_date='{8}'";
                        else
                        {
                            strSql1 += ",out_date ='{8}'";
                            upd_qty = 0 - upd_qty;
                            upd_sec_qty = 0 - upd_sec_qty;
                        }
                        strSql1 += " Where within_code='{0}' And location_id='{1}' And carton_code='{2}' And goods_id='{3}'" +
                            " And lot_no='{4}' And mo_id='{5}'";
                        strSql += string.Format(@strSql1
                        , within_code, location_id, location_id, goods_id, lot_no1, mo_id, upd_qty, upd_sec_qty, check_date);
                    }
                }
                //還原OC中的發票數:i_invoice_qty
                strSql += string.Format(@" Update so_order_details Set i_invoice_qty=i_invoice_qty-'{4}'" +
                    " Where within_code='{0}' And id='{1}' And ver='{2}' And sequence_id='{3}'"
                    , within_code, InvDetails[i].order_id, InvDetails[i].so_ver, InvDetails[i].so_sequence_id, u_invoice_qty_pcs
                 );
                //刪除交易記錄表的記錄
                strSql += string.Format(@" Delete FROM st_business_record " +
                        " Where within_code='{0}' And id='{1}' And sequence_id='{2}' And goods_id='{3}'"
                        , within_code, ID, sequence_id, goods_id);
                
            }
            //更新發票表頭的版本號
            strSql += string.Format(@" Update so_invoice_mostly Set ver='{3}',state='{4}',check_date=null" +
                " Where within_code='{0}' And id='{1}' And ver='{2}'"
                 , within_code, ID, InvMostly.Ver, new_ver, "0"
                 );
            //更新發票記錄的版本號
            strSql += string.Format(@" Update so_invoice_details Set ver='{3}'" +
                " Where within_code='{0}' And id='{1}' And ver='{2}'"
                , within_code, ID, InvMostly.Ver, new_ver);
            //更新發票附加費記錄的版本號
            strSql += string.Format(@" Update so_invoice_other_fare Set ver='{3}'" +
                " Where within_code='{0}' And id='{1}' And ver='{2}'"
                , within_code, ID, InvMostly.Ver, new_ver);
            strSql += string.Format(@" COMMIT TRANSACTION ");
            string result = sh.ExecuteSqlUpdate(strSql);
            mdj.ReturnValue = ID;
            if (result == "")
            {
                mdj.Status = "0";
                mdj.Msg = "發票批準成功!";
            }
            else
            {
                mdj.Status = "1";
                mdj.Msg = result;
            }
            return mdj;
        }
        private static DataTable GetBusinessRecords(string ID,string sequence_id,string goods_id,string action_id)
        {
            string strSql = "";
            strSql = "Select a.* " +
                 " FROM st_business_record a" +
                 " Where a.within_code='" + within_code + "' And a.id='" + ID + "' And a.sequence_id='" + sequence_id
                  + "' And a.goods_id='" + goods_id + "'";
            if (action_id != "")
                strSql += " And a.action_id='" + action_id + "'";
            DataTable dtBus = sh.ExecuteSqlReturnDataTable(strSql);
            return dtBus;
        }

        //發貨確認
        public static UpdateStatusModel ConfirmSent(List<so_invoice_details> InvDetails,string issues_state)
        {
            UpdateStatusModel mdj = new UpdateStatusModel();
            string ID = InvDetails[0].ID;
            DataTable dtMostly = CehckInvMostlyExist(ID);
            if (dtMostly.Rows.Count > 0)
            {
                if (dtMostly.Rows[0]["state"].ToString().Trim() != "1")
                {
                    mdj.ReturnValue = ID;
                    mdj.Status = "1";
                    mdj.Msg = "只有已做批準的發票才可以執行發貨確認!";
                    return mdj;
                }
            }
            int Ver = InvDetails[0].Ver;
            string strSql = "";
            string strSql1 = "";
            strSql += string.Format(@" SET XACT_ABORT  ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");
            string check_date = Converter.ToCnDateTimeString(System.DateTime.Now);
            for (int i = 0; i < InvDetails.Count; i++)
            {
                string sequence_id = InvDetails[i].sequence_id;
                string mo_id = InvDetails[i].mo_id;
                string goods_id = InvDetails[i].goods_id;
                string action_id = "35";
                DataTable dtBus = GetBusinessRecords(ID, sequence_id, goods_id, action_id);
                for (int j = 0; j < dtBus.Rows.Count; j++)
                {

                    DataRow drBus = dtBus.Rows[j];
                    string location_id = drBus["ii_location_id"].ToString().Trim();
                    string lot_remark = drBus["lot_remark"].ToString().Trim();
                    string[] strSp = lot_remark.Split(';');
                    for (int k = 0; k < strSp.Length - 1; k++)
                    {
                        string[] strSp1 = strSp[k].Split(new char[3] { '/', '@', '/' });
                        string lot_no1 = strSp1[0];
                        decimal upd_qty = Convert.ToDecimal(strSp1[3]);
                        decimal upd_sec_qty = Convert.ToDecimal(strSp1[6]);
                        strSql1 = " Update st_details_lot Set qty=qty-'{6}',sec_qty=sec_qty-'{7}',out_date='{8}'" +
                            " Where within_code='{0}' And location_id='{1}' And carton_code='{2}' And goods_id='{3}'" +
                            " And lot_no='{4}' And mo_id='{5}'";
                        strSql += string.Format(@strSql1
                        , within_code, location_id, location_id, goods_id, lot_no1, mo_id, upd_qty, upd_sec_qty, check_date);
                    }
                }
                //刪除交易記錄表中的臨時記錄
                strSql += string.Format(@" Delete FROM st_business_record " +
                        " Where within_code='{0}' And action_id='{1}' And id='{2}' And sequence_id='{3}' And goods_id='{4}'"
                        , within_code, action_id, ID, sequence_id, goods_id);

            }
            //更新發票表頭的狀態
            strSql += string.Format(@" Update so_invoice_mostly Set state='{3}',issues_state='{4}',consignment_date='{5}'" +
                " Where within_code='{0}' And id='{1}' And ver='{2}'"
                 , within_code, ID, Ver, "7", issues_state, check_date
                 );
            strSql += string.Format(@" COMMIT TRANSACTION ");
            string result = sh.ExecuteSqlUpdate(strSql);
            mdj.ReturnValue = ID;
            if (result == "")
            {
                mdj.Status = "0";
                mdj.Msg = "發貨批準成功!";
            }
            else
            {
                mdj.Status = "1";
                mdj.Msg = result;
            }
            return mdj;
        }
        //取消發貨
        public static UpdateStatusModel CancelSent(List<so_invoice_details> InvDetails)
        {
            UpdateStatusModel mdj = new UpdateStatusModel();
            string ID = InvDetails[0].ID;
            DataTable dtMostly = CehckInvMostlyExist(ID);
            if (dtMostly.Rows.Count > 0)
            {
                if (dtMostly.Rows[0]["state"].ToString().Trim() != "7")
                {
                    mdj.ReturnValue = ID;
                    mdj.Status = "1";
                    mdj.Msg = "只有已做發貨確認的發票才可執行取消發貨!";
                    return mdj;
                }
            }
            int Ver = InvDetails[0].Ver;
            
            string strSql = "";
            string strSql1 = "";
            strSql += string.Format(@" SET XACT_ABORT  ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");
            for (int i = 0; i < InvDetails.Count; i++)
            {
                string sequence_id = InvDetails[i].sequence_id;
                string mo_id = InvDetails[i].mo_id;
                string goods_id = InvDetails[i].goods_id;
                string goods_name = InvDetails[i].goods_name;
                string action_id = "25";
                string lot_no = ID + sequence_id;
                string location_id = temp_location_id;
                decimal upd_qty = 0;
                decimal upd_sec_qty = 0;
                DataTable dtBus = GetBusinessRecords(ID, sequence_id, goods_id, action_id);
                string action_time = "";
                string check_date = "";
                string lot_remark = "";
                for (int j = 0; j < dtBus.Rows.Count; j++)
                {

                    DataRow drBus = dtBus.Rows[j];
                    action_time = Converter.ConvertFieldToCnDataString(drBus["action_time"].ToString());
                    check_date = Converter.ConvertFieldToCnDateTimeString(drBus["check_date"].ToString());
                    lot_remark = drBus["lot_remark"].ToString().Trim();
                    string[] strSp = lot_remark.Split(';');
                    for (int k = 0; k < strSp.Length - 1; k++)
                    {
                        string[] strSp1 = strSp[k].Split(new char[3] { '/', '@', '/' });
                        upd_qty += Convert.ToDecimal(strSp1[3]);
                        upd_sec_qty += Convert.ToDecimal(strSp1[6]);
                    }

                    //更新倉數到臨時倉
                    string check_date1 = Converter.ToCnDateTimeString(System.DateTime.Now);
                    if (!CheckGoodsStoreRecords(temp_location_id, mo_id, goods_id, lot_no))
                        strSql += string.Format(@"Insert Into st_details_lot (" +
                            " within_code,location_id,carton_code,goods_id,lot_no,goods_name,qty,sec_qty,average_cost,update_date" +
                            " ,state,remark,mo_id,vendor_id,in_date" +
                            " ) Values (" +
                            " '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'" +
                            ",'{10}','{11}','{12}','{13}','{14}' " +
                            " )"
                            , within_code, temp_location_id, temp_location_id, goods_id, lot_no, goods_name, upd_qty, upd_sec_qty
                            , 0, check_date1, "0", temp_location_id, mo_id, "", check_date1);
                    else
                        strSql += string.Format(@" Update st_details_lot Set " +
                            " qty='{6}',sec_qty='{7}',update_date='{8}',in_date='{9}'" +
                            " Where within_code='{0}' And location_id='{1}' And carton_code='{2}' And goods_id='{3}' And lot_no='{4}' And mo_id='{5}'"
                            , within_code, temp_location_id, temp_location_id, goods_id, lot_no, mo_id, upd_qty, upd_sec_qty
                            , check_date1, check_date1);
                    //插入流水帳記錄
                    strSql1 += " Insert Into st_business_record (" +
                        " within_code,action_id,id,sequence_id,goods_id,goods_name,lot_no,unit,qty,base_unit" +
                        " ,ii_qty,ii_location_id,ii_code,ib_qty,sec_qty,sec_unit,action_time,check_date,transfers_state,mo_id" +
                        ",lot_remark";
                    strSql1 += ") Values (";
                    strSql1 += " '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'" +
                        ",'{10}','{11}','{12}','{13}','{14}','{15}','{16}', '{17}','{18}','{19}'" +
                        ",'{20}'";
                    strSql1 += ")";
                    //臨時倉的記錄插入到流水帳
                    lot_remark = lot_no + "/@/" + upd_qty.ToString().Trim() + "/@/" + upd_sec_qty.ToString().Trim() + ";";
                    strSql += string.Format(@strSql1
                              , within_code, "35", ID, sequence_id, goods_id, goods_name, lot_no, drBus["unit"].ToString().Trim()
                              , upd_qty, drBus["base_unit"].ToString().Trim()
                              , drBus["ii_qty"].ToString(), temp_location_id, temp_location_id, 1, upd_sec_qty, drBus["sec_unit"].ToString().Trim()
                              , action_time, check_date, "0", mo_id, lot_remark
                              );
                }
            }
            //更新發票為已批準的狀態
            strSql += string.Format(@" Update so_invoice_mostly Set state='{3}',issues_state='',consignment_date=null" +
                " Where within_code='{0}' And id='{1}' And ver='{2}'"
                 , within_code, ID, Ver, "1"
                 );
            strSql += string.Format(@" COMMIT TRANSACTION ");
            string result = sh.ExecuteSqlUpdate(strSql);
            mdj.ReturnValue = ID;
            if (result == "")
            {
                mdj.Status = "0";
                mdj.Msg = "發貨取消成功!";
            }
            else
            {
                mdj.Status = "1";
                mdj.Msg = result;
            }
            return mdj;
        }


        //取消發貨
        public static UpdateStatusModel CancelDoc(string ID)
        {
            UpdateStatusModel mdj = new UpdateStatusModel();
            mdj.ReturnValue = ID;
            DataTable dtInv = CehckInvMostlyExist(ID);
            if (dtInv.Rows.Count > 0)
            {
                string state = dtInv.Rows[0]["state"].ToString().Trim();
                if (state == "1")
                {
                    mdj.Status = "1";
                    mdj.Msg = "已批準的發票不能進行注銷操作!";
                    return mdj;
                }
                else if (state == "7")
                {
                    mdj.Status = "1";
                    mdj.Msg = "已發貨的發票不能進行注銷操作!";
                    return mdj;
                }
                else if (state == "2")
                {
                    mdj.Status = "1";
                    mdj.Msg = "已注銷的發票不能再進行此操作!";
                    return mdj;
                }
            }
            else
            {
                mdj.Status = "1";
                mdj.Msg = "發票記錄不存在!";
                return mdj;
            }
            int Ver = Convert.ToInt32(dtInv.Rows[0]["ver"]);
            string strSql = "";
            strSql += string.Format(@" SET XACT_ABORT  ON ");
            strSql += string.Format(@" BEGIN TRANSACTION ");

            string update_date = Converter.ToCnDateTimeString(System.DateTime.Now);
            //更新發票為已注銷的狀態
            strSql += string.Format(@" Update so_invoice_mostly Set state='{3}',update_date='{4}'" +
                " Where within_code='{0}' And id='{1}' And ver='{2}'"
                 , within_code, ID, Ver, "2", update_date
                 );
            strSql += string.Format(@" COMMIT TRANSACTION ");
            string result = sh.ExecuteSqlUpdate(strSql);
            mdj.ReturnValue = ID;
            if (result == "")
            {
                mdj.Status = "0";
                mdj.Msg = "發票注銷成功!";
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
