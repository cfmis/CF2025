using CF2025.Base.DAL;
using CF2025.Web.Admin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CF2025.Web.Admin.Areas.Base.Controllers
{
    public class CommonController : AdminControllerBase
    {       
        //密碼確認
        public ActionResult PasswordConfirm(string user_id)
        {
            ViewData["user_id"] = user_id;
            return View();
        }
        
        //取倉庫調整最大單號
        public ActionResult GetMaxIDStock(string bill_id, int serial_len)
        {           
            var result = CommonDAL.GetMaxID(bill_id, serial_len);
            return Json(result, JsonRequestBehavior.AllowGet);            
        }
        //取最大單號
        public ActionResult GetMaxID(string bill_id, string dept_id, int serial_len)
        {
            var result = CommonDAL.GetMaxID(bill_id, dept_id, serial_len);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //取移交單最大單號
        public ActionResult GetMaxIDJo07(string out_dept, string in_dept, string doc_type)
        {
            var result = CommonDAL.GetMaxIDJo07(out_dept, in_dept, doc_type);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //檢查批準狀態       
        public JsonResult CheckApproveState(string table_name, string id)
        {
            var result = CommonDAL.CheckApproveState(table_name, id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserName(string user_id)
        {
            var result = CommonDAL.GetUserName(user_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserInfo(string user_id, string password)
        {
            var result = CommonDAL.GetUserInfo(user_id, password);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //檢查是否可以反批準       
        public JsonResult CheckCanApprove(string id,string window_id)
        {
            var result = CommonDAL.CheckCanApprove(id,window_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult QtyToSecQty(string within_code,string location_id,string goods_id,decimal qty)
        {
            var result = CommonDAL.QtyToSecQty(within_code, location_id, goods_id, qty);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //檢查當前用戶部門的操作權限:0無權限,1有權限
        public JsonResult CheckUserDeptRights(string user_id, string dept_id)
        {           
            var result = CommonDAL.CheckUserDeptRights(user_id, dept_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //檢查移交單是否已簽收
        public JsonResult CheckSignfor(string id)
        {
            var result = CommonDAL.CheckSignfor(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //計劃是否已批準
        public JsonResult GetPlanApproveState(string mo_id)
        {
            var result = 0;
            result = CommonDAL.GetPlanApproveState(mo_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //計劃是否已批準
        public JsonResult GetPlanHoldState(string mo_id,string goods_id,string out_dept,string in_dept)
        {
            var result = 0;
            result = CommonDAL.GetPlanHoldState(mo_id,goods_id,out_dept,in_dept);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //判断OC是否被hold
        public JsonResult GetOcHoldState(string mo_id)
        {
            var result = 0;
            result = CommonDAL.GetOcHoldState(mo_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStDetailsLotNo(string location_id,string goods_id,string mo_id)
        {
            var result = CommonDAL.GetStDetailsLotNo(location_id,goods_id,mo_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}