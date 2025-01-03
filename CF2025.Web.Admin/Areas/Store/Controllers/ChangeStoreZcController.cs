﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;
using CF2025.Prod.Contract.Model;
using CF2025.Store.DAL;

namespace CF2025.Web.Admin.Areas.Store.Controllers
{
    public class ChangeStoreZcController : Controller
    {
        //倉庫轉倉
        // GET: Store/ChangeStoreZc
        public ActionResult Index()
        {
            string user_id = string.Empty;
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

        //保存
        [HttpPost]
        public JsonResult Save(st_inventory_mostly headData, List<st_i_subordination> lstDetailData1, List<st_i_subordination> lstDelData1, string user_id,string moduleType)
        {
            var result = ChangeStoreDAL.SaveCeCj(headData, lstDetailData1, lstDelData1, user_id, moduleType);
            if (result.Substring(0, 2) == "00")
                result = "OK";
            else
                result = "Error";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //批準前檢查的庫存
        [HttpPost]
        public JsonResult CheckStock(string id, string moduleType)
        {
            var result = ChangeStoreDAL.CheckStorageCe(id, moduleType);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //批準/反準
        [HttpPost]
        public JsonResult Approve(st_inventory_mostly head, string user_id, string approve_type,string moduleType)
        {
            var result = "";
            result = ChangeStoreDAL.Approve(head, user_id, approve_type, moduleType);//B,C
            if (result.Substring(0, 2) == "00")
            {
                result = "OK";
            }
            return Json(result, JsonRequestBehavior.AllowGet);            
        }

        //注銷
        [HttpPost]
        public ActionResult DeleteHead(st_inventory_mostly head)
        {
            string result = ChangeStoreDAL.DeleteHead(head);
            if (result == "")
                return Json("OK");
            else
                return Json("Error");
        }

        public JsonResult GetHeadByID(string id, string moduleType)
        {
            var DataHead = ChangeStoreDAL.GetHeadByID(id, moduleType); //B,C
            return Json(DataHead, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailsByID(string id)
        {
            var DataDetail = ChangeStoreDAL.GetDetailsByID(id);
            return Json(DataDetail, JsonRequestBehavior.AllowGet);
        }       

        public JsonResult GetPlanItemList(string mo_id)
        {
            var DataDetail = ChangeStoreDAL.GetPlanItemListByMo(mo_id);
            return Json(DataDetail, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetPlanItemList(string mo_id,string ii_location)
        //{
        //    var DataDetail = ChangeStoreDAL.GetPlanItemListByMo(mo_id, ii_location);
        //    return Json(DataDetail, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult CheckMo(string mo_id)
        {
            string result = ChangeStoreDAL.CheckMo(mo_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckGoodsID(string goods_id)
        {
            var result = ChangeStoreDAL.CheckGoodsID(goods_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}