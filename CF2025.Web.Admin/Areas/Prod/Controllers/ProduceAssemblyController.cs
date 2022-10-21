using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;
using CF2025.Prod.DAL;
using CF2025.Prod.Contract;

namespace CF2025.Web.Admin.Areas.Produce.Controllers
{
    public class ProduceAssemblyController : Controller
    {
        // GET: Produce/ProduceAssembly
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

        public JsonResult GetHeadByID(string id)
        {
            var DataHead = ProduceAssemblyDAL.GetHeadByID(id);
            return Json(DataHead, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailsByID(string id)
        {
            var DataDetail = ProduceAssemblyDAL.GetDetailsByID(id);
            return Json(DataDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailsPartByID(string id)
        {
            var DataDetail = ProduceAssemblyDAL.GetDetailsPartByID(id);
            return Json(DataDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGoodsId(string mo_id,string out_dept,string in_dept)
        {
            var goods= ProduceAssemblyDAL.GetGoodsId(mo_id,out_dept, in_dept);
            return Json(goods, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult CheckMo(string mo_id)
        //{
        //    var mo = ProduceAssemblyDAL.CheckMo(mo_id);
        //    return Json(mo, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult CheckItem(string goods_id)
        //{
        //    var item = ProduceAssemblyDAL.CheckItem(goods_id);
        //    return Json(item, JsonRequestBehavior.AllowGet);
        //}

        //GetAssembly
        public JsonResult GetAssembly(string mo_id, string goods_id, string out_dept, string in_dept)
        {
            var goods = ProduceAssemblyDAL.GetAssembly(mo_id, goods_id, out_dept, in_dept);
            return Json(goods, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLotNoList(string goods_id, string location_id, string mo_id)
        {
            var goods = ProduceAssemblyDAL.GetLotNoList(goods_id, location_id, mo_id);
            return Json(goods, JsonRequestBehavior.AllowGet);
        }

        ////取最大單號
        //public ActionResult GetMaxID(string bill_id,string dept_id,int serial_len)
        //{
        //    var result = ProduceAssemblyDAL.GetMaxID(bill_id,dept_id, serial_len);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        ////檢查批準狀態
        ////[HttpPost]
        //public JsonResult CheckApproveStatus(string table_name,string id)
        //{
        //    var result = ProduceAssemblyDAL.CheckApproveStatus(table_name,id);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //批準,反準
        [HttpPost]
        public JsonResult Approve(jo_assembly_mostly head,string user_id, string approve_type)
        {
            var result = ProduceAssemblyDAL.Approve(head.id,head.handover_id,user_id, approve_type);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //保存
        [HttpPost]
        public JsonResult Save(jo_assembly_mostly headData, List<jo_assembly_details> lstDetailData1, List<jo_assembly_details_part> lstDetailData2, 
            List<jo_assembly_details> lstDelData1, List<jo_assembly_details_part> lstDelData2, List<jo_assembly_details> lstTurnOver, List<jo_assembly_details> lstTurnOverQc)
        {
            var result = ProduceAssemblyDAL.Save(headData, lstDetailData1, lstDetailData2, lstDelData1, lstDelData2, lstTurnOver, lstTurnOverQc);
            if (result == "")
                result = "OK";
            else
                result = "Error";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //保存前檢查成份的庫存
        [HttpPost]
        public JsonResult CheckStock(List<check_part_stock> partData )
        {
            var result = ProduceAssemblyDAL.CheckStorage(partData);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //移交数量是否允许超过生产数量;1为允许;0为不允许
        [HttpPost]
        public JsonResult CheckParameters(string id)
        {
            var result = ProduceAssemblyDAL.CheckParameters(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //是否電鍍部門;1为電鍍;0为不是
        [HttpPost]
        public JsonResult CheckPlateDept(string id)
        {
            var result = ProduceAssemblyDAL.CheckPlateDept(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //批量生產單號是否存在
        [HttpPost]
        public JsonResult CheckIsExistsPrdId(int prd_id, string id)
        {
            var result = ProduceAssemblyDAL.CheckIsExistsPrdId(prd_id,id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //計劃流程是否存在
        [HttpPost]
        public JsonResult CheckIsExistsPlan(string mo_id, string goods_id, string wp_id, string next_wp_id)
        {
            var result = ProduceAssemblyDAL.CheckIsExistsPlan(mo_id, goods_id, wp_id, next_wp_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }       
        //是否有QC流程或者已完成
        [HttpPost]
        public JsonResult GetQcQty(string mo_id, string goods_id, string wp_id)
        {
            var result = ProduceAssemblyDAL.GetQcQty(mo_id, goods_id, wp_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //已组装的数量
        [HttpPost]
        public JsonResult GetAssemblyQty(string id, string mo_id, string goods_id, string out_dept)
        {
            var result = ProduceAssemblyDAL.GetAssemblyQty(id,mo_id, goods_id, out_dept);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //取流程生产数量string out_dept,string mo_id, string goods_id
        [HttpPost]
        public JsonResult GetPlanProdQty(string out_dept,string mo_id, string goods_id )
        {
            var result = ProduceAssemblyDAL.GetPlanProdQty(out_dept, mo_id, goods_id );
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //是否有移交特批权限
        public JsonResult GetUserPope(string user_id)
        {
            var result = ProduceAssemblyDAL.GetUserPope(user_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //// GET: Produce/ProduceAssembly/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Produce/ProduceAssembly/Delete/5
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
