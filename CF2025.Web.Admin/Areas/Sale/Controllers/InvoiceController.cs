using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;
using CF.Sale.DAL;

namespace CF2025.Web.Admin.Areas.Sale.Controllers
{
    public class InvoiceController : AdminControllerBase//: Controller//: Controller
    {
        // GET: Sale/Invoice
        public ActionResult Index()
        {
            ViewBag.Title = "銷售發票";
            return View();
        }
        public JsonResult GetComboxList(string SourceType)
        {
            var result = Invoice.GetComboxList(SourceType);
            //var result = new { rows = list };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}