using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;
using CF2025.Store.DAL;
using CF2025.Base.DAL;
using CF2025.Store.Contract;

namespace CF2025.Web.Admin.Areas.Store.Controllers
{
    public class TransferOutUnconfirmController : AdminControllerBase // Controller
    {
        // GET: Store/TransferOutUnconfirm
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetComboxList(string SourceType)
        {
            var result = DataComboxList.GetComboxList(SourceType);            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCartonCodeList(string LocationId)
        {
            var result = DataComboxList.GetCartonCodeList(LocationId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataList(TransferOutFind model)        
        {
            var list = TransferOutUnconfirmDAL.GetSearchDataList(model);
            return Json(list, JsonRequestBehavior.AllowGet);            
        }


    }
}