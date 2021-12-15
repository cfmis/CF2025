using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace CF.Framework.Utility
{
    public static class ConvertJson
    {
        public static string DataTableJsonReturnExcel(DataTable dt)
        {
            StringBuilder json = new StringBuilder();
            StringBuilder jsonBuilder = new StringBuilder();
            json.Append("[");



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    string aa = WipeRiskString(dt.Rows[i][j].ToString().Trim());
                    jsonBuilder.Append(WipeRiskString(dt.Rows[i][j].ToString().Trim()));
                    jsonBuilder.Append("\",");
                }
                if (dt.Columns.Count > 0)
                {
                    jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                }
                jsonBuilder.Append("},");
            }
            if (dt.Rows.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }

            json.Append(jsonBuilder.ToString());
            json.Append("]");

            return json.ToString();
        }
        //去處非法的字符
        public static string WipeRiskString(string fstr)
        {
            string tstr = fstr;
            tstr = tstr.Replace("\r\n", "");
            tstr = tstr.Replace("\r", "");
            tstr = tstr.Replace("\n", "");
            tstr = tstr.Replace("\\", "");//SBL012647
            tstr = tstr.Replace("\u0002", "");//GBV043482
            tstr = tstr.Replace("\t", "");
            tstr = tstr.Replace("%", "");
            tstr = tstr.Replace("!", "");
            tstr = tstr.Replace("\"", "");
            tstr = tstr.Replace("”", "");
            tstr = tstr.Replace("“", "");
            //tstr = tstr.Replace(".", "");
            //tstr = tstr.Replace("~", "");
            tstr = tstr.Replace("{", "");
            tstr = tstr.Replace("}", "");
            tstr = tstr.Replace("?", "");
            return tstr;
        }
    }
}
