using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;
using CF2025.Prod.DAL;
using CF2025.Prod.Contract;

namespace CF2025.Web.Admin.Areas.Prod.Controllers
{
    public class ReturnRechangeController : AdminControllerBase //Controller
    {
        //移交單退回
        // GET: Prod/ReturnRechange  
        public ActionResult Index()
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
            ViewData["user_id"] = string.IsNullOrEmpty(user_id) ? "" : user_id;
            return View();
        }

        //[HttpPost]
        public JsonResult GetHeadByID(string id)
        {
            var DataHead = ReturnRechangeDAL.GetHeadByID(id);
            return Json(DataHead, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailsByID(string id)
        {
            var DataDetail = ReturnRechangeDAL.GetDetailsByID(id);
            return Json(DataDetail, JsonRequestBehavior.AllowGet);
        }

        //批準前檢查的庫存
        [HttpPost]
        public JsonResult CheckStock(string id)
        {
            var result = ReturnRechangeDAL.CheckStorage(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //批準,反準
        [HttpPost]
        public JsonResult Approve(jo_assembly_mostly head, string user_id, string approve_type)
        {
            var result = "";
            //批準/反批準  
            result = ReturnRechangeDAL.Approve(head, user_id, approve_type);
            if (result.Substring(0, 2) == "00")
            {
                result = "OK";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //移交單總數量
        [HttpPost]
        public JsonResult GetTotalRechange(string id, string mo_id, string goods_id, string out_dept, string in_dept)
        {
            var result = ReturnRechangeDAL.GetTotalRechange(id, mo_id, goods_id, out_dept, in_dept);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //批色相關數據
        [HttpPost]
        public JsonResult GetShadingColorData(string out_dept, string mo_id, string goods_id)
        {
            var result = ReturnRechangeDAL.GetShadingColorData(out_dept, mo_id, goods_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //判断生产计划是否有移交QC的流程
        [HttpPost]
        public JsonResult CheckPlanQcProcess(string mo_id, string goods_id, string out_dept, string in_dept)
        {
            var result = ReturnRechangeDAL.CheckPlanQcProcess(mo_id, goods_id, out_dept, in_dept);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //判断生产计划是否有移交QC的流程
        [HttpPost]
        public JsonResult CheckOcQcProcess(string mo_id, string goods_id, string out_dept, string in_dept)
        {
            var result = ReturnRechangeDAL.CheckOcQcProcess(mo_id, goods_id, out_dept, in_dept);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //判斷對應生產計畫中的QC流程是否已開有移交單
        [HttpPost]
        public JsonResult CheckRechangeQc(string mo_id, string goods_id, string out_dept, string in_dept)
        {
            var result = ReturnRechangeDAL.CheckRechangeQc(mo_id, goods_id, out_dept, in_dept);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //保存
        [HttpPost]
        public JsonResult Save(jo_assembly_mostly headData, List<jo_assembly_details> lstDetailData1, List<jo_assembly_details> lstDelData1, string user_id)
        {
            var result = ReturnRechangeDAL.Save(headData, lstDetailData1, lstDelData1, user_id);
            if (result == "")
                result = "OK";
            else
                result = "Error";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //注銷
        [HttpPost]
        public ActionResult DeleteHead(jo_assembly_mostly head, string user_id)
        {
            string result = ReturnRechangeDAL.DeleteHead(head, user_id);
            if (result == "")
                return Json("OK");
            else
                return Json("Error");
        }

        //檢查計劃中的流程是否存在
        [HttpPost]
        public JsonResult CheckPlanFlow(string mo_id, string goods_id, string out_dept, string in_dept)
        {
            var result = ReturnRechangeDAL.CheckRechangeQc(mo_id, goods_id, out_dept, in_dept);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}