﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.Framework.Contract
{
    public class ModelBase
    {
        public ModelBase()
        {
            CreateTime = DateTime.Now;
        }

        public virtual int ID { get; set; }
        
        public virtual DateTime CreateTime { get; set; }
    }
}
