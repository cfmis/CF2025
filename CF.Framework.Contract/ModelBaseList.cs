using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.Framework.Contract
{ 
    public class ModelBaseList
    {
        public string value { get; set; }
        public string label { get; set; }

    }
    public class UpdateStatusModel
    {
        public string Status { get; set; }
        public string Msg { get; set; }
        public string ReturnValue { get; set; }
    }
}
