using CF2025.Base.DAL;
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
    public class TransferOutController : Controller
    {
        // GET: Store/TransferOut
        public ActionResult Index()
        {
            var loinfinfo = AdminUserContext.Current.LoginInfo;
            string user_id = (loinfinfo != null) ? loinfinfo.LoginName : "";
            ViewData["user_id"] = string.IsNullOrEmpty(user_id) ? "" : user_id;
            return View();
        }

        // GET: Store/TransferOut/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
       
        public ActionResult GetMaxID(string billId, string billType, string groupNo, int serialLen)
        {
            var result = CommonDAL.GetMaxID(billId, billType, groupNo, serialLen);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult GetStDetailsLotNo(string moId,string locationId )
        {
            var result = TransferOutUnconfirmDAL.GetItemLotNo(moId, locationId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductIdByMo(string moId,bool suit)
        {
            var result = TransferOutUnconfirmDAL.GetProductIdByMo(moId, suit);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPackingInfoByMo(string moId)
        {
            var result = TransferOutUnconfirmDAL.GetPackingInfoByMo(moId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //保存
        [HttpPost]
        public JsonResult Save(TransferInHead headData, List<TransferInDetails> lstDetailData1, List<TransferDetailPart> lstDetailData2,
            List<TransferInDetails> lstDelData1, List<TransferDetailPart> lstDelData2, string user_id)
        {
            //inventory_qty,inventory_sec_qty
            var result = TransferOutUnconfirmDAL.Save(headData, lstDetailData1, lstDetailData2, lstDelData1, lstDelData2, user_id);
            if (result == "")
                result = "OK";
            else
                result = "Error";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHeadByID(string id)
        {
            var DataHead = TransferOutUnconfirmDAL.GetHeadOut(id);
            return Json(DataHead, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailsByID(string id)
        {
            var DataDetail = TransferOutUnconfirmDAL.GetDetailsOutByID(id);
            return Json(DataDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailsPartByID(string id)
        {
            var DataDetail = TransferOutUnconfirmDAL.GetDetailsPartByID(id);
            return Json(DataDetail, JsonRequestBehavior.AllowGet);
        }

        //批準,反準
        [HttpPost]
        public JsonResult Approve(TransferInHead head, string user_id, string approve_type)
        {
            var result = "";
            //1--批準;0--反批準            
            result = TransferOutUnconfirmDAL.Approve(head, user_id, approve_type);
            if (result.Substring(0, 2) == "00")
            {
                result = "OK";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //// GET: Store/TransferOut/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Store/TransferOut/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Store/TransferOut/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Store/TransferOut/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
