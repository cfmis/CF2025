using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CF.Account.Contract
{
    public class TbSyUser
    {
        public int Uid { get; set; }
        [Key]
        [StringLength(20, ErrorMessage = "編碼代號不能超过20个字")]
        [Required(ErrorMessage = "編碼代號不能为空")]
        public string Uname { get; set; }

        [StringLength(20, ErrorMessage = "編碼代號不能超过20个字")]
        [Required(ErrorMessage = "編碼代號不能为空")]
        public string Uname_desc { get; set; }
        public string UserGroupEmail { get; set; }
        public string DefaultCustID { get; set; }
        public string OrderStartID { get; set; }

    }
}
