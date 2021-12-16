using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;
using CF2025.Base.DAL;

namespace CF2025.Web.Admin.Areas.Base.Controllers
{
    public class DataComboxListController : AdminControllerBase//: Controller//: Controller
    {
        // GET: Base/DataComboxList
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetComboxList(string SourceType)
        {
            var result = DataComboxList.GetComboxList(SourceType);
            //var result = new { rows = list };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}