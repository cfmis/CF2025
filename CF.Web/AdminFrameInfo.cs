using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CF.Account.Contract;

namespace CF.Web
{
    public class AdminFrameInfo
    {
        public static AdminFrameInfo Current = new AdminFrameInfo();

        public AdminFrameUser AdminFrameUser
        {
            get
            {
                string UserId = HttpContext.Current.Request.Cookies["USERINFO"]["USERID"];
                AdminFrameUser objModel = new AdminFrameUser();
                objModel.lblUserName = UserId;
                return objModel;
            }
        }
    }
}