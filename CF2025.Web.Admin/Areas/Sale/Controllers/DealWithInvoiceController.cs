using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;
using CF2025.Sale.DAL;
using CF2025.Sale.Contract;
using FastReport.Web;
using System.Web.UI.WebControls;
using CF.Framework.Contract;

namespace CF2025.Web.Admin.Areas.Sale.Controllers
{
    public class DealWithInvoiceController : AdminControllerBase//: Controller//: Controller
    {
        // GET: Sale/DealWithInvoice
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ConfirmSent()
        {
            return View();
        }


        public JsonResult FindInvoice(string search_type,string it_customer_from, string it_customer_to
            , string oi_date_from, string oi_date_to, string sd_date_from, string sd_date_to,string ID, string mo_id,string state)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = DealWithInvoiceDAL.FindInvoice(search_type,it_customer_from, it_customer_to
                , oi_date_from, oi_date_to, sd_date_from, sd_date_to, ID, mo_id, state);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //發貨確認
        [HttpPost]
        public JsonResult ConfirmSent(List<InvoiceModel> InvoiceModel, string issues_state)
        {
            UpdateStatusModel result = new UpdateStatusModel();
            for (int i = 0; i < InvoiceModel.Count; i++)
            {
                var so_invoice_details = DealWithInvoiceDAL.GetInvoiceByID(InvoiceModel[i].ID);
                result = InvoiceDAL.ConfirmSent(so_invoice_details, "0", issues_state);
                if (result.Status == "1")
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //收貨設定
        [HttpPost]
        public JsonResult ConfirmReceipt(List<InvoiceModel> InvoiceModel, string conf_flag, string return_state)
        {
            UpdateStatusModel result = new UpdateStatusModel();
            result = DealWithInvoiceDAL.ConfirmReceipt(InvoiceModel, conf_flag, return_state);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ConfirmReceipt()
        {
            return View();
        }
    }
}