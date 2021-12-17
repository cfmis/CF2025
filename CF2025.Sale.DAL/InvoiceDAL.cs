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
            so_invoice_mostly objInv = new so_invoice_mostly();
            string strSql = "";
            strSql += " Select a.it_customer,a.m_id,a.merchandiser " +
                " From so_order_manage a " +
                " Inner Join so_order_details b On a.within_code=b.within_code And a.id=b.id And a.ver=b.ver " +
                " Where b.within_code='" + within_code + "' And b.mo_id='" + mo_id + "'";
            DataTable dtMostly = sh.ExecuteSqlReturnDataTable(strSql);
            if (dtMostly.Rows.Count > 0)
            {
                DataRow dr = dtMostly.Rows[0]; 
                objInv.it_customer = dr["it_customer"].ToString();
                objInv.m_id = dr["m_id"].ToString();
                objInv.merchandiser = dr["merchandiser"].ToString();
            }
            return objInv;
        }
    }
}
