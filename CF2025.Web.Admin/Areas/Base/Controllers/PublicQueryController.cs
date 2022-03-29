using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;
using CF2025.Base.Contract.Model;
using CF2025.Base.DAL;

namespace CF2025.Web.Admin.Areas.Base.Controllers
{
    public class PublicQueryController : Controller
    {
        string language_id = AdminUserContext.Current.LoginInfo.LanguageID;
        // GET: Base/PublicQuery
        public ActionResult Index(string window_id)
        {
            string user_id = "";
            try
            {
                user_id = AdminUserContext.Current.LoginInfo.LoginName;
            }
            catch
            {
                user_id = "";
            }
            ViewData["user_id"] = string.IsNullOrEmpty(user_id) ? "" : user_id;
            ViewData["window_id"] = window_id;
            return View();
        }

        //初始化欄位名稱下拉列表
        public JsonResult GetFieldName(string window_id)
        {
            var result = CommonDAL.GetFieldNameList(window_id, language_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //提取對應用戶已保存的查詢條件
        public JsonResult GetSaved(string user_id, string window_id)
        {
            var result = CommonDAL.GetSavedList(user_id, window_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //保存用戶新提交的查詢條件
        [HttpPost]
        public ActionResult SaveQuery(string user_id, List<ModelQuerySavedList> detailData)
        {
            string result = CommonDAL.SaveQueryList(user_id, detailData);
            if (result == "")
                return Json("OK");
            else
                return Json("Error");
        }

        //返回查詢數據
        public JsonResult Query(string sqlText)
        {
            var result = CommonDAL.QueryList(sqlText);           
            return Json(result,JsonRequestBehavior.AllowGet);
        }
    }


}