using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using Newtonsoft.Json;
using CF.Core.Config;
using CF.Framework.Contract;
using CF.Framework.Utility;
using CF.Account.Contract;
using CF.Framework.Web;
using CF.Web;
using System.Threading;

namespace CF2025.Web.Admin.Common
{
    public abstract class AdminControllerBase : CF.Web.ControllerBase
    {
        public AdminCookieContext CookieContext
        {
            get
            {
                return AdminCookieContext.Current;
            }
        }

        public AdminUserContext UserContext
        {
            get
            {
                return AdminUserContext.Current;
            }
        }

        public CachedConfigContext ConfigContext
        {
            get
            {
                return CachedConfigContext.Current;
            }
        }

        /// <summary>
        /// 重写分页Size
        /// </summary>
        public override int PageSize
        {
            get
            {
                return 12;
            }
        }

        /// <summary>
        /// 操作人，为了记录操作历史
        /// </summary>
        public override Operater Operater
        {
            get
            {
                return new Operater()
                {
                    Name = this.LoginInfo == null ? "" : this.LoginInfo.LoginName,
                    Token = this.LoginInfo == null ? Guid.Empty : this.LoginInfo.LoginToken,
                    UserId = this.LoginInfo == null ? 0 : this.LoginInfo.UserID,
                    Time = DateTime.Now,
                    IP = Fetch.UserIp
                };
            }
        }

        /// <summary>
        /// 用户Token，每次页面都会把这个UserToken标识发送到服务端认证
        /// </summary>
        public virtual Guid UserToken
        {
            get
            {
                return CookieContext.UserToken;
            }
        }

        /// <summary>
        /// 登录后用户信息
        /// </summary>
        public virtual LoginInfo LoginInfo
        {
            get
            {
                return UserContext.LoginInfo;
            }
        }

        /// <summary>
        /// 登录后用户信息里的用户权限
        /// </summary>
        public virtual List<EnumBusinessPermission> PermissionList
        {
            get
            {
                var permissionList = new List<EnumBusinessPermission>();

                if (this.LoginInfo != null)
                    permissionList = this.LoginInfo.BusinessPermissionList;

                return permissionList;
            }
        }

        #region Override controller methods
        /// <summary>
        /// 方法执行前，如果没有登录就调整到Passport登录页面，没有权限就抛出信息
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var noAuthorizeAttributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthorizeIgnoreAttribute), false);
            if (noAuthorizeAttributes.Length > 0)
                return;
            base.OnActionExecuting(filterContext);

            if (this.LoginInfo == null)
            {
                //filterContext.Result = RedirectToAction("Login", "Auth", new { Area = "Account" });
                filterContext.Result = Redirect("/Home/Login");
                return;
            }

            bool hasPermission = true;
            var permissionAttributes = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(PermissionAttribute), false).Cast<PermissionAttribute>();
            permissionAttributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(PermissionAttribute), false).Cast<PermissionAttribute>().Union(permissionAttributes);
            var attributes = permissionAttributes as IList<PermissionAttribute> ?? permissionAttributes.ToList();
            if (permissionAttributes != null && attributes.Count() > 0)
            {
                hasPermission = true;
                foreach (var attr in attributes)
                {
                    foreach (var permission in attr.Permissions)
                    {
                        if (!this.LoginInfo.BusinessPermissionList.Contains(permission))
                        {
                            hasPermission = false;
                            break;
                        }
                    }
                }

                if (!hasPermission)
                {
                    if (Request.UrlReferrer != null)
                        filterContext.Result = this.Stop("没有权限！", Request.UrlReferrer.AbsoluteUri);
                    else
                        filterContext.Result = Content("没有权限！");
                }
            }
        }

        /// <summary>
        /// 方法后执行后注入一些视图数据
        /// </summary>
        /// <param name="filterContext">filter context</param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {   
            if (filterContext.ActionDescriptor.ActionName.Contains("Edit") ||
                filterContext.ActionDescriptor.ActionName.Contains("Add"))
                return;

            RenderViewData();
        }

        /// <summary>
        /// 如果是Ajax请求的话，清除浏览器缓存
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.HttpContext.Response.Cache.SetNoStore();
            }

            base.OnResultExecuted(filterContext);
        }

        /// <summary>
        /// 注入资源，权限，城市等信息
        /// </summary>
        protected override void RenderViewData()
        {
            //var permissions = string.Join(",", this.PermissionList);
            //this.ViewData["permissions"] = permissions;
        }

        #endregion

        #region 多語言轉換
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            //HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies["LanguageID"];
            var LanguageID = this.CookieContext.LanguageID.ToString().Trim();
            string langName = "";
            switch (LanguageID)
            {
                case "0":
                    langName = "zh-TW";//繁體中文
                    break;
                case "1":
                    langName = "zh-CN";//簡體中文
                    break;
                case "2":
                    langName = "en";   //英文
                    break;
                default:
                    langName = "en";
                    break;
            }
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(langName);
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(langName);
            return base.BeginExecuteCore(callback, state);
        }
        #endregion
    }
}
