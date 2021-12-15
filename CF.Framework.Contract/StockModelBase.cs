using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CF.Framework.Contract
{
    public class StockModelBase
    {
        public StockModelBase()
        {
            CreateTime = DateTime.Now;
            AmendTime = DateTime.Now;
        }

        [Required]
        public virtual string ID { get; set; }
        public virtual string CreateUser { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string AmendUser { get; set; }
        public virtual DateTime AmendTime { get; set; }
    }
}
