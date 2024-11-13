using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Base.DAL;
using CF2025.Base.Contract;
using CF2025.Web.Admin.Common;


namespace CF2025.Web.Admin.Areas.Base.Controllers
{
    public class RoleAuthorityPowersController : AdminControllerBase
    {         
        // GET: Base/RoleAuthorityPowers
        public ActionResult Index2()
        {
            return View();
        }

        /// <summary>
        /// 檢查當前用戶模塊某按鈕的權限 
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="menu_id">菜單功能模塊ID</param>
        /// <param name="func_name">按鈕功能等</param>
        /// <returns></returns>
        public JsonResult CheckAuthority(string user_id, string menu_id, string func_name)
        {
            var result = CommonDAL.CheckAuthority(user_id, menu_id, func_name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchRoleAuthorityPowers(RoleAuthorityPowersModels searchParams)
        {
            var powerList = CommonDAL.GetRoleAuthorityPowers(searchParams);
            return Json(powerList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddRoleAuthorityPowers(RoleAuthorityPowersModels updateParams)
        {
            var loinfinfo = AdminUserContext.Current.LoginInfo;
            var user_id = (loinfinfo != null) ? loinfinfo.LoginName : "";           
            var lst = CommonDAL.UpdateRoleAuthorityPowers(user_id,updateParams);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DelRoleAuthorityPowers(int ID)
        {            
            var lst = CommonDAL.DelRoleAuthorityPowersByID(ID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPermissions(string AuthorityID)
        {
            var loinfinfo = AdminUserContext.Current.LoginInfo;
            var user_id = (loinfinfo != null) ? loinfinfo.LoginName : "";
            var PermissionList = CommonDAL.GetPermission(user_id, AuthorityID);
            return Json(PermissionList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBasePermission(string TableName)
        {
            var lst = CommonDAL.GetBasePermissionList(TableName);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
    }
}