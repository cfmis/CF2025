using CF2025.Store.Contract;
using CF2025.Store.DAL;
using CF2025.Web.Admin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CF2025.Web.Admin.Areas.Store.Controllers
{
    public class TransferInController : Controller
    {
        // GET: Store/TransferIn
        public ActionResult Index(string flagChild)
        {
            string user_id = "";
            try
            {
                user_id = AdminUserContext.Current.LoginInfo.LoginName;
            }
            catch
            {
                user_id = "";
            }            
            ViewData["user_id"] = string.IsNullOrEmpty(user_id)?"": user_id;
            ViewData["flagChild"] = flagChild;
            //ViewData["server_date"] = DateTime.Now.Date.ToString("yyyy-MM-dd");
            return View();
        }

        //取最大單號
        public ActionResult GetMaxID(string bill_id,string dept_id)
        {
            var result = TransferOutUnconfirmDAL.GetMaxID(bill_id,dept_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
      
        //主檔,明細一起更新
        [HttpPost]
        public ActionResult Save(TransferInHead head, List<TransferInDetails> detailData)
        {
            string result = TransferOutUnconfirmDAL.SaveAll(head, detailData);
            if (result == "")
                return Json("OK");
            else
                return Json("Error");
        }
    
        public JsonResult GetHeadByID(string id)
        {           
            var DataHead = TransferOutUnconfirmDAL.GetHeadByID(id);
            return Json(DataHead, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailsByID(string id)
        {
            var DataDetail = TransferOutUnconfirmDAL.GetDetailsByID(id);
            return Json(DataDetail, JsonRequestBehavior.AllowGet);
        }

        //批準,反準
        [HttpPost]
        public JsonResult Approve(TransferInHead head,string approve_type)
        {
            var result = TransferOutUnconfirmDAL.Approve(head, approve_type);
            return Json(result, JsonRequestBehavior.AllowGet);
            //if (result == "")
            //    return Json("OK");
            //else
            //    return Json("Error");
        }
        
        //注銷
        [HttpPost]
        public ActionResult DeleteHead(TransferInHead head)
        {
            string result = TransferOutUnconfirmDAL.DeleteHead(head);
            if (result == "")
                return Json("OK");
            else
                return Json("Error");
        }

        public ActionResult QueryPartialView()
        {
            //TableAEntity tae = new TableAEntity();
            //return PartialView(tae.GetTableAs());
            return PartialView();
        }
    }
}
