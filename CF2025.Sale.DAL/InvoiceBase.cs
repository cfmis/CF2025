using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CF.SQLServer.DAL;
using CF.Core.Config;
using CF.Framework.Contract;

namespace CF2025.Sale.DAL
{
    public static class InvoiceBase
    {
        private static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        private static string within_code = "0000";
        public static UpdateStatusModel GenDocNumber(string bill_id,string IDType,string bill_type_no,string bill_date)
        {
            UpdateStatusModel mdjID = new UpdateStatusModel();
            string bill_code = "";
            string MonthCode = MonthConvertToCode(bill_date);
            string year_month = bill_date.Substring(2, 2) + MonthCode;
            string strSql = "";
            strSql = "Select bill_code From sys_bill_max_separate Where within_code='" + within_code + "' And bill_id='" +
                bill_id + "' And year_month='" + year_month + "' And bill_text1='" + IDType + "' And bill_text2='" + bill_type_no + "'";
            bill_code = IDType + bill_type_no + year_month;
            DataTable dtID = sh.ExecuteSqlReturnDataTable(strSql);
            if (dtID.Rows.Count == 0)
            {
                bill_code = bill_code + "00001";//HC21C00006
                strSql = string.Format(@"Insert Into sys_bill_max_separate (within_code,bill_id,year_month,bill_code,bill_text1,bill_text2) Values " +
                    "('{0}','{1}','{2}','{3}','{4}','{5}')"
                    , within_code, bill_id, year_month, bill_code, IDType, bill_type_no);
            }
            else
            {
                string IDSeq = (Convert.ToInt32(dtID.Rows[0]["bill_code"].ToString().Substring(5, 5)) + 1).ToString().PadLeft(5, '0');
                bill_code = bill_code + IDSeq;//HC21C00006
                strSql = string.Format(@"Update sys_bill_max_separate set bill_code='{3}' Where " +
                    "within_code='{0}' And bill_id='{1}' And year_month='{2}' And bill_text1='{4}' And bill_text2='{5}'"
                    , within_code, bill_id, year_month, bill_code, IDType, bill_type_no);
            }
            //string result = sh.ExecuteSqlUpdate(strSql);
            mdjID.ReturnValue = bill_code;
            mdjID.ReturnValue1 = strSql;
            return mdjID;
        }
        private static string MonthConvertToCode(string bill_date)
        {
            string MonthCode = "";
            string Month1 = bill_date.Substring(5, 2);
            switch(Month1)
            {
                case "01":
                    MonthCode = "1";
                    break;
                case "02":
                    MonthCode = "2";
                    break;
                case "03":
                    MonthCode = "3";
                    break;
                case "04":
                    MonthCode = "4";
                    break;
                case "05":
                    MonthCode = "5";
                    break;
                case "06":
                    MonthCode = "6";
                    break;
                case "07":
                    MonthCode = "7";
                    break;
                case "08":
                    MonthCode = "8";
                    break;
                case "09":
                    MonthCode = "9";
                    break;
                case "10":
                    MonthCode = "A";
                    break;
                case "11":
                    MonthCode = "B";
                    break;
                case "12":
                    MonthCode = "C";
                    break;
            }
            return MonthCode;
        }
    }
}
