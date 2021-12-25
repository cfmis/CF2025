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
                    strSql += "Select id,name,name As english_name From sys_bill_origin Where function_id='SO01' AND language='3' Order By id";
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
        public static so_invoice_mostly GetDataMostly(string mo_id)
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
        public static viewOc GetDataDetails(string mo_id)
        {
            viewOc objInv = new viewOc();
            string strSql = "";
            strSql += " Select b.id,a.it_customer,b.goods_id,b.table_head,b.customer_goods,b.customer_color_id,b.order_qty " +
                ",b.goods_unit,b.unit_price,b.p_unit,b.disc_rate,c.name As goods_name,d.name As color,b.contract_cid,e.name As big_class " +
                ",b.is_free,b.table_head,b.add_remark,b.customer_test_id " +
                " From so_order_manage a " +
                " Inner Join so_order_details b On a.within_code=b.within_code And a.id=b.id And a.ver=b.ver " +
                " Left Join it_goods c On b.within_code=c.within_code And b.goods_id=c.id " +
                " Left Join cd_color d On c.within_code=d.within_code And c.color=d.id " +
                " Left Join cd_goods_class e On c.within_code=e.within_code And c.big_class=e.id " +
                " Where b.within_code='" + within_code + "' And b.mo_id='" + mo_id + "'";
            DataTable dtMostly = sh.ExecuteSqlReturnDataTable(strSql);
            if (dtMostly.Rows.Count > 0)
            {
                DataRow dr = dtMostly.Rows[0];
                objInv.ocMostly.it_customer = dr["it_customer"].ToString().Trim();
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
            }
            return objInv;
        }
    }
}
