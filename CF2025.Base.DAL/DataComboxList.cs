using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CF.Framework.Contract;
using CF.SQLServer.DAL;
using CF.Core.Config;

namespace CF2025.Base.DAL
{
    
    public static class DataComboxList
    {
        private static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        public static List<ModelBaseList> GetComboxList(string SourceType)
        {
            string strSql = "";
            string within_code = "0000";
            switch (SourceType)
            {
                case "QtyUnitList"://單位
                    strSql += "Select id,name,english_name From cd_units Where kind='05' Order By id";
                    break;
                case "SalesmanList"://營業員&跟單員
                    strSql += "Select id,id+'--'+name As name,english_name From cd_personnel Where within_code='" + within_code + "' And sales_group is not null and state='0' Order By id";
                    break;
                case "CurrList"://貨幣代號
                    strSql += "Select id,id+'--'+name As name,english_name From cd_money Where within_code='" + within_code + "' Order By id";
                    break;
                case "AccountList"://銀行賬號
                    strSql += "Select abbreviate As id,accounts As name,accounts As english_name From cd_company_accounts Where within_code='" + within_code + "' Order By abbreviate";
                    break;
                case "MoGroupList"://負責組別
                    strSql += "Select id,id+'--'+name As name,english_name From cd_mo_type Where within_code='" + within_code + "' And mo_type='3' Order By id";
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
                obj.label = dr["name"].ToString().Trim();
                ls.Add(obj);
            }
            return ls;
        }
    }
}
