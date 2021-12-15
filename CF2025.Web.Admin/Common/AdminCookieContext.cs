using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.Core.Cache;
using CF.Framework.Utility;
using CF.Web;

namespace CF2025.Web.Admin.Common
{
    public class AdminCookieContext : CookieContext
    {
        public static AdminCookieContext Current
        {
            get
            {
                return CacheHelper.GetItem<AdminCookieContext>();
            }
        }

        public override string KeyPrefix
        {
            get
            {
                return Fetch.ServerDomain + "_AdminContext_";
            }
        }
    }
}
