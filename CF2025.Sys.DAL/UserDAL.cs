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
using CF2025.Sys.Contract;


namespace CF2025.Sys.DAL
{
    public static class UserDAL
    {
        private static SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.Crm);
        private static string within_code = "0000";
        private static string language_id = "1";
        public static UpdateStatusModel ModifyPwd(string UserName, string Pwd)
        {
            UpdateStatusModel mdj = new UpdateStatusModel();
            string strSql = "";
            string ePwd = "";
            ePwd = Encrypt.MD5(Pwd);
            strSql = " Update t_User Set Password='" + ePwd + "'" +
                " Where LoginName='" + UserName + "'";
            var Result = sh.ExecuteSqlUpdate(strSql);
            if (Result == "")
            {
                mdj.Status = "0";
                mdj.Msg = "密碼修改成功!";
            }
            else
            {
                mdj.Status = "1";
                mdj.Msg = "密碼修改失敗!";
            }
            return mdj;
        }
    }
}
