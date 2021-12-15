using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CF.Core.Config;

namespace CF.SQLServer.DAL
{
    public class GenNumberDAL
    {
        SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.Crm);
        public string GetOcDetailsSeq(string tbName,string ID)
        {
            string result = "";
            string strSql = "";
            strSql = "Select MAX(Seq) AS Seq FROM " + tbName + " Where OcID='" + ID + "'";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
                result = dt.Rows[0]["Seq"].ToString() != "" ? (Convert.ToInt32(dt.Rows[0]["Seq"]) + 1).ToString().PadLeft(3, '0') : "001";
            else
                result = "001";
            return result;
        }
        public string GetDetailsSeq(string tbName, string ID)
        {
            string result = "";
            string strSql = "";
            strSql = "Select MAX(Seq) AS Seq FROM " + tbName + " Where ID='" + ID + "'";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
                result = dt.Rows[0]["Seq"].ToString() != "" ? (Convert.ToInt32(dt.Rows[0]["Seq"]) + 1).ToString().PadLeft(3, '0') : "001";
            else
                result = "001";
            return result;
        }

        public string GenTransferID(string flag0,string LocID,string NextLocID,string flag2)
        {
            
            string result = "";
            string strSql = "";
            string BillID = "WM01";
            string BillCode = "";
            string DocType = "";
            string ToFlag = flag2.ToString().Trim();
            if (ToFlag != "TO")
            {
                DocType = flag0.Trim() + LocID.Trim();
            }
            else
            {
                DocType = flag0.Trim() + LocID.Trim() + NextLocID.Trim();
            }
            strSql = "Select bill_code FROM sys_bill_max Where bill_id='" + BillID + "' And bill_text1='" + DocType + "'";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                BillCode = dt.Rows[0]["bill_code"].ToString();
                if (ToFlag != "TO")
                    BillCode = DocType + (Convert.ToInt32(BillCode.Substring(4, 8)) + 1).ToString().PadLeft(8, '0');
                else
                    BillCode = DocType + (Convert.ToInt32(BillCode.Substring(7, 5)) + 1).ToString().PadLeft(5, '0');
                strSql = "Update sys_bill_max Set bill_code='" + BillCode + "'" +
                    " Where bill_id='" + BillID + "' And bill_text1='" + DocType + "'";
            }
            else
            {
                if (ToFlag != "TO")
                    BillCode = DocType + "00000001";//TV0100000001
                else
                    BillCode = DocType + "00001";//TV01V0200001
                strSql = "Insert Into sys_bill_max (bill_id,bill_code,bill_text1) Values " +
                    "('" + BillID + "','" + BillCode + "','" + DocType + "')";
            }
            result = sh.ExecuteSqlUpdate(strSql);
            result = BillCode;
            return result;
        }
    }
}
