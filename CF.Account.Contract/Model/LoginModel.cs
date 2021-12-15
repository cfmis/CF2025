using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CF.Framework.Utility;
using CF.Framework.Contract;

namespace CF.Account.Contract
{
    public class LoginModel : ModelBase
    {
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "User Name Empty!")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password Empty!")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^\w+$", ErrorMessage = "Password Format Error")]
        public string Password { get; set; }


        [Display(Name = "记住登陆?")]
        public bool RememberMe { get; set; }

        public int TypeID { get; set; }

        public int SubClassID { get; set; }
 
        public string RealName { get; set; }
 
        public string Sex { get; set; }
 
        public int GroupID { get; set; }

        public string Dept { get; set; }
        [Display(Name = "語言")]
        [Required(ErrorMessage = "語言不能为空")]
        public string LangId { get; set; }
        public string State { get; set; }
        //private string _State;
        public string Login()
        {
            //该方法应从数据库中对比用户名和密码,并取得用户权限列表
            //这里为了简单直接对比字符串并返回权限列表,返回NULL则说明用户名或者密码错误
            //权限列表即为用,分割的权限名称
            string result = null;

            //if (this.UserName == "guest" & this.Password == "guest")
            //    result = "Add";

            //if (this.UserName == "admin" & this.Password == "admin")
            //    result = "Add,Edit";

            result = this.LangId;

            return result;

        }

        //public string State
        //{
        //    get { return _State; }
        //    set { _State = value; }
        //}

    }

    /// <summary>
    /// 語言
    /// </summary>
    public enum EnumLang
    {
        [EnumTitle("繁體中文")]
        Ch = 0,
        [EnumTitle("簡體中文")]
        Cn = 1,
        [EnumTitle("English")]
        En = 2
    }
}
