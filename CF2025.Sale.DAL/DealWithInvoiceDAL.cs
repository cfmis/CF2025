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
using CF2025.Base.Contract;

namespace CF2025.Sale.DAL
{
    public static class DealWithInvoiceDAL
    {
        private static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        private static string within_code = "0000";
        private static string language_id = "1";
        public static List<view_so_invoice_mostly> FindInvoice(string it_customer_from, string it_customer_to
            , string oi_date_from, string oi_date_to, string sd_date_from, string sd_date_to, string ID, string mo_id, string state)
        {
            //List<viewOc> lsInv= new List<viewOc>();
            string strSql = "";
            strSql = "Select DISTINCT a.id,a.Ver,a.oi_date,a.it_customer,c.name AS cust_cname,a.return_state,a.separate" +
                ",d.name AS sent_goods_state,e.name AS transport_style,a.seller_id,a.consignment_date" +
                ",a.create_by,a.create_date,a.check_by,f.matter AS state" +
                " FROM so_invoice_mostly a" +
                " Inner Join so_invoice_details b On a.within_code=b.within_code And a.id=b.id And a.Ver=b.Ver" +
                " Left Join it_customer c On a.within_code=c.within_code And a.it_customer=c.id" +
                " Left Join cd_mo_type d On a.within_code=d.within_code And a.issues_state=d.id " +
                " Left Join cd_mo_type e On a.within_code=e.within_code And a.transport_style=e.id " +
                " Left Join sy_bill_state f On a.within_code=f.within_code And a.state=f.id" +
                " Where a.within_code='" + within_code + "'"
                + " And d.mo_type='A' And e.mo_type='B'" + " And f.language_id='" + language_id + "' ";
            if (it_customer_from != "" && it_customer_to != "")
                strSql += " And a.it_customer>='" + it_customer_from + "' And a.it_customer<='" + it_customer_to + "'";
            if (oi_date_from != "" && oi_date_to != "")
            {
                string oi_date_to1 = "";
                oi_date_to1 = Converter.ToCnDataString(Convert.ToDateTime(oi_date_to).AddDays(1));
                strSql += " And a.oi_date>='" + oi_date_from + "' And a.oi_date<'" + oi_date_to1 + "'";
            }
            if (ID != "")
                strSql += " And a.id='" + ID + "'";
            if (mo_id != "")
                strSql += " And b.mo_id='" + mo_id + "'";
            if (state == "01")
                strSql += " And a.state='1'";
            else if (state == "02")
                strSql += " And a.state>='2'";
            strSql += " Order By a.id ";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);//
            var lsInv = ConvertHelper.DataTableToList<view_so_invoice_mostly>(dt);
            return lsInv;
        }
        //發貨確認
        public static UpdateStatusModel ConfirmSent(List<InvoiceModel> InvoiceModel, string issues_state)
        {
            UpdateStatusModel mdj = new UpdateStatusModel();
            return mdj;
        }
        //public static List<viewOc> GetInvoiceByID(string ID)
        public static List<so_invoice_details> GetInvoiceByID(string ID)
        {
            //List<viewOc> lsInv= new List<viewOc>();
            string strSql = "";
            strSql = "Select a.id,a.Ver,a.sequence_id,a.mo_id,a.goods_id" +
                " FROM so_invoice_details a" +
                " Where a.within_code='" + within_code + "' And a.id='" + ID + "'";
            strSql += " Order By a.sequence_id ";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);//
            var so_invoice_details = ConvertHelper.DataTableToList<so_invoice_details>(dt);
            return so_invoice_details;
        }
    }
}
