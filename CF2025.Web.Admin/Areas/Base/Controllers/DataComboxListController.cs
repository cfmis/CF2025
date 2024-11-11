using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;
using CF2025.Base.DAL;

namespace CF2025.Web.Admin.Areas.Base.Controllers
{
    public class DataComboxListController : AdminControllerBase 
    {
        // GET: Base/DataComboxList        
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetComboxList(string SourceType)
        {
            var loginInfo = AdminUserContext.Current.LoginInfo;
            string language_id = loginInfo != null ? AdminUserContext.Current.LoginInfo.LanguageID : "0";
            var result = DataComboxList.GetComboxList(SourceType,language_id);           
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetQtyUnitRateList(string SourceType)
        {
            var result = DataComboxList.GetQtyUnitRateList("");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCartonCodeList(string LocationId)
        {            
            var result = DataComboxList.GetCartonCodeList(LocationId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDateServer()
        {
            var result = DateTime.Now.Date.ToString("yyyy-MM-dd"); 
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}