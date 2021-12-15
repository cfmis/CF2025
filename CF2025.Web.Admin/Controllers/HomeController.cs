using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CF2025.Web.Admin.Common;
using CF.Framework.Web;
using CF2025.Sys.DAL;
using CF.Framework.Contract;

namespace CF2025.Web.Admin.Controllers
{
    public class HomeController : AdminControllerBase//: Controller//
    {
        public ActionResult Index()
        {
            //return RedirectToAction("Index", "Auth", new { Area = "Account" });
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [AuthorizeIgnore]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AuthorizeIgnore]
        //public ActionResult Login(string username, string password, string verifycode,string languageid)
        //{
        public ActionResult Login(string loginUsername, string loginPassword, string verifycode, string languageID)
        {
            //////return RedirectToAction("Index");

            var loginInfo = this.AccountService.Login(loginUsername, loginPassword, languageID);
            if (loginInfo != null)
            {
                this.CookieContext.UserToken = loginInfo.LoginToken;
                this.CookieContext.UserName = loginInfo.LoginName;
                this.CookieContext.UserId = loginInfo.UserID;
                this.CookieContext.LanguageID = loginInfo.LanguageID;
                return Json("OK");
                //return RedirectToAction("Index");
            }
            else
            {
                //ModelState.AddModelError("error", "用户名或密码错误");
                //return View();
                return Json("ERROR");
            }
            //if (username == "admin")
            //{
            //    return RedirectToAction("Index");
            //}
            //else
            //{
            //    ModelState.AddModelError("error", "用户名或密码错误");
            //    return View();
            //}
        }
        public ActionResult Logout()
        {
            this.AccountService.Logout(this.CookieContext.UserToken);
            this.CookieContext.UserToken = Guid.Empty;
            this.CookieContext.UserName = string.Empty;
            this.CookieContext.UserId = 0;
            return RedirectToAction("Login");
            //HttpResponse.write("<Script Language=Javascript>window.alert('恭喜您，退出成功！');location.href='user_login.asp';</Script>");
            //window.parent.location.replace("AdminLogin.aspx");
        }
        public ActionResult Top()
        {
            var LoginName = AdminUserContext.Current.LoginInfo.LoginName;//.BusinessPermissionList.Select(p => p.ToString());
            var LanguageID = AdminUserContext.Current.LoginInfo.LanguageID;
            GenAdminiMenu ga = new GenAdminiMenu();
            var model = ga.getAdminMenuList(LoginName, LanguageID);
            ViewBag.UserName = "超级管理员";
            ViewBag.AvailableBalance = "8888.00";
            return View(model);
        }
        public ActionResult Left()
        {
            //getAdminMenuDetailsList()
            var LoginName = AdminUserContext.Current.LoginInfo.LoginName;//.BusinessPermissionList.Select(p => p.ToString());
            var LanguageID = AdminUserContext.Current.LoginInfo.LanguageID;
            GenAdminiMenu ga = new GenAdminiMenu();
            var model = ga.getAdminMenuDetailsList(LoginName, LanguageID);
            return View(model);
        }
        [AuthorizeIgnore]
        public ActionResult Right()
        {
            return View();
        }
        [AuthorizeIgnore]
        public ActionResult RightMain()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CheckLogin()
        {
            var result = "OK";
            try{
                var LoginName = AdminUserContext.Current.LoginInfo.LoginName;//.BusinessPermissionList.Select(p => p.ToString());
            }
            catch
            {
                result = "ERROR";
            }
            //var LanguageID = AdminUserContext.Current.LoginInfo.LanguageID;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}