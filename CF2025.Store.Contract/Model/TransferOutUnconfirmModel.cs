using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Store.Contract
{
    public class TransferOutUnconfirmModel
    {
    }
    public class TransferOutFind
    {
        public string id { get; set; }
        public string id_end { get; set; }
        public string mo_id { get; set; }
        public string mo_id_end { get; set; }
        public string goods_id { get; set; }
        public string goods_id_end { get; set; }
        public string transfer_date { get; set; }
        public string transfer_date_end { get; set; }
        public string goods_name { get; set; }
        public string location_id { get; set; }
        public string shelf { get; set; }
        public string customer_id { get; set; }
        public string customer_id_end { get; set; }
    }

    public class TransferOutDetails
    {
        public string id { get; set; }
        public string transfer_date { get; set; }
        public string mo_id { get; set; }
        public string goods_id { get; set; }
        public string goods_name { get; set; }
        public string color { get; set; }
        public string unit { get; set; }
        public decimal transfer_amount { get; set; }
        public decimal al_transfer_amount { get; set; }
        public string sec_unit { get; set; }
        public decimal sec_qty { get; set; }
        public decimal package_num { get; set; }
        public decimal nwt { get; set; }
        public decimal gross_wt { get; set; }
        public string location_id { get; set; }
        public string move_location_id { get; set; }
        public decimal inventory_qty { get; set; }
        public string lot_no { get; set; }
        public string remark { get; set; }
    }
}
