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
                case "LocationList"://倉庫
                    strSql += "SELECT id,id+'--'+ name AS name FROM cd_productline WHERE within_code='"+ within_code + "' AND state='0' AND id<>'ZZZ' ORDER BY id";
                    break;                
                case "CustomerList"://客戶編號
                    strSql += "SELECT id,id+'--' + name AS name FROM it_customer WHERE within_code='" + within_code + "' AND customer_group='1' AND state<>'2' ORDER BY id";
                    break;
                default:
                    strSql += "";
                    break;
            }
            ModelBaseList obj1 = new ModelBaseList();
            obj1.value = "";
            obj1.label = "";
            List<ModelBaseList> lst = new List<ModelBaseList>();
            lst.Add(obj1);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                ModelBaseList obj = new ModelBaseList();
                obj.value = dr["id"].ToString();
                obj.label = dr["name"].ToString().Trim();
                lst.Add(obj);
            }
            return lst;
        }

        /// <summary>
        /// 倉位取倉位資料
        /// </summary>
        /// <param name="location_id"></param>
        /// <returns></returns>
        public static List<ModelBaseList> GetCartonCodeList(string location_id)
        {
            string within_code = "0000";
            string strSql = string.Format(
                @"SELECT S.* 
                  FROM (SELECT id, name FROM cd_carton_code WHERE within_code='{0}' and location_id='{1}' AND id<>'ZZZ') S
                  WHERE S.name NOT LIKE '%臨時%' 
                  ORDER BY S.id", within_code, location_id);            
            ModelBaseList obj1 = new ModelBaseList();
            obj1.value = "";
            obj1.label = "";
            List<ModelBaseList> lst = new List<ModelBaseList>();
            lst.Add(obj1);
            DataTable dtWh = sh.ExecuteSqlReturnDataTable(strSql);
            for (int i = 0; i < dtWh.Rows.Count; i++)
            {
                DataRow dr = dtWh.Rows[i];
                ModelBaseList obj = new ModelBaseList();
                obj.value = dr["id"].ToString();
                obj.label = dr["name"].ToString().Trim();
                lst.Add(obj);
            }
            return lst;
        }
    }
}
