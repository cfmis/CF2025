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

namespace CF2025.Web.Admin.Areas.Sale.Controllers
{
    public class InvoiceController : AdminControllerBase//: Controller//: Controller
    {
        // GET: Sale/Invoice
        public ActionResult Index(string DocType)
        {
            ViewBag.Title = "銷售發票";
            ViewBag.DocType = DocType;
            return View();
        }
        public ActionResult Create()
        {
            ViewBag.Title = "銷售發票";
            return View();
        }
        public JsonResult GetComboxList(string SourceType)
        {
            var result = InvoiceDAL.GetComboxList(SourceType);
            //var result = new { rows = list };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMostlyFromOc(string mo_id)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();GetDataDetails
            var result = InvoiceDAL.GetMostlyFromOc(mo_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDetailsFromOc(string mo_id)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();SaveInvoice
            var result = InvoiceDAL.GetDetailsFromOc(mo_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveInvoice(so_invoice_mostly InvMostly, List<so_invoice_details> InvDetails,List<so_other_fare> InvOtherFare)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.SaveInvoice(InvMostly, InvDetails, InvOtherFare);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DelInvoice(so_invoice_mostly InvMostly, string sequence_id, List<so_other_fare> InvOtherFare)
        {
            ////PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.DelInvoice(InvMostly, sequence_id, InvOtherFare);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetInvoiceByID(string ID,string flag)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.GetInvoiceByID(ID, flag);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public JsonResult GetFareDataByID(string ID)
        //{
        //    //PlanDAL clsPlanDAL = new PlanDAL();
        //    var result = InvoiceDAL.GetFareDataByID(ID);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult ValidGoodsData(string ID,string mo_id,string goods_id,string u_invoice_qty,string goods_unit,string sec_qty,string location_id)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.ValidGoodsData(ID, mo_id, goods_id, u_invoice_qty, goods_unit, sec_qty, location_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckGoodsQty(string ID, string mo_id, string goods_id, string u_invoice_qty, string goods_unit, string sec_qty, string location_id)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.CheckGoodsQty(ID, mo_id, goods_id, u_invoice_qty, goods_unit, sec_qty, location_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGoodsWegFromStore(string mo_id, string goods_id, string location_id, string u_invoice_qty, string goods_unit)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.GetGoodsWegFromStore(mo_id, goods_id, location_id, u_invoice_qty, goods_unit);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ApproveInvoice(so_invoice_mostly InvMostly, List<so_invoice_details> InvDetails, List<so_other_fare> InvOtherFare)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.ApproveInvoice(InvMostly, InvDetails, InvOtherFare);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult NewVersion(so_invoice_mostly InvMostly, List<so_invoice_details> InvDetails, List<so_other_fare> InvOtherFare)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.NewVersion(InvMostly, InvDetails, InvOtherFare);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //發貨確認
        [HttpPost]
        public JsonResult ConfirmSent(List<so_invoice_details> InvDetails,string flag,string issues_state)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.ConfirmSent(InvDetails, flag, issues_state);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //取消發貨
        [HttpPost]
        public JsonResult CancelSent(List<so_invoice_details> InvDetails,string flag)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.CancelSent(InvDetails, flag);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //注銷
        [HttpPost]
        public JsonResult CancelDoc(string ID)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.CancelDoc(ID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //打印
        WebReport rpt = new WebReport();
        public ActionResult Print(string ID,string Ver,string report_id)
        {
            report_id = report_id.Replace(".rpt", ".frx");
            if (report_id == "so_invoice_remark_commercial.frx")
            {
                //打印條款(出口發票), 出口與本地條款一致
                report_id = "so_invoice_remark.frx";
            }            
            string report_path = $"{Request.MapPath(Request.ApplicationPath)}Reports\\" + report_id;           
            switch (report_id)
            {
                case "so_invoice.frx":
                    //本地戶口發票
                    var list1 = InvoiceDAL.GetReportDataByID(ID, Ver,"");
                    rpt.Report.RegisterData(list1, "InvData");//注冊數據
                    break;
                case "so_invoice_cash.frx":
                    //現沽發票
                    var list2 = InvoiceDAL.GetReportDataByID(ID, Ver,"DC");
                    rpt.Report.RegisterData(list2, "InvData");
                    break;
                case "so_invoice_ship_remark.frx":
                    //打印裝運嘜頭
                    var list3 = InvoiceDAL.GetShipRemark(ID, Ver);
                    rpt.Report.RegisterData(list3, "InvData");
                    break;
                case "so_invoice_remark.frx":             //打印條款(本地及現沽發票)
                //case "so_invoice_remark_commercial.frx":  //打印條款(出口發票)                    
                    rpt.Report.RegisterData("", "InvData");
                    break;
                case "so_invoice_commercial.frx":
                    //出口發票格式1
                    var list4 = InvoiceDAL.GetReportCommercial(ID, Ver);
                    rpt.Report.RegisterData(list4, "InvData");
                    break;
                case "so_invoice_commercial_2.frx":
                    //出口發票格式2
                    var list5 = InvoiceDAL.GetReportCommercial(ID, Ver);
                    rpt.Report.RegisterData(list5, "InvData");
                    break;                
                case "so_invoice_remark2.frx":
                    //打印備註                     
                    rpt.Report.RegisterData("", "InvData");
                    break;
            }          
            rpt.Report.Load(report_path);//調用報表模板
            rpt.Width = 1024;// Unit.Percentage(100);
            rpt.Height = 800;// Unit.Percentage(100);
            rpt.ToolbarStyle = ToolbarStyle.Small; //將原先的圖標變小
            rpt.ToolbarIconsStyle = ToolbarIconsStyle.Blue;
            rpt.ToolbarBackgroundStyle = ToolbarBackgroundStyle.Light;
            //rpt.PreviewMode = true;//预览模式
            //rpt.PrintInPdf = true;//在PDF打印
            ViewBag.WebReport = rpt;
            return View("/Areas/Base/Views/Print/Print.cshtml");
        }

        [HttpPost]
        public JsonResult GetSelectReport(string ID,string it_customer, string m_id)
        {           
            var result = InvoiceDAL.GetSelectReport(ID,it_customer, m_id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult CreateService1()
        //{

        //    //PlanDAL clsPlanDAL = new PlanDAL();
        //    var result = "";
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //public JsonResult CreateService()//<T>() where T : new()
        //{

        //    //PlanDAL clsPlanDAL = new PlanDAL();
        //    var result = InvoiceDAL.CreateService<InvoiceModel>();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult DataTableReturnList()//<T>() where T : new()
        //{

        //    //PlanDAL clsPlanDAL = new PlanDAL();
        //    var result = InvoiceDAL.DataTableReturnList();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public JsonResult AdditionSaveInvoice(so_invoice_mostly InvMostly)
        {
            //PlanDAL clsPlanDAL = new PlanDAL();
            var result = InvoiceDAL.AdditionSaveInvoice(InvMostly);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}