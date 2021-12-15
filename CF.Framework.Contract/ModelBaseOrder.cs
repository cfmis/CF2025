using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.Framework.Contract
{
    public class ModelBaseOrder
    {
        public ModelBaseOrder()
        {
            CreateTime = DateTime.Now;
        }

        public virtual string ID { get; set; }
        public virtual string CreateUser { get; set; }

        public virtual DateTime CreateTime { get; set; }
    }
}
