using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;
using CF2025.Base.DAL;
using CF2025.Base.Contract.Model;

namespace CF2025.Web.Admin.Areas.Base.Controllers
{
    public class DataComboxListController : AdminControllerBase 
    {
        // GET: Base/DataComboxList
        string language_id = AdminUserContext.Current.LoginInfo.LanguageID;
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetComboxList(string SourceType)
        {           
            var result = DataComboxList.GetComboxList(SourceType,language_id);           
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