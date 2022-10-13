using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CF.Framework.Contract;
using CF2025.Base.Contract;
using CF.SQLServer.DAL;
using CF.Core.Config;

namespace CF2025.Base.DAL
{
    
    public static class DataComboxList
    {
        private static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        private static string within_code="0000";
        public static List<ModelBaseList> GetComboxList(string SourceType, string language_id)
        {
            string strSql = "";
            string LanguageID = sh.ConvertLanguage(language_id);
            switch (SourceType)
            {
                case "QtyUnitList"://數量單位
                    strSql += "SELECT id,name,english_name FROM cd_units WHERE kind='05' Order By id";
                    break;
                case "WegUnitList"://重量單位
                    strSql += "SELECT id,name,english_name FROM cd_units WHERE kind='03' Order By id";                    
                    break;
                case "SalesmanList"://營業員&跟單員
                    strSql += "SELECT id,id+'--'+name As name,english_name FROM cd_personnel WHERE within_code='" + within_code + "' AND sales_group is not null AND state='0' Order By id";
                    break;
                case "CurrList"://貨幣代號
                    strSql += "SELECT id,id+'--'+name As name,english_name FROM cd_money WHERE within_code='" + within_code + "' Order By id";
                    break;
                case "AccountList"://銀行賬號
                    strSql += "SELECT abbreviate As id,accounts As name,accounts As english_name FROM cd_company_accounts WHERE within_code='" + within_code + "' Order By abbreviate";
                    break;
                case "MoGroupList"://負責組別
                    strSql += "SELECT id,id+'--'+name As name,english_name FROM cd_mo_type WHERE within_code='" + within_code + "' AND mo_type='3' Order By id";
                    break;
                case "LocationList"://倉庫
                    strSql += "SELECT id,id+'--'+ name AS name FROM cd_productline WHERE within_code='"+ within_code + "' AND state='0' AND id<>'ZZZ' ORDER BY id";
                    break;                
                case "CustomerList"://客戶編號
                    strSql += "SELECT id,id+'--' + name AS name FROM it_customer WHERE within_code='" + within_code + "' AND customer_group='1' AND state<>'2' ORDER BY id";
                    break;
                case "TackFareList"://附加費用
                    strSql += "SELECT id,name,english_name FROM cd_tack_fare WHERE within_code='" + within_code + "' Order By id";
                    break;
                case "issues_state_list"://發貨狀態
                    strSql += "SELECT id,id+'--' + name AS name FROM cd_mo_type WHERE within_code='" + within_code + "' AND mo_type='A' ORDER BY id";
                    break;
                case "DeptList"://部門編碼
                    strSql += "SELECT id,id+'--' + name AS name FROM cd_department WHERE within_code='" + within_code + "' AND LEN(id)<=4 AND state<>'2' ORDER BY id";
                    break;
                case "StateList"://狀態 
                    strSql += string.Format(@"SELECT id, matter as name FROM sy_bill_state WHERE language_id = '{0}'", LanguageID); ;
                    break;
                case "BigClass"://大類
                    strSql += "SELECT id,id+'--'+name AS name FROM cd_goods_class WHERE within_code='" + within_code + "' AND gc_type='0' AND state='0' ORDER BY id";
                    break;
                case "BaseClass"://中類(產品類型,貨品編碼的第3,4位)
                    strSql += "SELECT DISTINCT id,id+'--'+name AS name FROM cd_goods_class WHERE within_code='" + within_code + "' AND gc_type='1' AND state='0' ORDER BY id";
                    break;
                case "SmallClass"://小類
                    strSql += "SELECT id,id+'--'+name AS name FROM cd_goods_class WHERE within_code='" + within_code + "' AND gc_type='2' AND state='0' ORDER BY id";
                    break;
                case "Datum"://材質(產品類型,貨品編碼的第1,2位)
                    strSql += "SELECT id,id+'--'+name AS name FROM cd_datum WHERE within_code='" + within_code + "' AND state='0' ORDER BY id";
                    break;
               
                case "set_state_list"://設定狀態
                    strSql += "SELECT id,id+'--' + name AS name FROM cd_mo_type WHERE within_code='" + within_code + "' AND mo_type='T' ORDER BY id";
                    break;               
                default:
                    strSql += "";
                    break;
            }
           
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            ModelBaseList obj1 = new ModelBaseList();
            obj1.value = "";
            obj1.label = "";
            List<ModelBaseList> lst = new List<ModelBaseList>();
            lst.Add(obj1);
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

        public static List<QtyUnitRate> GetQtyUnitRateList(string unit_code)
        {
            string strSql = "";
            strSql += "Select a.id,a.basic_unit,a.unit_code,a.rate" +
                " From it_coding a " +
                " Where a.within_code='" + within_code + "' And a.id='*' ";
            if (unit_code != "")
                strSql += " And a.unit_code='" + unit_code + "'";
            strSql += " Order By a.id";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            QtyUnitRate obj1 = new QtyUnitRate();
            obj1.value = "";
            obj1.label = "";
            List<QtyUnitRate> lst = new List<QtyUnitRate>();
            lst.Add(obj1);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                QtyUnitRate obj = new QtyUnitRate();
                obj.value = dr["unit_code"].ToString();
                obj.label = dr["unit_code"].ToString().Trim();
                obj.rate = dr["rate"].ToString() == "" ? 0 : Convert.ToDecimal(dr["rate"].ToString());
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
