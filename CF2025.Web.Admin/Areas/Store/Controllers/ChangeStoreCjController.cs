using CF2025.Prod.Contract.Model;
using CF2025.Store.DAL;
using CF2025.Web.Admin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CF2025.Web.Admin.Areas.Store.Controllers
{
    public class ChangeStoreCjController : Controller
    {
        //R單轉正單
        public ActionResult Index()
        {
            string user_id = string.Empty;
            try
            {
                user_id = AdminUserContext.Current.LoginInfo.LoginName;
            }
            catch
            {
                user_id = "";
            }
            ViewData["user_id"] = string.IsNullOrEmpty(user_id) ? "" : user_id;
            return View();
        }
       
    }
}