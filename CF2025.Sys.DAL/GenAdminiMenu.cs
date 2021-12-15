using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CF.SQLServer.DAL;
using CF2025.Sys.Contract;
using CF.Core.Config;

namespace CF2025.Sys.DAL
{
    public class GenAdminiMenu
    {
        SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.Crm);
        public List<AdminMenuGroup> getAdminMenuList(string LoginName, string LanguageID)
        {
            List<AdminMenuGroup> lsAdminMenuGroup = new List<AdminMenuGroup>();
            //LanguageID = "0";
            string strSql = "Select DISTINCT a.AuthorityID,b.AuthorityName,a.ModuleUrl,a.WebUrl,b.Info,a.DivID,a.UlID,a.Icon,a.Class" +
                " From Authority a" +
                " Inner Join AuthorityLang b ON a.AuthorityID = b.AuthorityID" +
                " Inner Join RoleAuthority c ON a.AuthorityID = c.AuthorityID" +
                " Inner Join UserRole d ON c.RoleID = d.RoleID" +
                " Inner Join [User] e ON d.UserID = e.ID" +
                " Where a.TypeID = '20000' And b.LangID = '" + LanguageID + "' And e.LoginName = '" + LoginName + "'";
            DataTable dtMenu = sh.ExecuteSqlReturnDataTable(strSql);
            for (int i = 0; i < dtMenu.Rows.Count; i++)
            {
                DataRow dr = dtMenu.Rows[i];
                AdminMenuGroup item = new AdminMenuGroup();
                item.Id = Convert.ToInt32(dr["AuthorityID"]);
                item.Name = dr["AuthorityName"].ToString();
                item.Url = dr["WebUrl"].ToString();
                item.Info = dr["Info"].ToString();
                item.DivId = dr["DivID"].ToString();
                item.UlId = dr["UlID"].ToString();
                item.Icon = dr["Icon"].ToString();
                item.Class = dr["Class"].ToString();
                lsAdminMenuGroup.Add(item);
            }
            return lsAdminMenuGroup;
        }
        public List<AdminMenuGroupAll> getAdminMenuDetailsList(string LoginName, string LanguageID)
        {
            List<AdminMenuGroupAll> lsAdminMenuGroupAll = new List<AdminMenuGroupAll>();
            //LanguageID = "0";
            string strSql1 = "Select DISTINCT a.AuthorityID,a.TypeID,b.AuthorityName,a.ModuleUrl,a.WebUrl,b.Info,a.DivID,a.UlID,a.Icon,a.Class,a.MenuLayer,a.WebID" +
                " From Authority a" +
                " Inner Join AuthorityLang b ON a.AuthorityID = b.AuthorityID" +
                " Inner Join RoleAuthority c ON a.AuthorityID = c.AuthorityID" +
                " Inner Join UserRole d ON c.RoleID = d.RoleID" +
                " Inner Join [User] e ON d.UserID = e.ID";
            string strSql = strSql1 + " Where a.TypeID='20000' And b.LangID='" + LanguageID + "' And e.LoginName='" + LoginName + "'";
            DataTable dtMenu = sh.ExecuteSqlReturnDataTable(strSql);
            for (int i = 0; i < dtMenu.Rows.Count; i++)
            {
                AdminMenuGroupAll itemAll = new AdminMenuGroupAll();
                List<AdminMenuGroup> lsAdminMenuGroup = new List<AdminMenuGroup>();
                itemAll.DivId = dtMenu.Rows[i]["DivID"].ToString();
                var TypeID = Convert.ToInt32(dtMenu.Rows[i]["AuthorityID"]);
                DataTable dt = getSubAdminMenuDetails(strSql1, TypeID, LoginName, LanguageID);
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    AdminMenuGroup item = new AdminMenuGroup();
                    DataRow dr = dt.Rows[j];
                    item.Id = Convert.ToInt32(dr["AuthorityID"]);
                    item.Name = dr["AuthorityName"].ToString();
                    item.Url = dr["WebUrl"].ToString();
                    item.Info = dr["Info"].ToString();
                    item.DivId = dr["DivID"].ToString();
                    item.UlId = dr["UlID"].ToString();
                    item.Icon = dr["Icon"].ToString();
                    item.Class = dr["Class"].ToString();
                    item.MenuLayer = dr["MenuLayer"].ToString();
                    item.WebId = dr["WebID"].ToString();
                    var TypeID1 = Convert.ToInt32(dr["AuthorityID"]);
                    DataTable dt1 = getSubAdminMenuDetails(strSql1, TypeID1, LoginName, LanguageID);
                    List<AdminMenu> lsAdminMenu = new List<AdminMenu>();
                    for (int k = 0; k < dt1.Rows.Count; k++)
                    {
                        AdminMenu item1 = new AdminMenu();
                        DataRow dr1 = dt1.Rows[k];
                        item1.Id = Convert.ToInt32(dr1["AuthorityID"]);
                        item1.Name = dr1["AuthorityName"].ToString();
                        item1.Url = dr1["WebUrl"].ToString();
                        item1.Info = dr1["Info"].ToString();
                        item1.DivId = dr1["DivID"].ToString();
                        item1.UlId = dr1["UlID"].ToString();
                        item1.Icon = dr1["Icon"].ToString();
                        item1.Class = dr1["Class"].ToString();
                        item1.MenuLayer = dr1["MenuLayer"].ToString();
                        item1.WebId = dr1["WebID"].ToString();
                        lsAdminMenu.Add(item1);
                    }
                    item.AdminMenuArray = lsAdminMenu;
                    lsAdminMenuGroup.Add(item);
                }
                itemAll.AdminMenuGroup = lsAdminMenuGroup;
                lsAdminMenuGroupAll.Add(itemAll);
            }
            return lsAdminMenuGroupAll;
        }

        //public List<AdminMenu> getAdminMenuDetailsList()
        //{
        //    List<AdminMenu> lsAdminMenu = new List<AdminMenu>();
        //    var LoginName = AdminUserContext.Current.LoginInfo.LoginName;//.BusinessPermissionList.Select(p => p.ToString());
        //    string strSql1 = "Select a.TypeID,a.AuthorityID,b.AuthorityName,a.ModuleUrl,a.WebUrl,b.Info,a.DivID,a.UlID,a.Icon,a.Class,a.MenuLayer,a.WebID" +
        //        " From t_Authority a" +
        //        " Inner Join t_AuthorityLang b ON a.AuthorityID=b.AuthorityID" +
        //        " Inner Join t_GroupAuthority c ON a.AuthorityID=c.AuthorityID " +
        //        " Inner Join User d ON c.GroupID=d.t_GroupID ";
        //    string strSql=strSql1+" Where a.TypeID='20000' And b.LangID='0' And d.LoginName='" + LoginName + "'";
        //    DataTable dt1 = SQLHelper.ExecuteSqlReturnDataTable(strSql);
        //    for (int i = 0; i < dt1.Rows.Count; i++)
        //    {
        //        lsAdminMenu.Add(changeTableItem(dt1,i));
        //        var TypeID = dt1.Rows[i]["AuthorityID"].ToString();
        //        DataTable dt2 = getSubAdminMenuDetails(strSql1, TypeID);
        //        for (int j = 0; j < dt2.Rows.Count; j++)
        //        {
        //            lsAdminMenu.Add(changeTableItem(dt2, j));
        //            var TypeID1 = dt2.Rows[j]["AuthorityID"].ToString();
        //            DataTable dt3 = getSubAdminMenuDetails(strSql1,TypeID1);
        //            for (int k = 0; k < dt3.Rows.Count; k++)
        //            {
        //                lsAdminMenu.Add(changeTableItem(dt3, k));
        //            }
        //        }
        //    }
        //    return lsAdminMenu;
        //}
        public DataTable getSubAdminMenuDetails(string strSql1, int TypeID, string LoginName, string LanguageID)
        {
            //var LoginName = AdminUserContext.Current.LoginInfo.LoginName;//.BusinessPermissionList.Select(p => p.ToString());
            //string strSql2 = "Select a.TypeID,a.AuthorityID,b.AuthorityName,a.ModuleUrl,a.WebUrl,b.Info,a.DivID,a.UlID,a.Icon,a.Class,a.MenuLayer,a.WebID" +
            //    " From t_Authority a" +
            //    " Inner Join t_AuthorityLang b ON a.AuthorityID=b.AuthorityID" +
            //    " Inner Join t_GroupAuthority c ON a.AuthorityID=c.AuthorityID " +
            //    " Inner Join tb_sy_user d ON c.GroupID=d.t_GroupID ";
            string strSql = strSql1 + " Where b.LangID='" + LanguageID + "' And e.LoginName='" + LoginName + "' And a.TypeID='" + TypeID + "'";
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            return dt;
        }
        public AdminMenu changeTableItem(DataTable dtMenu, int rowNo)
        {
            AdminMenu item = new AdminMenu();
            DataRow dr = dtMenu.Rows[rowNo];
            item.Id = Convert.ToInt32(dr["AuthorityID"]);
            item.Name = dr["AuthorityName"].ToString();
            item.Url = dr["WebUrl"].ToString();
            item.Info = dr["Info"].ToString();
            item.DivId = dr["DivID"].ToString();
            item.UlId = dr["UlID"].ToString();
            item.Icon = dr["Icon"].ToString();
            item.Class = dr["Class"].ToString();
            item.MenuLayer = dr["MenuLayer"].ToString();
            item.WebId = dr["WebID"].ToString();
            return item;
        }
    }
}
