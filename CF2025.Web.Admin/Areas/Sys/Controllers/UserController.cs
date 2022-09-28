using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF.Framework.Contract;
using CF2025.Sys.DAL;

namespace CF2025.Web.Admin.Areas.Sys.Controllers
{
    public class UserController : Controller
    {
        // GET: Sys/User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Pwd()
        {
            return View();
        }
        //密碼更改確認
        [HttpPost]
        public JsonResult ModifyPwd(string UserName, string Pwd)
        {
            UpdateStatusModel result = new UpdateStatusModel();
            result = UserDAL.ModifyPwd(UserName, Pwd);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}