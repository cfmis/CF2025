using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CF.Framework.Web;
using CF.Core.Log;
using CF.Account.Contract;
//using CF.Crm.Contract;

namespace CF.Web
{
    public abstract class ControllerBase : CF.Framework.Web.ControllerBase
    {
        public virtual IAccountService AccountService
        {
            get
            {
                return ServiceContext.Current.AccountService;
            }
        }

        //public virtual ICmsService CmsService
        //{
        //    get
        //    {
        //        return ServiceContext.Current.CmsService;
        //    }
        //}

        //public virtual ICrmService CrmService
        //{
        //    get
        //    {
        //        return ServiceContext.Current.CrmService;
        //    }
        //}

        //public virtual IOAService OAService
        //{
        //    get
        //    {
        //        return ServiceContext.Current.OAService;
        //    }
        //}

        protected override void LogException(Exception exception,
            WebExceptionContext exceptionContext = null)
        {
            base.LogException(exception);

            var message = new
            {
                exception = exception.Message,
                exceptionContext = exceptionContext,
            };

            Log4NetHelper.Error(LoggerType.WebExceptionLog, message, exception);
        }

        public IDictionary<string, object> CurrentActionParameters { get; set; }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }
    }
}
