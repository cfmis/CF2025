using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;
using CF2025.Sale.Contract;
using CF2025.Sale.DAL;

namespace CF2025.Web.Admin.Areas.Sale.Controllers
{
    public class PackingListController : AdminControllerBase//: Controller//: Controller
    {
        // GET: Sale/PackingList
        public ActionResult Index(string packing_type)
        {
            ViewBag.packing_type = packing_type == null ? "0" : packing_type;
            return View();
        }
        [HttpPost]
        public JsonResult GetDataFromOc(string set_flag,string mo_id)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();SaveInvoice
            var result = PackingListDAL.GetDataFromOc(set_flag,mo_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetDataFromInvoice(string ID, string mo_id)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();SaveInvoice
            var result = PackingListDAL.GetDataFromInvoice(ID,mo_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetDataFromPackingList(string packing_type, string mo_id)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();SaveInvoice
            var result = PackingListDAL.GetDataFromPackingList(packing_type,"", mo_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPackingListByID(string packing_type, string ID)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();SaveInvoice
            var result = PackingListDAL.GetDataFromPackingList(packing_type, ID, "");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Save(xl_packing_list_model PkMostly, List<xl_packing_list_details_model> PkDetails)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = PackingListDAL.Save(PkMostly, PkDetails);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Delete(string ID,string sequence_id)
        {
            var result = PackingListDAL.Delete(ID, sequence_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}