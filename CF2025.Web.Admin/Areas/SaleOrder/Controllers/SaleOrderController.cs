using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;


namespace CF2025.Web.Admin.Areas.SaleOrder.Controllers
{
    public class SaleOrderController :  AdminControllerBase
    {
        // GET: SaleOrder/SaleOrder
        public ActionResult Index()
        {
            ViewBag.Title = "訂單錄入";
            return View();
        }
    }
}