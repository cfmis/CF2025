using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FastReport.Web;

namespace CF2025.Web.Admin.Areas.Base.Controllers
{
    public class PrintController : Controller
    {
        // GET: Base/Print
        public ActionResult Index()
        {
            return View();
        }

        //打印
        WebReport rpt = new WebReport();
        public ActionResult Print()
        {
            return View();
        }
    }
}