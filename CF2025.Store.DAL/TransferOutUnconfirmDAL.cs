using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CF.Framework.Contract;
using CF.SQLServer.DAL;
using CF.Core.Config;
using CF2025.Store.Contract;
using CF2025.Base.DAL;

namespace CF2025.Store.DAL
{
    public static class TransferOutUnconfirmDAL
    {
        private static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);
        //private static string strRemoteDB = "DGERP2.cferp.dbo.";//SQLHelper.RemoteDB;
        public static List<TransferOutDetails> GetSearchDataList(TransferOutFind model)
        {           
            string strSql = 
             @"SELECT id,Convert(varchar(10),transfer_date,120) AS transfer_date,mo_id,goods_id,goods_name,color,unit,transfer_amount,
             al_transfer_amount,sec_unit,sec_qty,package_num,nwt,gross_wt,location_id,move_location_id,inventory_qty,lot_no,remark
             FROM v_rpt_transfer_out_list
             WHERE within_code = '0000'";
            if (!string.IsNullOrEmpty(model.transfer_date))
            {
                strSql += string.Format(" AND transfer_date >= '{0}'", model.transfer_date);
            }
            if (!string.IsNullOrEmpty(model.transfer_date_end))
            {
                strSql += string.Format(" AND transfer_date <= '{0}'", model.transfer_date_end);
            }
            if (!string.IsNullOrEmpty(model.id))
            {
                strSql += string.Format(" AND id >= '{0}'", model.id);
            }
            if (!string.IsNullOrEmpty(model.id_end))
            {
                strSql += string.Format(" AND id <= '{0}'", model.id_end);
            }
            if (!string.IsNullOrEmpty(model.mo_id))
            {
                strSql += string.Format(" AND mo_id >= '{0}'", model.mo_id);
            }
            if (!string.IsNullOrEmpty(model.mo_id_end))
            {
                strSql += string.Format(" AND mo_id <= '{0}'", model.mo_id_end);
            }
            if (!string.IsNullOrEmpty(model.goods_id))
            {
                strSql += string.Format(" AND goods_id >= '{0}'", model.goods_id);
            }
            if (!string.IsNullOrEmpty(model.goods_id_end))
            {
                strSql += string.Format(" AND goods_id <= '{0}'", model.goods_id_end);
            }
            strSql += " AND type = '0' AND state = '1'";
            strSql += " AND (transfer_amount - al_transfer_amount) > 0";
            DataTable dtOut = sh.ExecuteSqlReturnDataTable(strSql);
            List<TransferOutDetails> lstModel = CommonUtils.DataTableToList<TransferOutDetails>(dtOut);
            return lstModel;
        }

       
    }
}
