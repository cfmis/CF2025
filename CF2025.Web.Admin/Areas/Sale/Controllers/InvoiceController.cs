using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;
using CF2025.Sale.DAL;
using CF2025.Sale.Contract;

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
            var result = InvoiceDAL.GetComboxList(SourceType);
            //var result = new { rows = list };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMostlyFromOc(string mo_id)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();GetDataDetails
            var result = InvoiceDAL.GetMostlyFromOc(mo_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDetailsFromOc(string mo_id)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();SaveInvoice
            var result = InvoiceDAL.GetDetailsFromOc(mo_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveInvoice(so_invoice_mostly InvMostly, List<so_invoice_details> InvDetails,List<so_other_fare> InvOtherFare)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.SaveInvoice(InvMostly, InvDetails, InvOtherFare);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetDataByID(string ID)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.GetDataByID(ID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetFareDataByID(string ID)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.GetFareDataByID(ID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}