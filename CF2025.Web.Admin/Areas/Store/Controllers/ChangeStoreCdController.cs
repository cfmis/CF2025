using CF2025.Store.DAL;
using CF2025.Web.Admin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CF2025.Web.Admin.Areas.Store.Controllers
{
    public class ChangeStoreCdController : Controller
    {
        // GET: Store/ChangeStoreCd 轉貨品編號(轉批號)
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

        public JsonResult GetPlanItemList(string mo_id, string ii_location)
        {
            var DataDetail = ChangeStoreDAL.GetPlanItemListByMo(mo_id, ii_location);
            return Json(DataDetail, JsonRequestBehavior.AllowGet);
        }
    }
}