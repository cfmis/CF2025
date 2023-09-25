using CF2025.Store.Contract.Model;
using CF2025.Store.DAL;
using CF2025.Web.Admin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CF2025.Web.Admin.Areas.Store.Controllers
{
    public class AdjustmentController : AdminControllerBase //Controller
    {
        //倉庫調整
        // GET: Store/Adjustment  
        public ActionResult Index()
        {
            string user_id = this.LoginInfo.LoginName.ToString();            
            ViewData["user_id"] = string.IsNullOrEmpty(user_id) ? "" : user_id;
            return View();
        }
       
        public JsonResult GetHeadByID(string id)
        {
            var DataHead = AdjustmentDAL.GetHeadByID(id);
            return Json(DataHead, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailsByID(string id)
        {
            var DataDetail = AdjustmentDAL.GetDetailsByID(id);
            return Json(DataDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStGoodsLotNo(string within_code, string location_id, string mo_id)
        {
            var goods_lotno = AdjustmentDAL.GetStDetailsGoods(within_code, location_id,mo_id);
            return Json(goods_lotno, JsonRequestBehavior.AllowGet);
        }

        //保存
        [HttpPost]
        public JsonResult Save(st_adjustment_mostly headData, List<st_a_subordination> lstDetailData1, List<st_a_subordination> lstDelData1, string user_id)
        {
            var result = AdjustmentDAL.Save(headData, lstDetailData1, lstDelData1, user_id);
            if (result == "")
                result = "OK";
            else
                result = "Error";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //批準,反準
        [HttpPost]
        public JsonResult Approve(st_adjustment_mostly head, string user_id, string approve_type)
        {
            var result = "";
            //批準/反批準  
            result = AdjustmentDAL.Approve(head, user_id, approve_type);
            if (result.Substring(0, 2) == "00")
            {
                result = "OK";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}