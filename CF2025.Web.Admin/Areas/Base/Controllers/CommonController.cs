﻿using CF2025.Base.DAL;
using CF2025.Web.Admin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CF2025.Web.Admin.Areas.Base.Controllers
{
    public class CommonController : AdminControllerBase
    {       
        //密碼確認
        public ActionResult PasswordConfirm(string user_id)
        {
            ViewData["user_id"] = user_id;
            return View();
        }

        //取最大單號
        public ActionResult GetMaxID(string bill_id, string dept_id, int serial_len)
        {
            var result = CommonDAL.GetMaxID(bill_id, dept_id, serial_len);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //檢查批準狀態       
        public JsonResult CheckApproveStatus(string table_name, string id)
        {
            var result = CommonDAL.CheckApproveStatus(table_name, id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserName(string user_id)
        {
            var result = CommonDAL.GetUserName(user_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserInfo(string user_id, string password)
        {
            var result = CommonDAL.GetUserInfo(user_id, password);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}