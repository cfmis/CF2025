using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;

namespace CF2025.Web.Admin.Areas.Sale.Controllers
{
    public class OrderController : AdminControllerBase//: Controller//
    {
        // GET: Sale/Order
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        public ActionResult Search()
        {
            return View();
        }
    }
}