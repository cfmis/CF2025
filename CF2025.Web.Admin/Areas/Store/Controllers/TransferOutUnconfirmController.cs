using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CF2025.Web.Admin.Areas.Store.Controllers
{
    public class TransferOutUnconfirmController : Controller
    {
        // GET: Store/TransferOutUnconfirm
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult GetDataList(Order_Head model)
        //{
        //    var list = SalesOrderDAL.GetOcHeadReturnList(model);           
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}
    }
}