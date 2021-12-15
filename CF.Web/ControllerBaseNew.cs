using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CF.Framework.Web;

namespace CF.Web
{
    public abstract class ControllerBaseNew : CF.Framework.Web.ControllerBase
    {
        //protected override void LogException(Exception exception,
        //    WebExceptionContext exceptionContext = null)
        //{
        //    base.LogException(exception);

        //    var message = new
        //    {
        //        exception = exception.Message,
        //        exceptionContext = exceptionContext,
        //    };

        //    Log4NetHelper.Error(LoggerType.WebExceptionLog, message, exception);
        //}

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
