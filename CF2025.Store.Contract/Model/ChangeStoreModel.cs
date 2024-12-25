using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Store.Contract.Model
{
    public class ChangeStoreModel
    {
    }

    public class PlanGoods
    {
        public string mo_id { get; set; }
        public string goods_id { get; set; }
        public string goods_name { get; set; }
        public string lot_no { get; set; }
        public decimal qty { get; set; }
        public decimal sec_qty { get; set; }
        public decimal average_cost { get; set; }
        public string vendor_id { get; set; }
        public string vendor_name { get; set; }

    }
}
