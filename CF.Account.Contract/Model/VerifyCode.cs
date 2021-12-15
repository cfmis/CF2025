using System;
using System.Linq;
using CF.Framework.Contract;
using System.Collections.Generic;
using CF.Framework.Utility;
using System.ComponentModel.DataAnnotations.Schema;

namespace CF.Account.Contract
{
    [Serializable]
    [Table("VerifyCode")]
    public class VerifyCode : ModelBase
    {
        public Guid Guid { get; set; }
        public string VerifyText { get; set; }
    }
}
