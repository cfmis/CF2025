using CF2025.Base.Contract;
using CF2025.Base.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CF2025.Web.Admin.Areas.Base.Controllers
{
    public class PublicItemQueryController : Controller
    {
        // GET: Base/PublicItemQuery
        public ActionResult Index(string window_id)
        {
            return View();
        }

        //返回貨品編碼基本資料數據
        public JsonResult Query(ModelItemQuery SearchAry )
        {
            var result = CommonDAL.ItemQueryList(SearchAry);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //檢查頁數
        public JsonResult CheckMo(string mo_id)
        {
            var mo = CommonDAL.CheckMo(mo_id);
            return Json(mo, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CheckByMo(string mo_id)
        {
            var mo = CommonDAL.CheckMo(mo_id);
            return Json(mo, JsonRequestBehavior.AllowGet);
        }
        //檢查貨品
        public JsonResult CheckItem(string goods_id)
        {
            var item = CommonDAL.CheckItem(goods_id);
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CheckItemById(string goods_id)
        {
            var item = CommonDAL.CheckItem(goods_id);
            return Json(item, JsonRequestBehavior.AllowGet);
        }

    }
}