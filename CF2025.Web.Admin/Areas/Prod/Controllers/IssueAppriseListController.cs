using CF2025.Prod.Contract.Model;
using CF2025.Prod.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CF2025.Web.Admin.Areas.Prod.Controllers
{
    public class IssueAppriseListController : Controller
    {
        // GET: Prod/IssueAppriseList
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult QueryData(query_condition query)
        {
            var data = IssueAppriseListDAL.QueryData(query);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult QueryDataSub(string location_id, string carton_code, string mo_id, string materiel_id, string upper_sequence,string key_id)
        {
            var data = IssueAppriseListDAL.QueryDataSub(location_id, carton_code, mo_id, materiel_id, upper_sequence, key_id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}